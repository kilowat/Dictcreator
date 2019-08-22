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

        protected override ColumnName ColName => ColumnName.REVERSO;

        public override string GetResult(string word)
        {
            return "https://context.reverso.net/%D0%BF%D0%B5%D1%80%D0%B5%D0%B2%D0%BE%D0%B4/%D0%B0%D0%BD%D0%B3%D0%BB%D0%B8%D0%B9%D1%81%D0%BA%D0%B8%D0%B9-%D1%80%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9/" + word;
        }
    }
}
