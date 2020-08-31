﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CommonServiceLocator;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.Common
{
    /// <summary>
    /// A sealed class that holds the instance of HttpClient for this lifetimescope.
    /// </summary>
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;
        private static Settings _settings;

        /// <summary>
        /// Gets or sets the connection URL.
        /// </summary>
        public static string BaseUrl
        {
            get; set;
        }

        /// <summary>
        /// Fetch the HttpClient instance.
        /// </summary>
        /// <returns>The HttpClient instance</returns>
        public static HttpClient Client(OpenHABHttpClientType connectionType, Settings settings)
        {
            _settings = settings;
            return _client ?? (_client = InitClient(connectionType));
        }

        /// <summary>
        /// Create an HttpClient instance for one-time use.
        /// </summary>
        /// <returns>The HttpClient instance.</returns>
        public static HttpClient DisposableClient(OpenHABHttpClientType connectionType, Settings settings)
        {
            _settings = settings;
            return InitClient(connectionType, true);
        }

        /// <summary>
        /// Forces creation of a new client, for example when the settings in the app are updated.
        /// </summary>
        public static void ResetClient()
        {
            _client = null;
        }

        private static HttpClient InitClient(OpenHABHttpClientType connectionType, bool disposable = false)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl) && !disposable)
            {
                return null;
            }

            var handler = new HttpClientHandler();

            if (_settings.WillIgnoreSSLCertificate.HasValue && _settings.WillIgnoreSSLHostname.HasValue)
            {
                handler.ServerCertificateCustomValidationCallback = CheckValidationResult;
            }

            var credentials = GetCredentials(connectionType);

            if (credentials != null)
            {
                handler.Credentials = credentials;
            }

            var client = new HttpClient(handler);
            if (!disposable)
            {
                client.BaseAddress = new Uri(BaseUrl);
            }

            return client;
        }

        private static NetworkCredential GetCredentials(OpenHABHttpClientType connectionType)
        {
            string username = connectionType == OpenHABHttpClientType.Local ? _settings.Username : _settings.RemoteUsername;
            string password = connectionType == OpenHABHttpClientType.Local ? _settings.Password : _settings.RemotePassword;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return new NetworkCredential(username, password);
            }

            return null;
        }

        private static bool CheckValidationResult(HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return result;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                result &= _settings.WillIgnoreSSLCertificate.Value;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                result &= _settings.WillIgnoreSSLHostname.Value;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                result = false;
            }

            return result;
        }
    }
}
