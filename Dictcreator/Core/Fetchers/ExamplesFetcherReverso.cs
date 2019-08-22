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
    public class ExamplesFetcherReverso : DataFetcher
    {
        private string _siteUrl = "https://context.reverso.net/translation/english-russian/";
        private string _xPathQueryList = "//div[@class='example']";
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

                    foreach (var item in translateList)
                    {
                        if (count > AppSettings.Instance.MaxExample) break;

                        examplesList.Add(Regex.Replace(item.InnerText, "\n", "").Trim());

                        count++;
                    }

                    result = string.Join("\n", examplesList);
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
