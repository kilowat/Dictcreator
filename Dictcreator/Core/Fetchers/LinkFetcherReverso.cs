using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class LinkFetcherReverso : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "Reverso";

        protected override ColumnName ColName => ColumnName.REVERSO;

        public override string GetResult(string word)
        {
            return "https://dictionary.reverso.net/english-russian/" + word;
        }
    }
}
