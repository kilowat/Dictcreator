using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
   public class ExamplesFetcherWoordHunt : DataFetcher
    {
        private string _siteUrl = "https://wooordhunt.ru/word/";
        private string _xPathQueryList = "//div[@class='block']//p";

        public override CellType CellExlType => CellType.STRING;

        public override string ServiceName => "WordHunt";

        protected override ColumnName ColName => ColumnName.EXAMPLES;

        public override string GetResult(string word)
        {
            var result = GetResultAsync(word);
            return result.Result;
        }

        private async Task<string> GetResultAsync(string word)
        {
            string result = "";
            var htmlDoc = new HtmlDocument();

            var client = new HttpClient();

            int count = 1;

            try
            {
                var response = await client.GetStringAsync(_siteUrl + word);
                htmlDoc.LoadHtml(response);
                HtmlAgilityPack.HtmlNodeCollection translateList = htmlDoc.DocumentNode.SelectNodes(_xPathQueryList);

                if (translateList != null && translateList.Count > 0)
                {
                    List<string> examplesList = new List<string>();
                    var exampleText = "";

                    foreach (var item in translateList)
                    {
                        if (count > AppSettings.Instance.MaxExample) break;
                        //" You can go now if you like.&ensp;  Если хотите, можете идти.&ensp;&#9776; "
                        exampleText += item.InnerText;
                        count++;
                    }
                    exampleText = Regex.Replace(exampleText, "&ensp;", "");
                    exampleText = Regex.Replace(exampleText, "&#9776;", "");
                    result = exampleText;
                }
            }
            catch (HttpRequestException e)
            {
                //404 nothing do
            }

            return result;
        }
    }
}
