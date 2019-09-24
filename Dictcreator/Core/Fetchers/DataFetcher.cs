using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public abstract class DataFetcher
    {
        public abstract string ServiceName { get; }
        public abstract CellType CellExlType { get; }
        protected abstract ColumnName ColName { get; }

        public int ColIndex
        {
            get { return AppSettings.Instance.ColumnNameIndexMap[ColName]; }
         }
        public abstract string  GetResult(string word);
    }

    public enum CellType
    {
        STRING,
        LINK,
        EMPTY
    }
}
