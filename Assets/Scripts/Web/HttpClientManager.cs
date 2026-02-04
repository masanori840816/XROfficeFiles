using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class HttpClientManager : MonoBehaviour
{
    private readonly CookieContainer cookieContainer = new CookieContainer();
    public HttpClient Client { get; private set; }
    public string GetCookieValue(string url, string cookieName)
    {
        foreach (Cookie cookie in cookieContainer.GetCookies(new Uri(url)))
        {
            if (cookie.Name == cookieName)
            {
                return WebUtility.UrlDecode(cookie.Value);
            }
        }
        return "";
    }
    public void SetCookieValue(string url, string cookieName, string cookieValue)
    {
        Cookie cookie = new Cookie(cookieName, WebUtility.UrlEncode(cookieValue))
        {
            Domain = new Uri(url).Host
        };
        cookieContainer.Add(cookie);
    }
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
#else
        HttpClientHandler handler = new HttpClientHandler()
        {
            CookieContainer = cookieContainer,
            UseCookies = true
        };
#endif
        this.Client = new HttpClient(handler);
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
