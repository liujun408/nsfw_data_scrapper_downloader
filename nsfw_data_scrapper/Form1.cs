using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nsfw_data_scrapper
{
    public partial class Form1 : Form
    {
        int mutiThreds = 100;
        int thredsCount = 0;
        public List<imgUrl> urlList = new List<imgUrl>();

        int downNum = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void getPicFromTxt(string type)
        {


            log("start download:" + type);

            var path = "raw_data/" + type + "/urls_" + type + ".txt";
            var saveFolder = "pics/" + type + "/";

            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                var urls = line.ToString();
                string url = "", fileName = "";
                var arr = urls.Replace("http://", "@").Split('@');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != "")
                    {
                        url = "http://" + arr[i];
                        fileName = url.Substring(url.LastIndexOf('/') + 1, url.Length - url.LastIndexOf('/') - 1);
                        //if (!File.Exists(saveFolder + fileName))
                        //{
                        //    DoGetImage(url, saveFolder + fileName);
                        //}
                        imgUrl img = new imgUrl();
                        img.url = url;
                        img.path = saveFolder + fileName;
                        if (!File.Exists(img.path))
                        {
                            urlList.Add(img);
                        }
                        

                    }
                }



            }
            log("url load success,total:" + urlList.Count);

            timer1.Enabled = true;
        }

        public class imgUrl
        {
            public string url { get; set; }
            public string path { get; set; }
        }

        public void DoGetImage()
        {
            var url = urlList[0].url;
            var path = urlList[0].path;
            urlList.RemoveAt(0);

            if (File.Exists(path))
            {
                downNum++;
                return;
            }

            thredsCount++;


            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.KeepAlive = true;

                req.ContentType = "image/png";
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                System.IO.Stream stream = null;

                try
                {
                    // 以字符流的方式读取HTTP响应
                    stream = rsp.GetResponseStream();
                    Image.FromStream(stream).Save(path);
                }
                finally
                {
                    // 释放资源
                    if (stream != null) stream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            catch (Exception err)
            {
                //log("error url:" + url);
            }
            thredsCount--;
            downNum++;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            getPicFromTxt("drawings");
            timer2.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (thredsCount < mutiThreds && urlList.Count > 0)
            {
                System.Threading.Thread t = new System.Threading.Thread(DoGetImage);
                t.Start();

            }

            if (urlList.Count == 0)
            {
                timer1.Enabled = false;
            }
        }

        public void log(string s)
        {
            textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + s + "\r\n");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            log("progress:" + downNum + "/" + urlList.Count + ",threds:" + thredsCount);
            if (urlList.Count == 0)
            {
                log("all pics done");

                timer2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getPicFromTxt("hentai");
            timer2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            getPicFromTxt("neutral");
            timer2.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getPicFromTxt("porn");
            timer2.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            getPicFromTxt("sexy");
            timer2.Enabled = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            getPicFromTxt("drawings");
            getPicFromTxt("hentai");
            getPicFromTxt("neutral");
            getPicFromTxt("porn");
            getPicFromTxt("sexy");
            timer2.Enabled = true;
        }
    }
}
