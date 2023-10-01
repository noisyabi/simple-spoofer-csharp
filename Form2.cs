using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyAuth;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace ud
{


    public partial class Form2 : Form
    {


        public static api KeyAuthApp = new api(
    name: "Hans",
    ownerid: "huoT27jcK2",
    secret: "411cb5c6510c6057a043a5079ba6858c197f88786b1b8b2cc2a0ca551bcd09e6",
    version: "1.0"
);
        private ListBox debugListBox;
        public Form2()
        {
            InitializeComponent();
        }

        public void ShowMessage(string message, int delaySeconds)
        {
            
            debugListBox.Items.Add(message);
            debugListBox.SelectedIndex = debugListBox.Items.Count - 1;

            
            Thread.Sleep(delaySeconds * 1000);
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            KeyAuthApp.init();
            KeyAuthApp.license(guna2TextBox1.Text);
            if (KeyAuthApp.response.success)
            {
                Form1 main = new Form1();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Wrong key retard");

        }
    }
}