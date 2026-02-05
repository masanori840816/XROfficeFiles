using System;
using System.Collections.Generic;

namespace XROfficeFiles.OfficeFiles.Values
{
    [Serializable]
    public class TableColumnWidth
    {
        public long id;
        public int column;
        public double width;
    }
}