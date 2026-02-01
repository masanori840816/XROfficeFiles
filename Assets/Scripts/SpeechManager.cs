using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class SpeechManager : MonoBehaviour
{   
    private const string PluginClassName = "jp.masanori.VoiceRecognitionActivity";
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private TMP_Text _message;
    private void Start()
    {
        StartCoroutine(DelayedCallAndroidFunction());
    }

    private IEnumerator DelayedCallAndroidFunction()
    {
        // 少なくとも1フレーム待つ、または0.5秒程度の短い遅延を入れる
        yield return new WaitForSeconds(0.5f); 
        
        CallAndroidFunction();
    }
    public void CallAndroidFunction()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            
            _message.text = "マイクパーミッションを要求中...";
            return; 
        }
        using (var pluginClass = new AndroidJavaClass(PluginClassName))
        {
            string androidVersion = pluginClass.CallStatic<string>("getAndroidVersion");
            _text.text = ("Android Version: " + androidVersion);
            Debug.Log("call log, startlistening");
            pluginClass.CallStatic("showLog", "Hello from Unity C#!");
            pluginClass.CallStatic("startListening");
        }
        
        #endif
    }
    public void RequestPermissionsAndStartListening()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        
        // 1. パーミッションが既に許可されているか確認
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // 許可されている場合、即座にプラグイン機能を実行
            ExecutePluginFunctions();
        }
        else
        {
            // 2. 許可されていない場合、コールバックを設定して要求
            _message.text = "マイクパーミッションを要求中...";
            
            PermissionCallbacks callbacks = new PermissionCallbacks();
            
            // 許可された場合の処理を割り当てる
            callbacks.PermissionGranted += OnPermissionGranted;
            
            // 拒否された場合の処理を割り当てる (オプションだが推奨)
            callbacks.PermissionDenied += OnPermissionDenied;
            
            // RequestUserPermission に PermissionCallbacks オブジェクトを渡す
            Permission.RequestUserPermission(Permission.Microphone, callbacks);
        }

        #endif
    }
    private void OnPermissionGranted(string permissionName)
    {
        if (permissionName == Permission.Microphone)
        {
            _message.text = "マイクパーミッションが許可されました。音声認識を開始します。";
            ExecutePluginFunctions();
        }
    }

    /// <summary>
    /// パーミッションが「拒否」されたときに呼び出されるメソッド
    /// </summary>
    private void OnPermissionDenied(string permissionName)
    {
        if (permissionName == Permission.Microphone)
        {
            _message.text = "マイクパーミッションが拒否されました。認識を開始できません。";
        }
    }

    private void ExecutePluginFunctions()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (var pluginClass = new AndroidJavaClass(PluginClassName))
        {
            // Androidバージョンの取得は、必要に応じてここに戻す
            string androidVersion = pluginClass.CallStatic<string>("getAndroidVersion");
            _text.text = ("Android Version: " + androidVersion);
            
            pluginClass.CallStatic("showLog", "Hello from Unity C#!");
            pluginClass.CallStatic("startListening");
        }
        #endif
    }
    public void OnRecognitionResult(string recognizedText)
    {
        if (string.IsNullOrEmpty(recognizedText))
        {
            _message.text = "認識結果: No Match";
        }
        else
        {
            _message.text = "認識結果: " + recognizedText;
        }
    }
    public void OnRecognitionError(string errorMessage)
    {
        _message.text = "音声認識エラー: " + errorMessage;
    }
}
