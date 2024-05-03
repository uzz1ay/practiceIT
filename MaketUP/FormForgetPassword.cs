using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    public partial class FormForgetPassword : Form
    {
        private AnimatedButton animatedButton1, animatedButton2, animatedButton3, animatedButton4;
        private int verificationCode;
        private void button2_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            verificationCode = random.Next(100000, 999999);
            string email = "uselesspuck@gmail.com";
            string subject = "Код для подтверждения";
            string body = $"Ваш код для подтверждения: {verificationCode}";
            string login = guna2TextBox2.Text;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT EXISTS(SELECT * FROM Администратор WHERE Логин=@login)";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    bool result = (bool)cmd.ExecuteScalar();
                    if (result)
                    {

                        guna2TextBox1.Visible = true;
                        label3.Visible = true;
                        button3.Visible = true;
                        using (MailMessage message = new MailMessage("uselesspuck@gmail.com", email, subject, body))
                        {
                            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                            {
                                smtpClient.Port = 587;
                                smtpClient.Credentials = new NetworkCredential("uselesspuck@gmail.com", "tkkt tinq iptk miwy");

                                smtpClient.EnableSsl = true;
                                try
                                {
                                    smtpClient.Send(message);
                                    MessageBox.Show("Код подтверждения отправлен на почту");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Ошибка при отправке сообщения : {ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти пользователя.");
                    }
                }
            }
            
            
            
        }
        private string connectionString = "Server = localhost;port = 5432;username=postgres;password=123;database=postgres";
        private void button4_Click(object sender, EventArgs e)
        {
            string login = guna2TextBox2.Text;
            string password = guna2TextBox3.Text;
            string input = guna2TextBox3.Text;
            if (guna2TextBox3.Text != "")
            {
                if (input.Length <= 6 || !Regex.IsMatch(input, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
                {
                    MessageBox.Show("Рекомендуется использовать пароль длинной более 6 символов,сдержащий буквы и числа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (input.Length >= 6 && Regex.IsMatch(input, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Администратор SET Пароль = @password WHERE Логин = @login";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@login", login);
                            cmd.Parameters.AddWithValue("@password", password);
                            string result = (string)cmd.ExecuteScalar();

                            MessageBox.Show("Ваш пароль изменен на: " + password);
                            this.Hide();
                            Closed += (s, args) => this.Close();


                        }
                    }
                    this.Hide();
                    Form1 form = new Form1();
                    Closed += (s, args) => this.Close();
                    form.Show();
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text != "")
            {
                int enteredCode = Convert.ToInt32(guna2TextBox1.Text);
                if (int.TryParse(guna2TextBox1.Text, out enteredCode))
                {
                    if (enteredCode == verificationCode)
                    {
                        guna2TextBox3.Visible = true;
                        label4.Visible = true;
                        button4.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Неверный код!");
                    }
                }
                else
                {
                    MessageBox.Show("Неверный код!");
                }
            }
            else
            {
                MessageBox.Show("Поле не может быть пустым!");
            }
            
        }

        public FormForgetPassword()
        {
            InitializeComponent();


            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedButton(button2, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton3 = new AnimatedButton(button3, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton4 = new AnimatedButton(button4, Color.FromArgb(130, 6, 255), Color.White);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }
    }
}
