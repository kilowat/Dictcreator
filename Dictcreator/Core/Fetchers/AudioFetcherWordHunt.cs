using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class AudioFetcherWordHunt : DataFetcher
    {
        private string _urlPath = "https://wooordhunt.ru/data/sound/word/us/mp3/";

        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "wooordhunt.ru";

        protected override ColumnName ColName => ColumnName.AUDIO;
        public override string GetResult(string word)
        {
            var result = GetResultAsync(word);
            var wordResult = result.Result;

            return wordResult;
        }

        private async Task<string> GetResultAsync(string word)
        {
            word = word.Replace(" ", "-").ToLower();

            string result = "";

            WebRequest request = WebRequest.Create(new Uri(_urlPath + word + ".mp3"));
            request.Method = "HEAD";

            using (WebResponse response = await request.GetResponseAsync())
            {
                if (response.ContentLength > 0)
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(_urlPath + word + ".mp3", AppSettings.Instance.PathToAudio +"\\" + word + ".mp3");
                    result = AppSettings.Instance.AudioDirName + "\\" + word + ".mp3";
                }
            }

            return result;
        }
    }
}
