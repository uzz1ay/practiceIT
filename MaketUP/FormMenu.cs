using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    public partial class FormMenu : Form
    {
        private AnimatedButton animatedButton1, animatedButton3, animatedButton2;

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            guna2Panel2.Visible = true;
            guna2Panel3.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            guna2Panel2.Visible = false;
            guna2Panel3.Visible = true;
        }

        private void guna2Panel1_MouseHover(object sender, EventArgs e)
        {
            guna2HtmlLabel1.Visible = true;
            guna2HtmlLabel2.Visible = true;
            guna2HtmlLabel3.Visible = true;
            guna2HtmlLabel4.Visible = true;
            guna2HtmlLabel5.Visible = true;
            guna2HtmlLabel6.Visible = true;
        }

        private void guna2Panel1_MouseLeave(object sender, EventArgs e)
        {
            guna2HtmlLabel1.Visible = false;
            guna2HtmlLabel2.Visible = false;
            guna2HtmlLabel3.Visible = false;
            guna2HtmlLabel4.Visible = false;
            guna2HtmlLabel5.Visible = false;
            guna2HtmlLabel6.Visible = false;
        }

        public FormMenu()
        {
            InitializeComponent();

            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        
    }
}
