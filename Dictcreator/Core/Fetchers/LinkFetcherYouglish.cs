using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class LinkFetcherYouglish : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "Youglish";

        protected override ColumnName ColName => ColumnName.YOUGLISH;

        public override string GetResult(string word)
        {
            return "https://youglish.com/search/" + word + "/us";
        }
    }
}
