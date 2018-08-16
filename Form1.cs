//Download From
//Visual C# Kicks - http://www.vcskicks.com
//Edited by Jumbofile (Gregory Plachno)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace URLChecker
{
    public partial class Form1 : MaterialForm

    {
        public Form1()
        {
            InitializeComponent();
            txtURL.Visible = false;
            //Define theme
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepPurple800, Primary.DeepPurple900, Primary.DeepPurple500, Accent.Purple200, TextShade.WHITE);
        }

        private bool CheckURL(string url)
        {
            //Update UI
           // lStatus.Text = "Checking URL...";
           // lStatus.ForeColor = Color.Black;
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            //This is part 2 of crash prevent. Sets to nothing, if its nothing just ignore
            if (url.Equals(""))
            {
                return true;
            }

            Uri urlCheck = new Uri(url);

            WebRequest request = WebRequest.Create(urlCheck);
            request.Timeout = 15000;
            WebResponse response;

            try
            {
                //get URL web response
                response = request.GetResponse();
            }
            catch (Exception e)
            {
                this.Cursor = Cursors.Default;
                Console.WriteLine(url + " : " + e);
                if (e.Message.Contains("Server Unavailable") || e.Message.Contains("Forbidden") || e.Message.Contains("timed out") || e.Message.Contains("connection was closed")) 
                {
                    return false; //url doesnt exist or has 503/403 errors
                }
                else
                {
                    return true; 
                }
            }

            string responseURL = response.ResponseUri.ToString();

            //Update UI
            this.Cursor = Cursors.Default;
            Application.DoEvents();
            Console.WriteLine(responseURL);
            Console.WriteLine(responseURL.Length);
            if (responseURL.Equals(urlCheck.ToString()))
            { //it was redirected, check to see if redirected to error page
                Console.WriteLine("YES" + responseURL);
                return true; //everything okay
               
            }
            else
            {
                //Check for 404/500 errors
                Console.WriteLine(responseURL);
                if (responseURL.IndexOf("404.php") > -1 ||
                        responseURL.IndexOf("500.php") > -1 ||
                        responseURL.IndexOf("404.htm") > -1 ||
                        responseURL.IndexOf("500.htm") > -1)
                {
                    //if(responseURL.)
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //Starts URL checking
        private void btnURL_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            for (int i = 0; i < richTextBox1.Lines.Length; i++)
            {
                txtURL.Text = richTextBox1.Lines[i];

                //HTTPS caused a false negative, get rid of https
                if (txtURL.Text.StartsWith("https"))
                {
                    string temp = txtURL.Text.Substring(8);
                    txtURL.Text = temp;
                    Console.WriteLine(txtURL.Text);
                }

                //add in http to make a valid uri
                if (!txtURL.Text.StartsWith("http://"))
                    txtURL.Text = "http://" + txtURL.Text;

                //Bad url check, doesnt completely work
                if (txtURL.Text == "http://" || txtURL.Text == string.Empty) return; //No URL entered

                //space means not a url, make it blank to prevent crash
                if (txtURL.Text.Contains(" "))
                {
                    txtURL.Text = "";
                }

                if (CheckURL(txtURL.Text))
                {
                    //lStatus.Text = txtURL.Text + " Exists";
                   // lStatus.ForeColor = Color.Green;
                }
                else
                {
                    //lStatus.Text = txtURL.Text + Environment.NewLine + " Was Not Found";
                    //lStatus.ForeColor = Color.Red;
                    richTextBox2.AppendText(richTextBox1.Lines[i] + Environment.NewLine);
                }
            }
        }

        private void txtURL_Leave(object sender, EventArgs e)
        {
            if (!txtURL.Text.StartsWith("http://"))
                txtURL.Text = "http://" + txtURL.Text;
        }

        private void lCheck_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lStatus.Text != string.Empty && lStatus.Text.IndexOf("http://") != -1)
                System.Diagnostics.Process.Start(lStatus.Text.Substring(0, lStatus.Text.IndexOf(' ')));
        }

        private void txtURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
                btnURL.PerformClick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}