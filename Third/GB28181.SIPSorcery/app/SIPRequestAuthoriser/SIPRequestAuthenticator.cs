//-----------------------------------------------------------------------------
// Filename: SIPRequestAuthoriser.cs
//
// Description: Central location to handle SIP Request authorisation.
// 
// History:
// 08 Mar 2009	Aaron Clauson	    Created.
// 30 May 2020	Edward Chen     Updated.
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//

using System;
using System.Text.RegularExpressions;
using GB28181.Sys;
using SIPSorcery.SIP;
using SIPSorcery.Sys;

namespace GB28181.App
{
    public class SIPRequestAuthenticator
    {
        private const int NONCE_REFRESH_SECONDS = 120;

        private static string m_previousNoncePrefix = null;
        private static string m_currentNoncePrefix = null;
        private static DateTime m_lastNoncePrefixUpdate = DateTime.Now;


        public static string GetNonce()
        {
            if (m_currentNoncePrefix == null || DateTime.Now.Subtract(m_lastNoncePrefixUpdate).TotalSeconds > NONCE_REFRESH_SECONDS)
            {
                m_lastNoncePrefixUpdate = DateTime.Now;
                m_previousNoncePrefix = m_currentNoncePrefix;
                m_currentNoncePrefix = Crypto.GetRandomInt().ToString();
            }

            return m_currentNoncePrefix + Crypto.GetRandomInt().ToString();
        }

        private static bool IsNonceStale(string nonce)
        {
            if (nonce.IsNullOrBlank())
            {
                return true;
            }
            else if (m_currentNoncePrefix != null && nonce.StartsWith(m_currentNoncePrefix))
            {
                return false;
            }
            else if (m_previousNoncePrefix != null && nonce.StartsWith(m_previousNoncePrefix))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
