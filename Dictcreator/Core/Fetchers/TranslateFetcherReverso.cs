﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dictcreator.Core.Fetchers
{
    public class TranslateFetcherReverso : DataFetcher
    {
        private string _siteUrl = "https://context.reverso.net/translation/english-russian/";
        private string _xPathQuery = "//div[@id='translations-content']//a";
        protected override ColumnName ColName => ColumnName.TRANSLATE;

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
  
            try
            {
                var response = await client.GetStringAsync(_siteUrl + word);
                htmlDoc.LoadHtml(response);
                HtmlAgilityPack.HtmlNodeCollection translateList = htmlDoc.DocumentNode.SelectNodes(_xPathQuery);

                if (translateList != null && translateList.Count > 0)
                {
                    List<string> translateWordList = new List<string>();

                    foreach (var item in translateList)
                    {
                        translateWordList.Add(Regex.Replace(item.InnerText, "\n", "").Trim());
                    }

                    result = string.Join(", ", translateWordList);
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
