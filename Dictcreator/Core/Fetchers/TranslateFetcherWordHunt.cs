using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class TranslateFetcherWordHunt :DataFetcher
    {
        private string _siteUrl = "https://wooordhunt.ru/word/";
        private string _xPathQuery = "//span[@class='t_inline_en']//text()";

        public override string ServiceName => "WordHunt";

        public override CellType CellExlType => CellType.STRING;

        protected override ColumnName ColName => ColumnName.TRANSLATE;

        public override string GetResult(string word)
        {
            var result = GetResultAsync(word);
            return result.Result;
        }

        private async Task<string> GetResultAsync(string word)
        {
            word = word.ToLower();

            string result = "";
            var htmlDoc = new HtmlDocument();

            var client = new HttpClient();

            try
            {
                var response = await client.GetStringAsync(_siteUrl + word);
                htmlDoc.LoadHtml(response);
                var transcription = htmlDoc.DocumentNode.SelectSingleNode(_xPathQuery);

                if (transcription != null && transcription.OuterHtml != null)
                {
                    result = transcription.OuterHtml;
                    result = result.Trim();
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
