using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class TranscriptionFetcherTopPhonetics : DataFetcher
    {
        private string _siteUrl = "https://tophonetics.com/";
        private string _xPathQuery = "//span[@class='transcribed_word']//text()";

        public override CellType CellExlType => CellType.STRING;

        protected override ColumnName ColName => ColumnName.TRANSCRIPTION;

        public override string GetResult(string word)
        {
            var result = GetResultAsync(word);
            return result.Result;
        }

        private async Task<string> GetResultAsync(string word)
        {
            var request = HttpWebRequest.Create(_siteUrl);

            var postData = "text_to_transcribe=" + word + "&submit=Show transcription&output_dialec=am&output_style=only_tr&output_dialect=am";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseString);

            var transcription = htmlDoc.DocumentNode.SelectSingleNode(_xPathQuery);

            if (transcription != null)
                return transcription.OuterHtml;
            else
                return "";
        }
    }
}
