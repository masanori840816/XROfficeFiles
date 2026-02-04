using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class HttpClientManager : MonoBehaviour
{
    private CookieContainer cookieContainer = new CookieContainer();
    public HttpClient Client { get; private set; }
    private void Awake()
    {
#if UNITY_EDITOR
        // Turn off proxies when executing in Unity Editor
        HttpClientHandler handler = new HttpClientHandler()
        {
            Proxy = null,
            UseProxy = false,
            CookieContainer = cookieContainer,
            UseCookies = true
        };
        this.Client = new HttpClient(handler);
#else
        this.Client = new HttpClient();
#endif
    }
    public async Task<string> GetXSRFTokenAsync(string serverAddress)
    {
        try
        {
            string url = $"{serverAddress}/api/xsrf-token";
            HttpResponseMessage response = await this.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri(url)))
            {
                if (cookie.Name == "XSRF-TOKEN")
                {
                    return WebUtility.UrlDecode(cookie.Value);
                }
            }
            Debug.LogWarning("XSRF-TOKEN cookie not found.");
            return null;
        }   
        catch (HttpRequestException ex)
        {
            Debug.LogError($"[GetXSRFTokenAsync]: {ex.Message}");
            return null;
        }
    }
    private void OnDestroy()
    {
        this.Client?.Dispose();
    }
}
