using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private TMPro.TMP_InputField serverAddressInput;
    
    private string lastServerAddress = "";

    public void SaveLastValues()
    {
        lastServerAddress = serverAddressInput.text;
    }
    public void CancelChanges()
    {
        serverAddressInput.text = lastServerAddress;
    }
    public void SaveChanges()
    {
        saveDataManager.UpdateServerAddress(serverAddressInput.text);
        lastServerAddress = serverAddressInput.text;
    }

    private void Start()
    {
        if(string.IsNullOrEmpty(saveDataManager.ServerAddress) == false)
        {
            serverAddressInput.text = saveDataManager.ServerAddress;
        }        
    }
}
