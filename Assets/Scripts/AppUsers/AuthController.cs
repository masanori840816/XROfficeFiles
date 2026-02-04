using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AuthController : MonoBehaviour
{
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private HttpClientManager httpClient;
    private string lastToken = null;
    
    public async Task<ApplicationResult> SignInAsync(string mailAddress, string password)
    {
        string serverAddress = saveDataManager.ServerAddress;
        if(string.IsNullOrEmpty(serverAddress))
        {
            return ApplicationResult.GetFailedResult("Server address is empty.");
        }
        if(string.IsNullOrEmpty(mailAddress) || string.IsNullOrEmpty(password))
        {
            return ApplicationResult.GetFailedResult("Mail address or password is empty.");
        }
        try
        {
            string xsrfToken = await httpClient.GetXSRFTokenAsync(serverAddress);
            if(string.IsNullOrEmpty(xsrfToken))
            {
                return ApplicationResult.GetFailedResult("Failed to get XSRF token.");
            }
            HttpClient client = httpClient.Client;
            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", xsrfToken);
            SignInValue value = new SignInValue
            {
                email = mailAddress,
                password = password,
            };
            StringContent content = new StringContent(
                JsonUtility.ToJson(value),
                Encoding.UTF8
            );
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            string url = $"{serverAddress}/api/users/signin";
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode(); 
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log($"Status: {response.StatusCode} body: {responseBody}");
            return ApplicationResult.GetFailedResult("Not implemented yet.");                        
        }
        catch (HttpRequestException ex)
        {
            Debug.Log($"[PostAsync]: {ex.Message}");
            return ApplicationResult.GetFailedResult("Failed signing in.");
        }
    }
    private void Start()
    {
        this.lastToken = saveDataManager.Token;
    }
}
