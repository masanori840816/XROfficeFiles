using UnityEngine;

public class SigninPage : MonoBehaviour
{
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private TMPro.TMP_InputField mailAddressInput;
    [SerializeField] private TMPro.TMP_Text errorMessageText;
    
    [SerializeField] private TMPro.TMP_InputField passwordInput;
    public void Signin()
    {
        if(string.IsNullOrEmpty(mailAddressInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            errorMessageText.gameObject.SetActive(true);
            return;
        }
        errorMessageText.gameObject.SetActive(false);
        
        Debug.Log("Signin clicked");
    }
}
