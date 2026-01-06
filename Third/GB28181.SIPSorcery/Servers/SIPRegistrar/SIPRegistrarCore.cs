// ============================================================================
// FileName: RegistrarCore.cs
//
// Description:
// SIP Registrar that strives to be RFC3822 compliant.
//
// Author(s):
// Aaron Clauson
//
// History:
// 21 Jan 2006	Aaron Clauson	Created.
// 22 Nov 2007  Aaron Clauson   Fixed bug where binding refresh was generating a duplicate exception if the uac endpoint changed but the contact did not.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//
// Copyright (c) 2006-2007 Aaron Clauson (aaronc@blueface.ie), Blue Face Ltd, Dublin, Ireland (www.blueface.ie)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
// the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
// disclaimer in the documentation and/or other materials provided with the distribution. Neither the name of Blue Face Ltd. 
// nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
// prior written permission. 
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.

using GB28181.App;
using GB28181.Cache;
using GB28181.Config;
using GB28181.Sys;
using GB28181.Sys.Model;
using Microsoft.Extensions.Logging;
using SIPSorcery.SIP;
using SIPSorcery.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;



namespace GB28181.Servers
{
    public enum RegisterResultEnum
    {
        Unknown = 0,
        Trying = 1,
        Forbidden = 2,
        Authenticated = 3,
        AuthenticationRequired = 4,
        Failed = 5,
        Error = 6,
        RequestWithNoUser = 7,
        RemoveAllRegistrations = 9,
        DuplicateRequest = 10,
        AuthenticatedFromCache = 11,
        RequestWithNoContact = 12,
        NonRegisterMethod = 13,
        DomainNotServiced = 14,
        IntervalTooBrief = 15,
        SwitchboardPaymentRequired = 16,
    }

    /// <summary>
    /// The registrar core is the class that actually does the work of receiving registration requests and populating and
    /// maintaining the SIP registrations list.
    /// 
    /// From RFC 3261 Chapter "10.2 Constructing the REGISTER Request"
    /// - Request-URI: The Request-URI names the domain of the location service for which the registration is meant.
    /// - The To header field contains the address of record whose registration is to be created, queried, or modified.  
    ///   The To header field and the Request-URI field typically differ, as the former contains a user name. 
    /// 
    /// [ed Therefore:
    /// - The Request-URI inidcates the domain for the registration and should match the domain in the To address of record.
    /// - The To address of record contians the username of the user that is attempting to authenticate the request.]
    /// 
    /// Method of operation:
    ///  - New SIP messages received by the SIP Transport layer and queued before being sent to RegistrarCode for processing. For requests
    ///    or response that match an existing REGISTER transaction the SIP Transport layer will handle the retransmit or drop the request if
    ///    it's already being processed.
    ///  - Any non-REGISTER requests received by the RegistrarCore are responded to with not supported,
    ///  - If a persistence is being used to store registered contacts there will generally be a number of threads running for the
    ///    persistence class. Of those threads there will be one that runs calling the SIPRegistrations.IdentifyDirtyContacts. This call identifies
    ///    expired contacts and initiates the sending of any keep alive and OPTIONs requests.
    /// </summary>
    public class SIPRegistrarCore : ISIPRegistrarCore
    {
        private const int MAX_REGISTER_QUEUE_SIZE = 1000;
        private const int MAX_PROCESS_REGISTER_SLEEP = 10000;

        private static ILogger logger = AppState.GetLogger("sipregistrar");

        private int m_minimumBindingExpiry = 100;

        private ISIPTransport _sipTransport;

        private SIPAuthenticateRequestDelegate _onSipRequestAuthenticator = null;
        public void SetAuthenticateRequestDelegate(SIPAuthenticateRequestDelegate ac)
        {
            _onSipRequestAuthenticator = ac;
        }
        private string m_serverAgent = GBSIPConstants.SIP_USERAGENT_STRING;
        private Queue<SIPNonInviteTransaction> m_registerQueue = new Queue<SIPNonInviteTransaction>();
        private AutoResetEvent m_registerARE = new AutoResetEvent(false);
        public event Action<double, bool> RegisterComplete;     // Event to allow hook into get notifications about the processing time for registrations. The boolean parameter is true of the request contained an authentication header.

        public int BacklogLength => m_registerQueue.Count;

        public bool Stop;


        private SIPAccount _localSipAccount;

        private IMemoCache<Camera> _cameraCache = null;

        /// <summary>
        /// �豸ע�ᵽDMS
        /// </summary>
        public event DmsRegisterDelegate DmsRegisterReceived;
        public event DeviceAlarmSubscribeDelegate DeviceAlarmSubscribe;

        public SIPRegistrarCore(ISIPTransport sipTransport, ISipStorage sipAccountStorage, IMemoCache<Camera> cameraCache)
        {
            _sipTransport = sipTransport;
            _localSipAccount = sipAccountStorage.GetLocalSipAccout();
            _cameraCache = cameraCache;
        }

        /// <summary>
        /// Authenticates a SIP request.
        /// </summary>
        public SIPRequestAuthenticationResult AuthenticateSIPRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest sipRequest, SIPAccount sipAccount)
        {
            try
            {
                if (sipAccount == null)
                {
                    return new SIPRequestAuthenticationResult(SIPResponseStatusCodesEnum.Forbidden, null);
                }
                else if (sipAccount.IsDisabled)
                {
                    return new SIPRequestAuthenticationResult(SIPResponseStatusCodesEnum.Forbidden, null);
                }
                else
                {
                    SIPAuthenticationHeader reqAuthHeader = sipRequest.Header.AuthenticationHeader;
                    if (reqAuthHeader == null)
                    {
                        // Check for IP address authentication.
                        if (!sipAccount.IPAddressACL.IsNullOrBlank())
                        {
                            SIPEndPoint uaEndPoint = (!sipRequest.Header.ProxyReceivedFrom.IsNullOrBlank()) ? SIPEndPoint.ParseSIPEndPoint(sipRequest.Header.ProxyReceivedFrom) : remoteEndPoint;
                            if (Regex.Match(uaEndPoint.GetIPEndPoint().ToString(), sipAccount.IPAddressACL).Success)
                            {
                                // Successfully authenticated
                                return new SIPRequestAuthenticationResult(true, true);
                            }
                        }

                        SIPAuthenticationHeader authHeader = new SIPAuthenticationHeader(SIPAuthorisationHeadersEnum.WWWAuthenticate, sipAccount.SIPDomain, SIPRequestAuthenticator.GetNonce());
                        return new SIPRequestAuthenticationResult(SIPResponseStatusCodesEnum.Unauthorised, authHeader);
                    }
                    else
                    {
                        return _onSipRequestAuthenticator.Invoke(localSIPEndPoint, remoteEndPoint, sipRequest, sipAccount);
                    }
                }
            }
            catch (Exception excp)
            {
                return new SIPRequestAuthenticationResult(SIPResponseStatusCodesEnum.InternalServerError, null);
            }
        }
        public void AddRegisterRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest registerRequest)
        {
            try
            {
                if (registerRequest.Method == SIPMethodsEnum.REGISTER)
                {
                    SIPSorceryPerformanceMonitor.IncrementCounter(SIPSorceryPerformanceMonitor.REGISTRAR_REGISTRATION_REQUESTS_PER_SECOND);

                    int requestedExpiry = GetRequestedExpiry(registerRequest);

                    if (registerRequest.Header.To == null)
                    {
                        logger.LogDebug("Bad register request, no To header from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest, SIPResponseStatusCodesEnum.BadRequest, "Missing To header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (registerRequest.Header.To.ToURI.User.IsNullOrBlank())
                    {
                        logger.LogDebug("Bad register request, no To user from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest, SIPResponseStatusCodesEnum.BadRequest, "Missing username on To header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (registerRequest.Header.Contact == null || registerRequest.Header.Contact.Count == 0)
                    {
                        logger.LogDebug("Bad register request, no Contact header from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest, SIPResponseStatusCodesEnum.BadRequest, "Missing Contact header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (requestedExpiry > 0 && requestedExpiry < m_minimumBindingExpiry)
                    {
                        logger.LogDebug("Bad register request, no expiry of " + requestedExpiry + " to small from " + remoteEndPoint + ".");
                        SIPResponse tooFrequentResponse = GetErrorResponse(registerRequest, SIPResponseStatusCodesEnum.IntervalTooBrief, null);
                        tooFrequentResponse.Header.MinExpires = m_minimumBindingExpiry;
                        _sipTransport.SendResponse(tooFrequentResponse);
                    }
                    else
                    {
                        if (m_registerQueue.Count < MAX_REGISTER_QUEUE_SIZE)
                        {
                            var registrarTransaction = _sipTransport.CreateNonInviteTransaction(registerRequest, remoteEndPoint, localSIPEndPoint, null);
                            lock (m_registerQueue)
                            {
                                m_registerQueue.Enqueue(registrarTransaction);
                            }
                            logger.LogDebug("m_registerQueue.Enqueue Counts: " + m_registerQueue.Count);
                        }
                        else
                        {
                            logger.LogError("Register queue exceeded max queue size " + MAX_REGISTER_QUEUE_SIZE + ", overloaded response sent.");
                            SIPResponse overloadedResponse = SIPTransport.GetResponse(registerRequest, SIPResponseStatusCodesEnum.TemporarilyUnavailable, "Registrar overloaded, please try again shortly");
                            _sipTransport.SendResponse(overloadedResponse);
                        }

                        m_registerARE.Set();
                    }
                }
            }
            catch (Exception excp)
            {
                logger.LogError("Exception AddRegisterRequest (" + remoteEndPoint.ToString() + "). " + excp.Message);
            }
        }

        public void ProcessRegisterRequest()
        {
            logger.LogDebug("SIPRegistrarCore is running at " + _localSipAccount.MsgProtocol + ":" + _localSipAccount.LocalIP + ":" + _localSipAccount.LocalPort);
            try
            {
                while (!Stop)
                {
                    if (m_registerQueue.Count > 0)
                    {
                        try
                        {
                            SIPNonInviteTransaction registrarTransaction = null;
                            lock (m_registerQueue)
                            {
                                registrarTransaction = m_registerQueue.Dequeue();
                            }

                            if (registrarTransaction != null)
                            {
                                DateTime startTime = DateTime.Now;
                                var result = Register(registrarTransaction);
                                var duration = DateTime.Now.Subtract(startTime);
                                RegisterComplete?.Invoke(duration.TotalMilliseconds, registrarTransaction.TransactionRequest.Header.AuthenticationHeader != null);

                                logger.LogDebug("Camera[" + registrarTransaction.RemoteEndPoint + "] have completed registering GB service.");
                                //CacheDeviceItem(registrarTransaction.TransactionRequest);

                                //device alarm subscribe
                                DeviceAlarmSubscribe?.Invoke(registrarTransaction);
                            }
                        }
                        catch (InvalidOperationException invalidOpExcp)
                        {
                            // This occurs when the queue is empty.
                            logger.LogWarning("InvalidOperationException ProcessRegisterRequest Register Job. " + invalidOpExcp.Message);
                        }
                        catch (Exception regExcp)
                        {
                            logger.LogError("Exception ProcessRegisterRequest Register Job. " + regExcp.Message);
                        }
                    }
                    else
                    {
                        m_registerARE.WaitOne(MAX_PROCESS_REGISTER_SLEEP);
                    }
                }

                logger.LogWarning("ProcessRegisterRequest thread " + Thread.CurrentThread.Name + " stopping.");
            }
            catch (Exception excp)
            {
                logger.LogError("Exception ProcessRegisterRequest (" + Thread.CurrentThread.Name + "). " + excp.Message);
            }
        }

        private static int GetRequestedExpiry(SIPRequest registerRequest)
        {
            int contactHeaderExpiry = (int)((registerRequest.Header.Contact != null && registerRequest.Header.Contact.Count > 0) ? registerRequest.Header.Contact[0].Expires : -1);
            return (contactHeaderExpiry == -1) ? (int)registerRequest.Header.Expires : contactHeaderExpiry;
        }

        private void CacheDeviceItem(SIPRequest sipRequest)
        {

            //Add Camera Item Into Cache
            _cameraCache.PlaceIn(sipRequest.URI.Host, new Camera()
            {
                DeviceID = sipRequest.Header.From.FromURI.User,
                IPAddress = sipRequest.Header.Vias.TopViaHeader.Host,
                Port = sipRequest.Header.Vias.TopViaHeader.Port
            });
        }

        private RegisterResultEnum Register(SIPTransaction registerTransaction)
        {
            try
            {
                SIPRequest sipRequest = registerTransaction.TransactionRequest;
                SIPURI registerURI = sipRequest.URI;
                SIPToHeader toHeader = sipRequest.Header.To;
                string toUser = toHeader.ToURI.User;
                //string canonicalDomain = (m_strictRealmHandling) ? GetCanonicalDomain_External(toHeader.ToURI.Host, true) : toHeader.ToURI.Host;
                string canonicalDomain = toHeader.ToURI.Host;
                int requestedExpiry = GetRequestedExpiry(sipRequest);

                if (canonicalDomain == null)
                {
                    SIPResponse noDomainResponse = GetErrorResponse(sipRequest, SIPResponseStatusCodesEnum.Forbidden, "Domain not serviced");
                    registerTransaction.SendFinalResponse(noDomainResponse);
                    return RegisterResultEnum.DomainNotServiced;
                }


                SIPAccount sipAccount = new SIPAccount
                {
                    Id = Guid.NewGuid(),
                    Owner = "admin",
                    SIPUsername = toUser,
                    SIPDomain = canonicalDomain
                };

                SIPRequestAuthenticationResult authenticationResult = AuthenticateSIPRequest(registerTransaction.LocalSIPEndPoint, registerTransaction.RemoteEndPoint, sipRequest, sipAccount);

                if (!authenticationResult.Authenticated)
                {
                    // 401 Response with a fresh nonce needs to be sent.
                    SIPResponse authReqdResponse = SIPTransport.GetResponse(sipRequest, authenticationResult.ErrorResponse, null);
                    authReqdResponse.Header.AuthenticationHeaders[0] = authenticationResult.AuthenticationRequiredHeader;
                    registerTransaction.SendFinalResponse(authReqdResponse);

                    if (authenticationResult.ErrorResponse == SIPResponseStatusCodesEnum.Forbidden)
                    {
                        return RegisterResultEnum.Forbidden;
                    }
                    else
                    {
                        return RegisterResultEnum.AuthenticationRequired;
                    }
                }
                else
                {

                    if (sipRequest.Header.Contact == null || sipRequest.Header.Contact.Count == 0)
                    {

                        SIPResponse okResponse = GetOkResponse(sipRequest);

                        registerTransaction.SendFinalResponse(okResponse);

                        CacheDeviceItem(sipRequest);
                    }
                    else
                    {
                        SIPEndPoint uacRemoteEndPoint = SIPEndPoint.TryParse(sipRequest.Header.ProxyReceivedFrom) ?? registerTransaction.RemoteEndPoint;
                        SIPEndPoint proxySIPEndPoint = SIPEndPoint.TryParse(sipRequest.Header.ProxyReceivedOn);
                        SIPEndPoint registrarEndPoint = registerTransaction.LocalSIPEndPoint;
                        SIPResponseStatusCodesEnum updateResult = SIPResponseStatusCodesEnum.Ok;

                        DateTime startTime = DateTime.Now;
                        TimeSpan duration = DateTime.Now.Subtract(startTime);

                        if (updateResult == SIPResponseStatusCodesEnum.Ok)
                        {
                            string proxySocketStr = (proxySIPEndPoint != null) ? " (proxy=" + proxySIPEndPoint.ToString() + ")" : null;

                            SIPResponse okResponse = GetOkResponse(sipRequest);

                            registerTransaction.SendFinalResponse(okResponse);

                            //Add Camera Item Into Cache
                            CacheDeviceItem(sipRequest);
                        }
                        else
                        {
                            sipRequest.Header.Contact[0].Expires = m_minimumBindingExpiry;
                            SIPResponse okResponse = GetOkResponse(sipRequest);
                            registerTransaction.SendFinalResponse(okResponse);

                        }
                    }

                    return RegisterResultEnum.Authenticated;
                }
            }
            catch (Exception excp)
            {
                string regErrorMessage = "Exception registrarcore registering. " + excp.Message + "\r\n" + registerTransaction.TransactionRequest.ToString();
                logger.LogError(regErrorMessage);
                try
                {
                    SIPResponse errorResponse = GetErrorResponse(registerTransaction.TransactionRequest, SIPResponseStatusCodesEnum.InternalServerError, null);
                    registerTransaction.SendFinalResponse(errorResponse);
                }
                catch { }

                return RegisterResultEnum.Error;
            }
        }


        private SIPResponse GetOkResponse(SIPRequest sipRequest)
        {
            try
            {
                SIPResponse okResponse = SIPTransport.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                SIPHeader requestHeader = sipRequest.Header;
                okResponse.Header = new SIPHeader(requestHeader.Contact, requestHeader.From, requestHeader.To, requestHeader.CSeq, requestHeader.CallId);

                // RFC3261 has a To Tag on the example in section "24.1 Registration".
                if (okResponse.Header.To.ToTag == null || okResponse.Header.To.ToTag.Trim().Length == 0)
                {
                    okResponse.Header.To.ToTag = CallProperties.CreateNewTag();
                }

                okResponse.Header.CSeqMethod = requestHeader.CSeqMethod;
                okResponse.Header.Vias = requestHeader.Vias;
                //okResponse.Header.Server = m_serverAgent;
                okResponse.Header.UserAgent = m_serverAgent;
                okResponse.Header.MaxForwards = Int32.MinValue;
                okResponse.Header.SetDateHeader();

                return okResponse;
            }
            catch (Exception excp)
            {
                logger.LogError("Exception GetOkResponse. " + excp.Message);
                throw excp;
            }
        }

        private SIPResponse GetErrorResponse(SIPRequest sipRequest, SIPResponseStatusCodesEnum errorResponseCode, string errorMessage)
        {
            try
            {
                SIPResponse errorResponse = SIPTransport.GetResponse(sipRequest, errorResponseCode, null);
                if (errorMessage != null)
                {
                    errorResponse.ReasonPhrase = errorMessage;
                }

                SIPHeader requestHeader = sipRequest.Header;
                SIPHeader errorHeader = new SIPHeader(requestHeader.Contact, requestHeader.From, requestHeader.To, requestHeader.CSeq, requestHeader.CallId);

                if (errorHeader.To.ToTag == null || errorHeader.To.ToTag.Trim().Length == 0)
                {
                    errorHeader.To.ToTag = CallProperties.CreateNewTag();
                }

                errorHeader.CSeqMethod = requestHeader.CSeqMethod;
                errorHeader.Vias = requestHeader.Vias;
                //errorHeader.Server = m_serverAgent;
                errorHeader.UserAgent = m_serverAgent;
                errorHeader.MaxForwards = Int32.MinValue;

                errorResponse.Header = errorHeader;

                return errorResponse;
            }
            catch (Exception excp)
            {
                logger.LogError("Exception GetErrorResponse. " + excp.Message);
                throw excp;
            }
        }

    }
}
