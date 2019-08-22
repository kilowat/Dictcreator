using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class LinkFetcherMerriemWebster : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        protected override ColumnName ColName => ColumnName.MERRIAM_WEBSTER;

        public override string GetResult(string word)
        {
            return "https://www.merriam-webster.com/dictionary/" + word;
        }
    }
}
