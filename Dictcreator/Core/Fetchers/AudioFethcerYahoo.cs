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
    public class AudioFethcerYahoo : DataFetcher
    {
        public override CellType CellExlType => CellType.LINK;

        public override string ServiceName => "yahoo.com";

        protected override ColumnName ColName => ColumnName.AUDIO;

        private string url = "https://s.yimg.com/bg/dict/dreye/live/f/";

        public override string GetResult(string word)
        {
            var result = GetWordAsync(word);
            var wordResult = result.Result;

            return wordResult;
        }
        private async Task<string> GetWordAsync(string word)
        {
            string result = "";
            string fileId = word.Replace(' ', '_') + ".mp3";
            string filePath = url + fileId;
            WebClient webClient = new WebClient();

            WebRequest request = WebRequest.Create(new Uri(filePath));
            request.Method = "HEAD";
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    if (response.ContentLength > 0)
                    {
                        word = word.Replace(" ", "-").ToLower();
                        webClient.DownloadFile(filePath, AppSettings.Instance.PathToAudio + "\\" + word + ".mp3");
                        result = AppSettings.Instance.AudioDirName + "\\" + word + ".mp3";
                    }
                }
            } catch (Exception ex)
            {
                return "";
            }
            
            return result;
        }
    }
}
