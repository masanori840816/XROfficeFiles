using System;
using System.Collections.Generic;

namespace XROfficeFiles.OfficeFiles.Values
{
    [Serializable]
    public class SpreadSheetCell
    {
        public long sheetId;
        public long groupId;        
        public long cellId;
        public int groupDisplayOrder;
        public string title;
        public int column;
        public int row;
        public int verticalLength;
        public int horizontalLength;
        public string value;
        public string formula;
        public string valueType;
        public string backgroundColor;
        public bool editabled;
        public bool verticalWriting;
        public int textRotation;
        public int borderLeft;
        public int borderTop;
        public int borderRight;
        public int borderBottom;
        public string fontName;
        public double fontSize;
        public string fontColor;
        public bool bold;
        public int mergedStartColumn;
        public int mergedStartRow;
        public int mergedEndColumn;
        public int mergedEndRow;
    }
}