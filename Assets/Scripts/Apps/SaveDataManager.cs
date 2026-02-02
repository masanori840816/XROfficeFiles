using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public string MailAddress { get; private set; } = "";
    public string ServerAddress { get; private set; } = "";
    public string Token { get; private set; } = "";

    public void UpdateMailAddress(string mailAddress)
    {
        MailAddress = mailAddress;
        PlayerPrefs.SetString("MailAddress", mailAddress);
    }
    public void UpdateServerAddress(string serverAddress)
    {
        ServerAddress = serverAddress;
        PlayerPrefs.SetString("ServerAddress", serverAddress);
    }
    public void UpdateToken(string token)
    {
        Token = token;
        PlayerPrefs.SetString("Token", token);
    }
    private void Awake()
    {
        Load();
    }
    private void Load()
    {
        MailAddress = PlayerPrefs.GetString("MailAddress", "");
        ServerAddress = PlayerPrefs.GetString("ServerAddress", "");
        Token = PlayerPrefs.GetString("Token", "");
    }
}
