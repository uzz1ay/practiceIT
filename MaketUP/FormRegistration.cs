using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    public partial class FormRegistration : Form
    {
        private string connectionString = "Server = localhost;port = 5432;username=postgres;password=123;database=postgres";
        private AnimatedButton animatedButton1, animatedButton2;
        private Button currentButton;
        public FormRegistration()
        {
            InitializeComponent();


            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedButton(button2, Color.FromArgb(130, 6, 255), Color.White);
        }
        private void FormRegistration_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Login = System.Text.RegularExpressions.Regex.Replace(guna2TextBox2.Text,@"\s +"," ").Trim() ;
            string Password = System.Text.RegularExpressions.Regex.Replace(guna2TextBox3.Text, @"\s +", " ").Trim();
            string Phone = System.Text.RegularExpressions.Regex.Replace(guna2TextBox4.Text, @"\s +", " ").Trim();
            string Mail = System.Text.RegularExpressions.Regex.Replace(guna2TextBox1.Text, @"\s +", " ").Trim();
            string input = Password;
            if ((guna2TextBox3.Text != "" && guna2TextBox2.Text!= "") &&(guna2TextBox4.Text != "" && guna2TextBox1.Text != ""))
            {
                if (input.Length <= 6 || !Regex.IsMatch(input, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
                {
                    MessageBox.Show("Рекомендуется использовать пароль длинной более 6 символов,сдержащий буквы и числа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(input.Length >= 6 && Regex.IsMatch(input, @"[a-zA-Za-яА-Я0-9-\w\W]+$"))
                {
                    
                    using (NpgsqlConnection conn3 = new NpgsqlConnection(connectionString))
                    {
                        conn3.Open();
                        string query3 = "SELECT EXISTS(SELECT * FROM Администратор WHERE Номер_телефона=@Phone OR Почта=@Mail OR Логин=@Login)";
                        using (NpgsqlCommand cmd3 = new NpgsqlCommand(query3, conn3))
                        {
                            cmd3.Parameters.AddWithValue("@Login", Login);
                            cmd3.Parameters.AddWithValue("@Phone", Phone);
                            cmd3.Parameters.AddWithValue("@Mail", Mail);
                            bool result3 = (bool)cmd3.ExecuteScalar();
                            if (result3)
                            {
                                MessageBox.Show("Пользователь с данным логином/почтой/телефоном занят");
                            }
                            else
                            {
                                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                                {
                                    conn.Open();
                                    string query = "INSERT INTO Администратор (Логин,Пароль,Номер_телефона,Почта) VALUES (@Login,@Password,@Phone,@Mail);";
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@Login", Login);
                                        cmd.Parameters.AddWithValue("@Password", Password);
                                        cmd.Parameters.AddWithValue("@Phone", Phone);
                                        cmd.Parameters.AddWithValue("@Mail", Mail);
                                        int rowsAffected = cmd.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Пользователь добавлен!");
                                            this.Hide();
                                            Form1 form = new Form1();
                                            Closed += (s, args) => this.Close();
                                            form.Show();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Не удалось добавить пользователя!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Поля не могут быть пустыми!");
            }
            
            
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
