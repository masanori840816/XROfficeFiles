using System;
using System.Collections.Generic;

namespace XROfficeFiles.OfficeFiles.Values
{
    [Serializable]
    public class SpreadSheet
    {
        public long sheetId;
        public List<TableColumnWidth> columnWidths;
        public List<TableRowHeight> rowHeights;
        public List<SpreadSheetCell> cells;
    }
}