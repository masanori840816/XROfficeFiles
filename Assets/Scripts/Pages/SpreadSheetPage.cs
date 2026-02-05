using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using XROfficeFiles.OfficeFiles.Values;

namespace XROfficeFiles.Pages
{
    public class SpreadSheetPage : MonoBehaviour, IPageBehaviour
    {
        [SerializeField] private SaveDataManager saveDataManager;

        public Action<AppPage> MoveNextPage { get; set; }
        public void OpenPage()
        {
            
        }
        public void ClosePage()
        {
            
        }
        
        private async void Start()
        {
            Debug.Log("InspectionPage enabled.");
            
            SpreadSheet sheet = await LoadJsonFromStreamingAssets("sample.json");
            if(sheet == null)
            {
                Debug.Log("Failed to load spreadsheet.");
                return;
            }
            Debug.Log($"sheet ID:{sheet.sheetId}");
        }

        private void OnDisable()
        {
            Debug.Log("InspectionPage disabled.");
        }

        private async Task<SpreadSheet> LoadJsonFromStreamingAssets(string fileName)
        {
            // 1. プラットフォームごとの適切なパスを取得
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            
            // Android の場合は URL 形式 (jar:file://...) になるため UnityWebRequest が必須
            using (UnityWebRequest request = UnityWebRequest.Get(filePath))
            {
                // 2. 読み込み開始
                var operation = request.SendWebRequest();

                // 読み込み完了まで待機
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonText = request.downloadHandler.text;
                    
                    // 3. JsonUtility でデシリアライズ
                    // ※ JSONのルートが配列（[ ]）で始まっている場合は、ラッパークラスが必要です
                    return JsonUtility.FromJson<SpreadSheet>(jsonText);
                }
                else
                {
                    Debug.LogError($"JSONの読み込みに失敗しました: {request.error}");
                    return null;
                }
            }
        }
    }
}