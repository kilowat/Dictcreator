using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using HtmlAgilityPack;
using System.Net.Http;

namespace Dictcreator.Core.Fetchers
{
    public class PictureFetcher : DataFetcher
    {
        private string _siteUrl = "https://www.shutterstock.com/ru/search/";
        private string _xPathQuery= "//img[@class='z_g_h']";
        public override string ServiceName => "pexels picture";

        public override CellType CellExlType => CellType.EMPTY;

        protected override ColumnName ColName => ColumnName.EMPTY;

        public override string GetResult(string word)
        {
            if (!AppSettings.Instance.DownloadPicture)
            {
                return "disabled";
            }

            var result = GetResultAsync(word);

            if (result.Result == null || result.Result == string.Empty)
            {
                return "not find";
            }
            
            var img = DownloadRemoteImageFile(result.Result);

            using (var ms = new MemoryStream(img))
            {
                var original = Image.FromStream(ms);
                var newHeight = AppSettings.Instance.PicSize;
                var newWidth = ScaleWidth(original.Height, AppSettings.Instance.PicSize, original.Width);

                using (var newPic = new Bitmap(newWidth, newHeight))
                using (var gr = Graphics.FromImage(newPic))
                {
                    gr.DrawImage(original, 0, 0, newWidth, newHeight);
                    newPic.Save(AppSettings.Instance.PathToPicture + "\\" + word + ".jpeg", ImageFormat.Jpeg);
                }
            }

            return "done";
        }

        private async Task<string> GetResultAsync(string word)
        {
            string result = "";

            var htmlDoc = new HtmlDocument();

            var client = new HttpClient();

            try
            {
                var response = await client.GetStringAsync(_siteUrl + word);
                htmlDoc.LoadHtml(response);
                var imageNodes = htmlDoc.DocumentNode.SelectSingleNode(_xPathQuery);
                if (imageNodes != null)
                {
                    result = imageNodes.Attributes["src"].Value;
                }
            }
            catch (HttpRequestException e)
            {
                //404 nothing do
            }

            return result;
        }

        private  byte[] DownloadRemoteImageFile(string uri)
        {
            byte[] content;
            var request = (HttpWebRequest)WebRequest.Create(uri);

            using (var response = request.GetResponse())
            using (var reader = new BinaryReader(response.GetResponseStream()))
            {
                content = reader.ReadBytes(100000);
            }

            return content;
        }
        private  int ScaleWidth(int originalHeight, int newHeight, int originalWidth)
        {
            var scale = Convert.ToDouble(newHeight) / Convert.ToDouble(originalHeight);

            return Convert.ToInt32(originalWidth * scale);
        }
    }
}
