using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Fiddler;

namespace Facebook_Conversation_Pictures_Downloader
{
    public partial class Form1 : Form
    {
        private const string SharedPhotosPath = "/ajax/graphql/query/?__pc=EXP1";
        private const int NumberOfPicturesToReturn = 100000;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.Startup(8888, FiddlerCoreStartupFlags.Default);

            webBrowser1.ScriptErrorsSuppressed = true;
            WebProxy myProxy = new WebProxy();
            Uri newUri = new Uri("http://localhost:8888");
            myProxy.Address = newUri;
        }

        void FiddlerApplication_AfterSessionComplete(Session oSession)
        {
            if (oSession.fullUrl.Contains(SharedPhotosPath))
            {
                var response = oSession.GetResponseBodyAsString();
                var urls = new HashSet<string>();

                var regex = new Regex(@"image1.+?(https.+?)\\""");
                var matches = regex.Matches(response);
                foreach (Match match in matches)
                {
                    var url = match.Groups[1].Value;
                    url = url.Replace("\\\\\\", "");
                    urls.Add(url);
                }

                var form2 = new Form2(urls);
                form2.ShowDialog();
            }
        }

        void FiddlerApplication_BeforeRequest(Session oSession)
        {
            if (oSession.fullUrl.Contains(SharedPhotosPath))
            {
                var requestBody = oSession.GetRequestBodyAsString();

                var regex = new Regex(@"params\[first\]=\d");
                requestBody = regex.Replace(requestBody, $"params[first]={NumberOfPicturesToReturn}");

                oSession.utilSetRequestBody(requestBody);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FiddlerApplication.Shutdown();
        }
    }
}
