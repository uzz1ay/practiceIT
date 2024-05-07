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
    public partial class FormCaptcha : Form
    {
        private AnimatedButton animatedButton1, animatedButton2;
        public FormCaptcha()
        {
            InitializeComponent();
            GenerateCaptcha();
            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedButton(button4, Color.FromArgb(130, 6, 255), Color.White);
        }
        private string captchaText;
        private PictureBox captchaImage;
        private void GenerateCaptcha()
        {
            captchaText = GenerateCaptchaText();
            captchaImage = new PictureBox();
            captchaImage.Size = new Size(350, 150);
            captchaImage.Location = new Point(10, 10);
            captchaImage.Image = GenerateCaptchaImage(captchaText);
            this.Controls.Add(captchaImage);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMenu form = new FormMenu();
            Closed += (s, args) => this.Close();
            form.Show();
        }
        private string GenerateCaptchaText()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string captcha = "";
            for (int i = 0; i < 6; i++)
            {
                captcha += chars[random.Next(chars.Length)];
            }
            return captcha;
        }
        private Bitmap GenerateCaptchaImage(string captchaText)
        {
            Bitmap bitmap = new Bitmap(350, 150);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            // Отрисовка текста капчи
            graphics.DrawString(captchaText, new Font("Arial", 50), Brushes.Black, new PointF(10, 10));

            // Добавление шумов
            for (int i = 0; i < 100; i++)
            {
                int x = new Random().Next(bitmap.Width);
                int y = new Random().Next(bitmap.Height);
                bitmap.SetPixel(x, y, Color.Gray);
            }

            return bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(guna2TextBox1.Text == captchaText)
            {
                MessageBox.Show("Капча пройдена!");
                ClassStorage.capctha = true;
                this.Hide();
                FormMenu form = new FormMenu();
                Closed += (s, args) => this.Close();
                form.Show();
            }
            else
            {

                MessageBox.Show("Попробуйте ещё раз!");
                guna2TextBox1.Text = "";
                captchaText = GenerateCaptchaText();
                captchaImage.Image = GenerateCaptchaImage(captchaText);
            }
        }
    }
}
