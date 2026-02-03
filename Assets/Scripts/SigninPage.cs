using UnityEngine;

public class SigninPage : MonoBehaviour
{
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private AuthController authController;
    [SerializeField] private TMPro.TMP_InputField mailAddressInput;
    [SerializeField] private TMPro.TMP_Text errorMessageText;
    
    [SerializeField] private TMPro.TMP_InputField passwordInput;
    public async void Signin()
    {
        ApplicationResult result = await authController.SignInAsync(
            mailAddressInput.text,
            passwordInput.text);            
        if(result.succeeded == false)
        {
            errorMessageText.text = result.errorMessage;
            errorMessageText.gameObject.SetActive(true);
            return;
        }
        errorMessageText.gameObject.SetActive(false);
        // Open next page
        Debug.Log("Signin clicked");
    }
}
