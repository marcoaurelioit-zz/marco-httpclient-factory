using Marco.Http.Client.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Reflection;

namespace Marco.Http.Client.AspNetCore
{
    internal class MarcoHttpClientFactory : IMarcoHttpClientFactory
    {
        #region [+] Attrs
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string XRootCorrelationIdHeaderKey = "X-Root-Correlation-Id";
        private const string XCorrelationIdHeaderKey = "X-Correlation-Id";
        private const string XRootConsumerNameHeaderKey = "X-Root-Consumer-Name";
        private const string XConsumerNameHeaderKey = "X-Consumer-Name";
        private const string XFullTraceHeaderKey = "X-Full-Trace";
        private static readonly string ApplicationName = $"{Assembly.GetEntryAssembly().GetName().Name}-{Environment.MachineName}";
        #endregion

        #region [+] Ctors
        public MarcoHttpClientFactory(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        #endregion

        #region [+] Mtds
        public HttpClient Create() => CustomHeaders(new HttpClient());
        public HttpClient Create(HttpClientHandler handler) => CustomHeaders(new HttpClient(handler));
        public HttpClient Create(HttpClientHandler handler, bool dispose) => CustomHeaders(new HttpClient(handler, dispose)); 
        #endregion

        #region [+] Pvts
        private HttpClient CustomHeaders(HttpClient httpClient)
        {
            var httpContext = _httpContextAccessor?.HttpContext ??
                    throw new InvalidOperationException("Could not get a valid HttpContext of HttpContextAccessor.");

            var requestHeaders = httpContext.Request?.Headers ??
                throw new InvalidOperationException("Could not read request headers.");        

            var identifier = httpContext.TraceIdentifier;

            var traceContent = requestHeaders.ContainsKey(XFullTraceHeaderKey) ?
                $"{requestHeaders[XFullTraceHeaderKey]}; "
                : string.Empty;

            traceContent += $"{identifier} - {ApplicationName}";
            httpClient.DefaultRequestHeaders.Add(XConsumerNameHeaderKey, ApplicationName);
            httpClient.DefaultRequestHeaders.Add(XCorrelationIdHeaderKey, identifier);
            httpClient.DefaultRequestHeaders.Add(XFullTraceHeaderKey, traceContent);

            var correlationId = requestHeaders.ContainsKey(XRootCorrelationIdHeaderKey) ? (string)requestHeaders[XRootCorrelationIdHeaderKey] : identifier;
            var callerId = requestHeaders.ContainsKey(XRootConsumerNameHeaderKey) ? (string)requestHeaders[XRootConsumerNameHeaderKey] : ApplicationName;

            httpClient.DefaultRequestHeaders.Add(XRootCorrelationIdHeaderKey, correlationId);
            httpClient.DefaultRequestHeaders.Add(XRootConsumerNameHeaderKey, callerId);

            return httpClient;
        }
        #endregion
    }
}