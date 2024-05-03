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


            ComponentRounder.SetRoundedShape(textBox1, 15);
            ComponentRounder.SetRoundedShape(textBox2, 15);
            ComponentRounder.SetRoundedShape(textBox3, 15);
            ComponentRounder.SetRoundedShape(textBox4, 15);
            ComponentRounder.SetRoundedShape(textBox5, 15);
            ComponentRounder.SetRoundedShape(textBox6, 15);
            ComponentRounder.SetRoundedShape(panel1, 30);


            HintManager.SetHint(textBox1, "Введите логин");
            HintManager.SetHint(textBox2, "Введите пароль");
            HintManager.SetHint(textBox3, "Введите номер телефона");
            HintManager.SetHint(textBox4, "Введите пароль");
            HintManager.SetHint(textBox5, "Введите логин");
            HintManager.SetHint(textBox6, "Введите пароль");

            animatedButton1 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedButton(button2, Color.FromArgb(130, 6, 255), Color.White);
        }
        private void FormRegistration_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Login = textBox4.Text;
            string Password = textBox5.Text;
            string input = textBox5.Text;
            if (input.Length <= 6 || !Regex.IsMatch(input, @"[a-zA-Za-яА-Я0-9]+$"))
            {
                MessageBox.Show("Рекомендуется использовать пароль длинной более 6 символов,сдержащий буквы и числа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Администратор (Логин,Пароль) VALUES (@Login,@Password);";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Login", Login);
                        cmd.Parameters.AddWithValue("@Password", Password);
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            Closed += (s, args) => this.Close();
            form.Show();
        }
    }
}
