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
    public class AudioFetcherForvo : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "Audio";

        protected override ColumnName ColName => ColumnName.AUDIO;

        public override string GetResult(string word)
        {
            var result = GetWordAsync(word);
            var wordResult = result.Result;

            return wordResult;
        }

        private async Task<string> GetWordAsync(string word)
        {
            word = word.Replace(" ", "-").ToLower();

            string result = "";

            WebClient webClient = new WebClient();
            var filePathResult = GetFilePathAsync(word);

            string filePath = filePathResult.Result;

            if (filePath != String.Empty)
            {
                var fileUrl = "https://forvo.com/player-mp3Handler.php?path=" + filePath;

                WebRequest request = WebRequest.Create(new Uri(fileUrl));
                request.Method = "HEAD";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    if (response.ContentLength > 0)
                    {
                        webClient.DownloadFile(fileUrl, AppSettings.Instance.PathToAudio + "\\" + word + ".mp3");
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

            var url = "https://forvo.com/search/" + word + "/en_usa/";
            var xPathQuery = "//li[@class='list-words']//ul//li//span";

            try
            {
                var response = await client.GetStringAsync(url);
                htmlDoc.LoadHtml(response);
                var source = htmlDoc.DocumentNode.SelectNodes(xPathQuery);

                if (source == null) //try all accent
                {
                    url = "https://forvo.com/search/" + word + "/en_uk/";
                    response = await client.GetStringAsync(url);
                    htmlDoc.LoadHtml(response);
                    source = htmlDoc.DocumentNode.SelectNodes(xPathQuery);
                }

                if (source != null)
                {
                    var span = source[0];
                    var text = span.InnerText.Replace(" pronunciation", "");
                    text = span.InnerText.Replace(" ", "-");
                    text = text.Replace("-pronunciation", "");
                    text = text.ToLower();
                    if (text == word)
                    {
                        var onclickTxt = span.Attributes["onclick"].Value;
                        var tmpsplit = onclickTxt.Split('\'');
                        result = tmpsplit[1];
                    }
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
