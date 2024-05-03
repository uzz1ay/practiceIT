using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    public partial class Form1 : Form
    {
        private AnimatedButton animatedButton1, animatedButton3, animatedButton2;
        private Button currentButton;
        public Form1()
        {
            InitializeComponent();
            ComponentRounder.SetRoundedShape(textBox1, 15);
            ComponentRounder.SetRoundedShape(textBox2, 15);
            ComponentRounder.SetRoundedShape(panel1, 15);
            ComponentRounder.SetRoundedShape(panel2, 30);
            ComponentRounder.SetRoundedShape(panel3, 30);
            ComponentRounder.SetRoundedShape(panel4, 30);
            ComponentRounder.SetRoundedShape(panel5, 30);
            textBox1.Text = ClassStorage.login;
            textBox2.Text = ClassStorage.password;
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                // Установка текстовых подсказок
                HintManager.SetHint(textBox1, "Введите логин");
                HintManager.SetHint(textBox2, "123456789");
            }
            


            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedButton(button2, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton3 = new AnimatedButton(button3, Color.FromArgb(130, 6, 255), Color.White);

        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            FormRegistration form = new FormRegistration();
            form.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            HintManager.SetHint(textBox1, "Введите почту");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            HintManager.SetHint(textBox1, "Введите номер телефона");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            FormForgetPassword form = new FormForgetPassword();
            Closed += (s, args) => this.Close();
            form.Show();
        }
        private string connectionString = "Server = localhost;port = 5432;username=postgres;password=123;database=postgres";
        private void button1_Click(object sender, EventArgs e)
        {
            
            string login = textBox1.Text;
            string password = textBox2.Text;
            if (checkBox1.Checked)
            {
                ClassStorage.login = login;
                ClassStorage.password = password;
            }
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
                        using (NpgsqlConnection conn2 = new NpgsqlConnection(connectionString))
                        {
                            conn2.Open();
                            string query2 = "SELECT EXISTS(SELECT * FROM Администратор WHERE Логин=@login AND Пароль=@password)";
                            using (NpgsqlCommand cmd2 = new NpgsqlCommand(query2, conn2))
                            {
                                cmd2.Parameters.AddWithValue("@login", login);
                                cmd2.Parameters.AddWithValue("@password", password);
                                bool result2 = (bool)cmd2.ExecuteScalar();
                                if (result2)
                                {
                                    MessageBox.Show("Авторизовано!");
                                    this.Hide();
                                    FormMenu form = new FormMenu();
                                    Closed += (s, args) => this.Close();
                                    form.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Неверный пароль.");
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {

            HintManager.SetHint(textBox1, "Введите логин");
        }
        private bool statusPasswordChar = false;
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            statusPasswordChar = !statusPasswordChar;
            if (!statusPasswordChar)
            {
                string imagePath = @"C:/Users/stud/Desktop/MaketUP/MaketUP/Resources/icons8-eye-100.png";
                button4.BackgroundImage = Image.FromFile(imagePath);
                textBox2.UseSystemPasswordChar = false;
            }
            if(statusPasswordChar)
            {
                string imagePath = @"C:/Users/stud/Desktop/MaketUP/MaketUP/Resources/icons8-closed-eye-100.png";
                button4.BackgroundImage = Image.FromFile(imagePath);
                textBox2.UseSystemPasswordChar = true;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormRegistration form = new FormRegistration();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        
    }
}
