using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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



        private Timer expandTimer = new Timer();
        private Timer collapseTimer = new Timer();
        public FormMenu()
        {
            InitializeComponent();
            
            /*animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);*/
        }
        private void guna2Panel1_MouseHover(object sender, EventArgs e)
        {
            collapseTimer.Stop();
            
            expandTimer.Start();
        }

        private void guna2Panel1_MouseLeave(object sender, EventArgs e)
        {
            expandTimer.Stop();
            
            collapseTimer.Start();
        }
        private void FormMenu_Load(object sender, EventArgs e)
        {
            collapseTimer.Interval = 10; // задержка в миллисекундах
            collapseTimer.Tick += CollapsePanel;
            expandTimer.Interval = 10; // задержка в миллисекундах
            expandTimer.Tick += ExpandPanel;
        }
        private void ExpandPanel(object sender, EventArgs e)
        {

            if (guna2Panel1.Width < 330)
            {
                
                guna2Panel1.Width += 15;
                guna2Panel1.Refresh();
            }
            else
            {
                guna2Button1.Width = guna2Panel1.Width;
                guna2Button1.Text = "Главная";
                guna2Button1.Image = null;
                guna2Button2.Width = guna2Panel1.Width;
                guna2Button2.Text = "Профиль";
                guna2Button2.Image = null;
                guna2Button3.Width = guna2Panel1.Width;
                guna2Button3.Text = "Назад";
                guna2Button3.Image = null;
                expandTimer.Stop();
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private void CollapsePanel(object sender, EventArgs e)
        {

            if (guna2Panel1.Width > 50)
            {
                guna2Panel1.Width -= 15;
                guna2Panel1.Refresh();
            }
            else
            {
                guna2Button1.Width = guna2Panel1.Width;
                guna2Button1.Text = "";
                guna2Button1.Image = Properties.Resources.call; 
                guna2Button2.Width = guna2Panel1.Width;
                guna2Button2.Text = "";
                guna2Button2.Image = Properties.Resources.email;
                guna2Button3.Width = guna2Panel1.Width;
                guna2Button3.Text = "";
                guna2Button3.Image = Properties.Resources.back_button;
                collapseTimer.Stop();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
