using System.Net.Http;
using UnityEngine;

public class HttpClientManager : MonoBehaviour
{
    public HttpClient Client { get; private set; }
    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Log("aaaa om ");
        // Turn off proxies when executing in Unity Editor
        HttpClientHandler handler = new HttpClientHandler()
        {
            Proxy = null,
            UseProxy = false,
        };
        this.Client = new HttpClient(handler);
#else
        this.Client = new HttpClient();
#endif
    }
    private void OnDestroy()
    {
        this.Client?.Dispose();
    }
}
