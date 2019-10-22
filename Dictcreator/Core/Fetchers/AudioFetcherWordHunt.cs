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

        public override string ServiceName => "Audio";

        protected override ColumnName ColName => ColumnName.AUDIO;

        public override string GetResult(string word)
        {
            var result = GetResultAsync(word);
            var wordResult = result.Result;

            if (wordResult == String.Empty) // Попробуем поискать слово на другом сайте
            {
                var resultForvo = GetWordOnForvoAsync(word);
                 wordResult = resultForvo.Result;
            }

            if (wordResult == String.Empty) // Попробуем поискать слово на другом сайте
            {
                var resultTuReng = GetWordOnTuRengAsync(word);
                wordResult = resultTuReng.Result;
            }

            return wordResult;
        }

        private async Task<string> GetWordOnForvoAsync(string word)
        {
            word = word.Replace(" ", "-").ToLower();

            string result = "";

            WebClient webClient = new WebClient();
            var filePathResult = GetFilePathOnForvoAsync(word);

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

        private async Task<string> GetFilePathOnForvoAsync(string word)
        { 
            string result = "";
            var htmlDoc = new HtmlDocument();

            var client = new HttpClient();

            int count = 1;

            var url = "https://forvo.com/search/"+word+"/en_usa/";
            var xPathQuery = "//li[@class='list-words']//ul//li//span";

            try
            {
                var response = await client.GetStringAsync(url);
                htmlDoc.LoadHtml(response);
                var source = htmlDoc.DocumentNode.SelectNodes(xPathQuery);
                
                if (source != null)
                {
                    var span = source[0];
                    var text = span.InnerText.Replace(" pronunciation", "");
                    text = span.InnerText.Replace(" ", "-");
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

        private async Task<string> GetWordOnTuRengAsync(string word)
        {
            word = word.Replace(" ", "-").ToLower();

            string result = "";

            WebClient webClient = new WebClient();
            var filePathResult = GetFilePathOnTuRenAsync(word);

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

        private async Task<string> GetFilePathOnTuRenAsync(string word)
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
