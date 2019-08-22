using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class LinkFetcherWordHunt : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        protected override ColumnName ColName => ColumnName.WORD_HUNT;

        public override string GetResult(string word)
        {
            return "https://wooordhunt.ru/word/" + word;
        }
    }
}
