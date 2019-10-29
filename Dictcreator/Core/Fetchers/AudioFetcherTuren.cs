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
    public class AudioFetcherTuren : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "tureng.com";

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
                var fileUrl = "https:" + filePath;

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

            var url = "https://tureng.com/en/turkish-english/";
            var xPathQuery = "//audio[@id='turengVoiceENTRENus']//source";

            try
            {
                var response = await client.GetStringAsync(url + word);
                htmlDoc.LoadHtml(response);
                var source = htmlDoc.DocumentNode.SelectSingleNode(xPathQuery);

                if (source != null && source.OuterHtml != null)
                {
                    result = source.Attributes["src"].Value;
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
