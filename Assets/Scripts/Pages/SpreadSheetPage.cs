using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XROfficeFiles.OfficeFiles.Values;

namespace XROfficeFiles.Pages
{
    public class SpreadSheetPage : MonoBehaviour, IPageBehaviour
    {
        [SerializeField] private Transform tableRoot;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private GameObject cellPrefab;

        [SerializeField] private float widthMultiplier = 100f;
        [SerializeField] private float heightMultiplier = 100f;
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
            SpreadSheet sheet = await LoadJsonFromStreamingAssets("sample.json");
            if(sheet == null)
            {
                Debug.Log("Failed to load spreadsheet.");
                return;
            }
            var rowGroups = sheet.cells.GroupBy(c => c.row).OrderBy(g => g.Key);
            foreach (var rowGroup in rowGroups) {
                int rowIdx = rowGroup.Key;
                GameObject rowObj = Instantiate(rowPrefab, tableRoot);
                double h = sheet.rowHeights.FirstOrDefault(rh => rh.row == rowIdx)?.height ?? 0.5d;
                rowObj.GetComponent<LayoutElement>().preferredHeight = (float)h * heightMultiplier;
                int currentRow = rowIdx;
                rowObj.GetComponent<Button>().onClick.AddListener(() => OnRowSelected(currentRow));
                var cellsInRow = rowGroup.OrderBy(c => c.column);
                foreach (var cell in cellsInRow)
                {
                    if (cell.row != cell.mergedStartRow || cell.column != cell.mergedStartColumn) {
                        continue;
                    }

                GameObject cellObj = Instantiate(cellPrefab, rowObj.transform);
                
                cellObj.GetComponentInChildren<TMPro.TMP_Text>().text = cell.value;

                // セルの幅を計算（結合されている場合はその分加算）
                double totalWidth = 0d;
                for (int i = cell.mergedStartColumn; i <= cell.mergedEndColumn; i++) {
                    totalWidth += sheet.columnWidths.FirstOrDefault(cw => cw.column == i)?.width ?? 1d;
                }
                
                LayoutElement le = cellObj.GetComponent<LayoutElement>();
                le.preferredWidth = (float)totalWidth * widthMultiplier;

                // set background color
                if (!string.IsNullOrEmpty(cell.backgroundColor)) {
                    Color bgColor;
                    if (ColorUtility.TryParseHtmlString("#" + cell.backgroundColor, out bgColor)) {
                        cellObj.GetComponent<Image>().color = bgColor;
                    }
                }
                }

            }
        }

        private void OnRowSelected(int selectedRow)
        {
            Debug.Log("selectedRow." + selectedRow);
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