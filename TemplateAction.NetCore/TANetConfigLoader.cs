using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace TemplateAction.NetCore
{
    public static class TANetConfigLoader
    {

        public static KestrelServerOptions CreateKestrelOptionsFrom(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var kestrelOptions = new KestrelServerOptions();
            kestrelOptions.ApplicationServices = serviceProvider;
            var kestrelConfigSection = configuration.GetSection("Kestrel");
            if (kestrelConfigSection != null)
            {
                LoadKestrelConfiguration(kestrelOptions, kestrelConfigSection);
            }
            return kestrelOptions;
        }
        private static void LoadKestrelConfiguration(KestrelServerOptions kestrelOptions, IConfigurationSection kestrelConfigSection)
        {
            try
            {
                // 处理端点配置 - 与标准Kestrel配置格式兼容
                var endpoints = kestrelConfigSection.GetSection("Endpoints");
                if (endpoints.Exists())
                {
                    foreach (var endpoint in endpoints.GetChildren())
                    {
                        ConfigureEndpoint(kestrelOptions, endpoint);
                    }
                }
                else
                {
                    // 没有配置端点时使用默认值
                    kestrelOptions.ListenAnyIP(880, o => o.Protocols = HttpProtocols.Http1AndHttp2);
                    Console.WriteLine("使用默认HTTP端点: http://localhost:880");
                }

                // 处理限制配置
                ConfigureLimits(kestrelOptions, kestrelConfigSection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载Kestrel配置失败: {ex.Message}");
                throw;
            }
        }
        private static (IPAddress address, int port) ParseWildcardUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            // 处理通配符主机名的特殊情况
            if (url.Contains("*:") || url.Contains("+:") || url.Contains("0.0.0.0:"))
            {
                // 提取端口号
                var portStartIndex = url.LastIndexOf(':') + 1;
                if (portStartIndex <= 0 || portStartIndex >= url.Length)
                    throw new FormatException($"URL格式无效: {url}");

                if (!int.TryParse(url.Substring(portStartIndex), out int port) || port < 1 || port > 65535)
                    throw new FormatException($"URL中的端口无效: {url}");

                // 通配符对应所有网络接口
                return (IPAddress.Any, port);
            }

            // 标准URL解析
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return (IPAddress.Any, uri.Port);
            }

            throw new FormatException($"无法解析URL: {url}");
        }
        private static void ConfigureEndpoint(KestrelServerOptions kestrelOptions, IConfigurationSection endpointSection)
        {
            var url = endpointSection["Url"];
            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine($"跳过配置错误的端点 '{endpointSection.Key}' - 未指定Url");
                return;
            }
            url = url.Trim();
            var (ipAddress, port) = ParseWildcardUrl(url);

            // 配置协议
            kestrelOptions.Listen(ipAddress, port, x =>
            {
                // 配置HTTPS
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    x.Protocols = HttpProtocols.Http1AndHttp2;
                    ConfigureHttps(x, endpointSection.GetSection("Certificate"));
                }
                else
                {
                    x.Protocols = HttpProtocols.Http1;
                }
            });
        }

        private static void ConfigureHttps(ListenOptions listenOptions, IConfigurationSection certSection)
        {
            if (!certSection.Exists())
            {
                throw new InvalidOperationException("HTTPS端点需要证书配置");
            }

            var certPath = certSection["Path"];
            var certPassword = certSection["Password"];
            var useDevelopmentCertificate = certSection.GetValue<bool>("UseDevelopmentCertificate");

            if (useDevelopmentCertificate)
            {
                // 使用开发证书
                listenOptions.UseHttps();
            }
            else if (!string.IsNullOrEmpty(certPath))
            {
                // 使用指定证书
                var fullPath = Path.IsPathRooted(certPath)
                    ? certPath
                    : Path.Combine(Directory.GetCurrentDirectory(), certPath);

                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException("未找到HTTPS证书文件", fullPath);
                }

                listenOptions.UseHttps(new X509Certificate2(fullPath, certPassword));
            }
            else
            {
                throw new InvalidOperationException("HTTPS端点需要有效的证书配置");
            }
        }

        private static void ConfigureLimits(KestrelServerOptions kestrelOptions, IConfigurationSection kestrelConfigSection)
        {
            var limits = kestrelConfigSection.GetSection("Limits");
            if (limits.Exists())
            {
                kestrelOptions.Limits.MaxRequestBodySize = limits.GetValue<long?>("MaxRequestBodySize") ?? 52428800;
                kestrelOptions.Limits.MaxConcurrentConnections = limits.GetValue<long?>("MaxConcurrentConnections");
                kestrelOptions.Limits.MaxConcurrentUpgradedConnections = limits.GetValue<long?>("MaxConcurrentUpgradedConnections");
                kestrelOptions.Limits.MaxRequestBufferSize = limits.GetValue<long?>("MaxRequestBufferSize");
            }
        }
    }
}
