using System.Net.Http;

namespace Marco.Http.Client.Abstractions
{
    public interface IMarcoHttpClientFactory
    {
        HttpClient Create();
        HttpClient Create(HttpClientHandler handler);
        HttpClient Create(HttpClientHandler handler, bool dispose);
    }
}