using System;
using System.IO;
using System.Net;
using AngleSharp.Parser.Html;

namespace TeseraGamesLoader.Repositories
{
    internal abstract class BaseRepository
    {
        protected object _parserLock = new object();
        protected HtmlParser _parser;

        protected BaseRepository()
        {
            _parser = new HtmlParser();
        }

        protected string GetData(string connectionString)
        {
            var request = WebRequest.Create(connectionString);
            var response = request.GetResponse();
            var data = response.GetResponseStream();
            var html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            return html;
        }
    }
}