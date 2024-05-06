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
        public Form1()
        {
            InitializeComponent();

            guna2TextBox2.Text = ClassStorage.login;
            guna2TextBox3.Text = ClassStorage.password;
            if (guna2TextBox3.Text == "" || guna2TextBox2.Text == "")
            {
                // Установка текстовых подсказок
                HintManager.SetHint(guna2TextBox2, "Введите логин");
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
            guna2TextBox3.UseSystemPasswordChar = true;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
           
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
           
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
            
            string login = System.Text.RegularExpressions.Regex.Replace(guna2TextBox2.Text, @"\s +", " ").Trim();
            string password = System.Text.RegularExpressions.Regex.Replace(guna2TextBox3.Text, @"\s +", " ").Trim();
            DateTime last_auth = DateTime.Now;
            string formated_last_auth = last_auth.ToString("yyyy-MM-dd HH:mm:ss");
            ClassStorage.last_auth = formated_last_auth; 
            ClassStorage.selectedLogin = login;
            ClassStorage.selectedPassword = password;
            string Role;
            //ПРОДУМАТЬ ВХОД С НОМЕРОМ И ПОЧТОЙ,ТАК КАК ТАМ НЕ ВВОДЯТ "АДМИН",СКОРЕЕ ВСЕГО С ПОМОЩЬЮ ЗАПРОСА
            if (login.ToLower().Contains("admin"))
            {
                Role = "admin";
                ClassStorage.role = "admin";
            }
            else
            {
                Role = "user";
                ClassStorage.role = "user";
            }
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
                                    lastAuth();
                                    MessageBox.Show("Авторизовано!");
                                    this.Hide();
                                    FormMenu form = new FormMenu();
                                    Closed += (s, args) => this.Close();
                                    form.Show();
                                }
                                else
                                {
                                    ClassStorage.countWrondPassword += 1;
                                    MessageBox.Show("Неверный пароль.");
                                }
                            }
                        }
                    }
                    else
                    {
                        using (NpgsqlConnection conn2 = new NpgsqlConnection(connectionString))
                        {
                            conn2.Open();
                            string query2 = "SELECT EXISTS(SELECT * FROM Администратор WHERE Номер_телефона=@login AND Пароль=@password)";
                            using (NpgsqlCommand cmd2 = new NpgsqlCommand(query2, conn2))
                            {
                                cmd2.Parameters.AddWithValue("@login", login);
                                cmd2.Parameters.AddWithValue("@password", password);
                                bool result2 = (bool)cmd2.ExecuteScalar();
                                if (result2)
                                {
                                    lastAuth();
                                    MessageBox.Show("Авторизовано!");
                                    this.Hide();
                                    FormMenu form = new FormMenu();
                                    Closed += (s, args) => this.Close();
                                    form.Show();
                                }
                                else
                                {
                                    using (NpgsqlConnection conn3 = new NpgsqlConnection(connectionString))
                                    {
                                        conn3.Open();
                                        string query3 = "SELECT EXISTS(SELECT * FROM Администратор WHERE Почта=@login AND Пароль=@password)";
                                        using (NpgsqlCommand cmd3 = new NpgsqlCommand(query3, conn3))
                                        {
                                            cmd3.Parameters.AddWithValue("@login", login);
                                            cmd3.Parameters.AddWithValue("@password", password);
                                            bool result3 = (bool)cmd3.ExecuteScalar();
                                            if (result3)
                                            {
                                                lastAuth();
                                                MessageBox.Show("Авторизовано!");
                                                conn.Close();
                                                this.Hide();
                                                FormMenu form = new FormMenu();
                                                Closed += (s, args) => this.Close();
                                                form.Show();
                                            }
                                            else
                                            {
                                                MessageBox.Show("Не удалось найти пользователя.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
        private void lastAuth()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                
                    string query = "UPDATE Администратор SET Последний_вход = @date WHERE Логин = @login AND Пароль = @password ;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", ClassStorage.selectedLogin);
                        cmd.Parameters.AddWithValue("@password", ClassStorage.selectedPassword);
                        cmd.Parameters.AddWithValue("@date", ClassStorage.last_auth);
                        cmd.Parameters.AddWithValue("@count_password", ClassStorage.countWrondPassword); 
                        int rowsAffected = cmd.ExecuteNonQuery();

                    }
                string query2 = "UPDATE Администратор SET Неверный_пароль_счетчик = @count_password WHERE Логин = @login AND Пароль = @password ;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", ClassStorage.selectedLogin);
                    cmd.Parameters.AddWithValue("@password", ClassStorage.selectedPassword);
                    cmd.Parameters.AddWithValue("@date", ClassStorage.last_auth);
                    cmd.Parameters.AddWithValue("@count_password", ClassStorage.countWrondPassword);
                    int rowsAffected = cmd.ExecuteNonQuery();

                }


                conn.Close();
            }
                
        }

        
        private bool statusPasswordChar = true;

        private void guna2Panel2_Click(object sender, EventArgs e)
        {
            ClassStorage.role = "admin";
            this.Hide();
            FormMenu form = new FormMenu();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            statusPasswordChar = !statusPasswordChar;
            guna2TextBox3.UseSystemPasswordChar = statusPasswordChar;
            if (statusPasswordChar == true)
            {
                
                button4.BackgroundImage = Properties.Resources.icons8_eye_100;
            }
            if(statusPasswordChar == false)
            {
                
                button4.BackgroundImage = Properties.Resources.icons8_closed_eye_100;
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
