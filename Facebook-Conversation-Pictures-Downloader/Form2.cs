using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Facebook_Conversation_Pictures_Downloader
{
    public partial class Form2 : Form
    {
        private ICollection<string> urls;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(ICollection<string> urls)
        {
            InitializeComponent();
            this.urls = urls;
            this.label1.Text = $"Found {urls.Count} pictures";
            this.progressBar1.Maximum = this.urls.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedPath = "";
            var t = new Thread(() =>
            {
                var fbd = new FolderBrowserDialog
                {
                    RootFolder = System.Environment.SpecialFolder.Desktop,
                    ShowNewFolderButton = true
                };
                var result = fbd.ShowDialog();
                if (result == DialogResult.Cancel)
                    return;

                selectedPath = fbd.SelectedPath;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            this.textBox1.Text = selectedPath;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Path.GetFullPath(this.textBox1.Text);
                this.button1.BackColor = Color.LawnGreen;
                this.button2.Enabled = true;
            }
            catch (Exception ex)
            {
                this.button1.BackColor = Color.Red;
                this.button2.Enabled = false;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;

            Directory.CreateDirectory(this.textBox1.Text);
            var directoryToSave = $"{this.textBox1.Text}{Path.DirectorySeparatorChar}";

            var downloader = new Downloader();
            downloader.FileDownloadedEvent += ClientOnDownloadFileCompleted;
            await downloader.DownloadMultipleFilesAsync(this.urls, directoryToSave);

            MessageBox.Show("All files downloaded successfully!", "Success", MessageBoxButtons.OK);
            this.Close();
        }

        public void ClientOnDownloadFileCompleted()
        {
            this.progressBar1.Value += 1;
            this.label2.Text = $"{this.progressBar1.Value} pictures downloaded";
        }
    }
}
