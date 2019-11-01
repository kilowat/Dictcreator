using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class AudioFethcerVocabulary : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "vocabulary.com";

        protected override ColumnName ColName => ColumnName.AUDIO;

        public override string GetResult(string word)
        {
            var result = GetWordAsync(word);
            var wordResult = result.Result;

            return wordResult;
        }
        private async Task<string> GetWordAsync(string word)
        {
            string result = "";

            WebClient webClient = new WebClient();
            var filePathResult = GetFilePathAsync(word);

            string fileId = filePathResult.Result;
            string filePath = "https://audio.vocab.com/1.0/us/"+ fileId +".mp3";

            if (fileId != String.Empty)
            {
                WebRequest request = WebRequest.Create(new Uri(filePath));
                request.Method = "HEAD";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    if (response.ContentLength > 0)
                    {
                        word = word.Replace(" ", "-").ToLower();
                        webClient.DownloadFile(filePath, AppSettings.Instance.PathToAudio + "\\" + word + ".mp3");
                        result = AppSettings.Instance.AudioDirName + "\\" + word + ".mp3";
                    }
                }
            }

            return result;
        }

        private async Task<string> GetFilePathAsync(string word)
        {
            string result = "";
            var htmlDoc = new HtmlDocument();

            var client = new HttpClient();

            int count = 1;

            var url = "https://www.vocabulary.com/dictionary/";
            var xPathQuery = "//a[@class='audio']";
            var xPathQueryCheck = "//h1";

            try
            {
                var response = await client.GetStringAsync(url + word);
                htmlDoc.LoadHtml(response);

                var checkWord = htmlDoc.DocumentNode.SelectSingleNode(xPathQueryCheck)?.InnerText;

                if (checkWord == null) return result;

                var checkWordWithDash = checkWord.Replace(" ", "-");

                var source = htmlDoc.DocumentNode.SelectSingleNode(xPathQuery);

                if ((checkWord == word || checkWordWithDash == word) && source != null && source.OuterHtml != null)
                {
                    result = source.Attributes["data-audio"].Value;
                }
                else
                {
                    return result;
                }

            }
            catch (HttpRequestException e)
            {
                return result;
            }

            return result;
        }
    }
}
