using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            String htmlData = GrabHtmlDataByWebRequest(website, param);
            if (htmlData != null)
            {
                lblGrabResult.Text = "Size of html data: " + htmlData.Length + " characters";
                List<EventLog> logs = ParseData(htmlData);
                lblParseResult.Text = "Count of items: " + logs.Count;
                // bind results to gridview
                gvParseResult.DataSource = logs;
                gvParseResult.DataBind();
            }
            else
            {
                lblGrabResult.Text = "Failed to grab html data";
            }
        }

        // way 1, get html data using WebRequest
        private string GrabHtmlDataByWebRequest(string website, string param)
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

        // way 2, get html data using WebClient
        private string GrabHtmlDatabyWebClient(string website, string param)
        {
            using (var client = new WebClient())
            {
                // create post parameters
                var values = new NameValueCollection();
                values["pro"] = param;
                // get response
                var response = client.UploadValues(website, values);
                return Encoding.Default.GetString(response);
            }
        }

        // parse data using Html Agility Pack
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
                time = colList[1].InnerText;
                status = colList[2].InnerText;
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
        public DateTime EventDate { get; set; }
        public String EventName { get; set; }
        public String EventLocation { get; set; }
        public override string ToString()
        {
            return EventDate + " " + EventName + " " + EventLocation;
        }
    }
}