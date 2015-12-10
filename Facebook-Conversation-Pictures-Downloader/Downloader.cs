using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Facebook_Conversation_Pictures_Downloader
{
    public class Downloader
    {
        public delegate void EventHandler();
        public event EventHandler FileDownloadedEvent;
        private List<string> errs = new List<string>();
        private async Task DownloadFileAsync(string url, string directory)
        {
            try
            {
                var fileName = Regex.Match(url, @".+/(.+\.jpg)").Groups[1].Value;

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(url), directory + fileName);
                    FileDownloadedEvent?.Invoke();
                }
            }
            catch (Exception ex)
            {
                this.errs.Add(ex.InnerException.Message);
                MessageBox.Show("Failed to download file: " + url, "Error", MessageBoxButtons.OK);
            }
        }

        public async Task DownloadMultipleFilesAsync(IEnumerable<string> urls, string directory)
        {
            await Task.WhenAll(urls.Select(url => DownloadFileAsync(url, directory)));
        }
    }
}
