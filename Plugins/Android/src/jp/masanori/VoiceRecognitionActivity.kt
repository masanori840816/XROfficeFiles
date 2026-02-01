package jp.masanori; // パッケージ名を統一

import android.Manifest
import android.speech.RecognitionListener
import android.speech.SpeechRecognizer
import android.content.Intent
import android.speech.RecognizerIntent
import android.content.Context
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import com.unity3d.player.UnityPlayer

class VoiceRecognitionActivity private constructor(private val context: Context) : RecognitionListener {
    
    private var speechRecognizer: SpeechRecognizer? = null
    private val unityGameObjectName = "SpeechManager"
    private val unityMethodName = "OnRecognitionResult"
    private val unityErrorMethodName = "OnRecognitionError"

    companion object {
        private const val TAG = "UnityPlugin"
        @JvmStatic
        private var instance: VoiceRecognitionActivity? = null

        @JvmStatic
        fun getInstance(): VoiceRecognitionActivity {
            if (instance == null) {
                val currentContext = UnityPlayer.currentActivity
                instance = VoiceRecognitionActivity(currentContext)
            }
            return instance!!
        }

        @JvmStatic
        fun getAndroidVersion(): String {
            return android.os.Build.VERSION.RELEASE
        }
        
        @JvmStatic
        fun startListening() {
            val plugin = getInstance() 
            plugin.checkAndStartListening()
        }
        
        @JvmStatic
        fun showLog(message: String) {
            Log.d(TAG, "Message from Unity: $message")
            val vr = getInstance() 
            vr.sendUnityMessage("Message To Unity $message")
        }
    }

    init {
        Handler(Looper.getMainLooper()).post {
            initializeSpeechRecognizer()
        }
    }

    private fun initializeSpeechRecognizer() {
       if (SpeechRecognizer.isRecognitionAvailable(context)) {
            // ContextがActivityであることを前提とする
            // この createSpeechRecognizer() もメインスレッドから呼ばれる必要がある
            speechRecognizer = SpeechRecognizer.createSpeechRecognizer(context).apply {
                setRecognitionListener(this@VoiceRecognitionActivity)
            }
            Log.d(TAG, "SpeechRecognizer initialized on Main Thread.")
        } else {
            sendUnityError("Recognition not available on this device")
            Log.e(TAG, "SpeechRecognizer not available.")
        }
    }
    fun checkAndStartListening() {
        Handler(Looper.getMainLooper()).post {
            if (speechRecognizer == null) {
                sendUnityError("SpeechRecognizer not initialized")
                return@post
            }
            
            val intent = Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH).apply {
                putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM)
                putExtra(RecognizerIntent.EXTRA_LANGUAGE, "ja-JP") // 日本語設定
                putExtra(RecognizerIntent.EXTRA_MAX_RESULTS, 1)
                putExtra(RecognizerIntent.EXTRA_PREFER_OFFLINE, true) // オフライン優先
            }
            
            try {
                speechRecognizer!!.startListening(intent)
                Log.d(TAG, "Speech recognition started.")
            } catch (e: Exception) {
                sendUnityError("StartListening failed: ${e.message}")
                Log.e(TAG, "StartListening exception: ${e.message}")
            }
        }
    }

    fun destroy() {
        if (speechRecognizer != null) {
            speechRecognizer!!.destroy()
            Log.d(TAG, "SpeechRecognizer destroyed.")
        }
        instance = null
    }
    
    override fun onResults(results: Bundle?) {
        val matches = results?.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION)
        if (!matches.isNullOrEmpty()) {
            val bestResult = matches[0]
            sendUnityMessage(bestResult)
        } else {
            sendUnityMessage("") // 認識結果なし
        }
    }
    
    override fun onError(error: Int) {
        val errorText = when(error) {
            SpeechRecognizer.ERROR_NETWORK_TIMEOUT -> "NETWORK_TIMEOUT"
            SpeechRecognizer.ERROR_NETWORK -> "NETWORK_ERROR"
            SpeechRecognizer.ERROR_INSUFFICIENT_PERMISSIONS -> "PERMISSION_ERROR"
            SpeechRecognizer.ERROR_NO_MATCH -> "NO_MATCH"
            SpeechRecognizer.ERROR_CLIENT -> "CLIENT_ERROR (e.g., stopping too early)"
            else -> "OTHER_ERROR: $error"
        }
        sendUnityError(errorText)
        Log.e(TAG, "Speech recognition error: $errorText")
    }

    override fun onReadyForSpeech(params: Bundle?) { Log.d(TAG, "onReadyForSpeech") }
    override fun onBeginningOfSpeech() { Log.d(TAG, "onBeginningOfSpeech") }
    override fun onBufferReceived(buffer: ByteArray?) {}
    override fun onEndOfSpeech() { Log.d(TAG, "onEndOfSpeech") }
    override fun onPartialResults(partialResults: Bundle?) {}
    override fun onEvent(eventType: Int, params: Bundle?) {}
    override fun onRmsChanged(rmsdB: Float) {}

    private fun sendUnityMessage(message: String) {
        UnityPlayer.UnitySendMessage(unityGameObjectName, unityMethodName, message)
        Log.d(TAG, "Sent result to Unity: $message")
    }
    
    private fun sendUnityError(errorMessage: String) {
        UnityPlayer.UnitySendMessage(unityGameObjectName, unityErrorMethodName, errorMessage)
        Log.e(TAG, "Sent error to Unity: $errorMessage")
    }
}