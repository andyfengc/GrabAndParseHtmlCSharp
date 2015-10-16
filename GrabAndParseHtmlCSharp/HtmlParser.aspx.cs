using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fizzler.Systems.HtmlAgilityPack;

namespace GrabAndParseHtmlCSharp
{
    public partial class HtmlParser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtWebsite.Text = "http://www.speedy.ca/en/status.asp";
            txtParam.Text = "3229000";
        }

        protected void btpParse_Click(object sender, EventArgs e)
        {
            String website = txtWebsite.Text;
            String param = txtParam.Text;
            String htmlData = GrabHtmlData(website, param);
            if (htmlData != null)
            {
                lblGrabResult.Text = "Size of html data: " + htmlData.Length + " characters";
                List<EventLog> logs = ParseData(htmlData);
                logs.ForEach(log => lblParseResult.Text = lblParseResult.Text + log + "<br>");
            }
            else
            {
                lblGrabResult.Text = "Failed to grab html data";
            }
        }

        private string GrabHtmlData(string website, string param)
        {
            // create http request
            HttpWebRequest request = WebRequest.CreateHttp(website) as HttpWebRequest;
            // set post
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded";
            // add post data
            string postData = "pro=" + param;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }
            // get response
            var response = request.GetResponse() as HttpWebResponse;
            using (var stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            return null;
        }
        private List<EventLog> ParseData(string htmlData)
        {
            var logs = new List<EventLog>();
            // load data
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(htmlData);
            // find the log table
            var resultNode = document.DocumentNode.QuerySelector(".ServicesResults .textTracing");
            // extract data
                String date = null;
                String time = null;
                String status = null;
                String location = null;
            foreach (var rowNode in resultNode.Descendants("tr").Skip(2))
            {
                var colNodes = rowNode.Descendants("td");
                if (colNodes.Count() != 4)
                {
                    throw new InvalidOperationException("The website modify the format result results");
                }
                var colList = colNodes.ToList();
                if (colList[0].InnerText != "&nbsp;")
                    date = colList[0].InnerText;
                time =  colList[1].InnerText;
                status =  colList[2].InnerText;
                location = colList[3].InnerText;
                var log = new EventLog()
                {
                    EventDate = DateTime.Now,
                    EventName = status,
                    EventLocation = location
                };
                logs.Add(log);
            }
            return logs;
        }
    }
    public class EventLog
    {
        public DateTime EventDate;
        public String EventName;
        public String EventLocation;
        public override string ToString()
        {
            return EventDate + " " + EventName + " " + EventLocation;
        }
    }
}