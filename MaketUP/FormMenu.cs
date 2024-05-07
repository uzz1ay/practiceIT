using Guna.UI2.WinForms;
using Npgsql;
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
        private AnimatedGunoButton animatedButton1, animatedButton3, animatedButton2, animatedButton4, animatedButton5;
        private AnimatedButton animatedButton11, animatedButton33, animatedButton22, animatedButton44, animatedButton55, animatedButton66, animatedButton77, animatedButton88, animatedButton99;

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormRegistration form = new FormRegistration();
            Closed += (s, args) => this.Close();
            form.Show();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            guna2Panel2.Visible = true;
            guna2Panel3.Visible = false;
            if(ClassStorage.role == "admin")
            {
                guna2HtmlLabel6.Text = "Вы вошли как  Администратор";
            }
            else
            {
                guna2HtmlLabel6.Text = "Вы вошли как  Пользователь";
            }
        }

        private void LoadData()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(comboboxQuery, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    conn.Close();
                    guna2DataGridView1.DataSource = table;
                    
                }
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                if(guna2ComboBox1.Text == "Администратор")
                {
                    DataGridViewRow row = guna2DataGridView1.Rows[e.RowIndex];
                    string login = System.Text.RegularExpressions.Regex.Replace(row.Cells["Логин"].Value.ToString(), @"\s +", " ").Trim();
                    ClassStorage.selectedLogin = login;
                    string password = System.Text.RegularExpressions.Regex.Replace(row.Cells["Пароль"].Value.ToString(), @"\s +", " ").Trim();
                    ClassStorage.selectedPassword = password;
                    string phone_number = System.Text.RegularExpressions.Regex.Replace(row.Cells["Номер_телефона"].Value.ToString(), @"\s +", " ").Trim();
                    ClassStorage.selectedPhone = phone_number;
                    string mail = System.Text.RegularExpressions.Regex.Replace(row.Cells["Почта"].Value.ToString(), @"\s +", " ").Trim();
                    ClassStorage.selectedMail = mail;
                    string role = System.Text.RegularExpressions.Regex.Replace(row.Cells["Роль"].Value.ToString(), @"\s +", " ").Trim();
                    ClassStorage.selectedRole = role;
                    MessageBox.Show($"Пользователь с логином {login} был выбран");
                }
                else
                {
                    //обработчик кнопок для других таблиц
                }
                
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Администратор WHERE Логин = @login AND Пароль = @password AND Номер_телефона = @phone_number AND Почта = @mail;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", ClassStorage.selectedLogin);
                    cmd.Parameters.AddWithValue("@password", ClassStorage.selectedPassword);
                    cmd.Parameters.AddWithValue("@phone_number", ClassStorage.selectedPhone);
                    cmd.Parameters.AddWithValue("@mail", ClassStorage.selectedMail);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Пользователь с логином {ClassStorage.selectedLogin} был удален");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить пользователя");
                    }
                    conn.Close();

                }
            }

            LoadData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (ClassStorage.selectedRole == "admin")
            {
                ClassStorage.selectedRole = "user";
            }
            else if (ClassStorage.selectedRole == "user")
            {
                ClassStorage.selectedRole = "admin";
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Администратор SET Роль = @role WHERE Логин = @login AND Пароль = @password AND Номер_телефона = @phone_number AND Почта = @mail;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", ClassStorage.selectedLogin);
                    cmd.Parameters.AddWithValue("@password", ClassStorage.selectedPassword);
                    cmd.Parameters.AddWithValue("@phone_number", ClassStorage.selectedPhone);
                    cmd.Parameters.AddWithValue("@mail", ClassStorage.selectedMail);
                    cmd.Parameters.AddWithValue("@role", ClassStorage.selectedRole);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Роль пользователя {ClassStorage.selectedLogin} была изменена на {ClassStorage.selectedRole}");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить пользователя");
                    }
                    conn.Close();

                }
            }

            LoadData();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Неверный_пароль_счетчик FROM Администратор WHERE Логин = @login AND Пароль = @password ;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", ClassStorage.selectedLogin);
                    cmd.Parameters.AddWithValue("@password", ClassStorage.selectedPassword);
                    string countpassword = cmd.ExecuteScalar().ToString();
                    ClassStorage.countWrondPassword = Convert.ToInt32(countpassword);
                    
                }


                conn.Close();
            }
            MessageBox.Show($"Количество неправильно введеных паролей для этого пользователя = {ClassStorage.countWrondPassword}");
        }
        private string comboboxQuery;
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = guna2ComboBox1.Text;
            switch (text)
            {
                case "Администратор":
                    comboboxQuery = "SELECT Логин, Пароль,Номер_телефона,Почта,Роль FROM Администратор";
                    button5.Visible = true;
                    button6.Visible = true;
                    button7.Visible = true;
                    button8.Visible = true;
                    button9.Visible = true;
                    LoadData();
                    break;
                case "Заготовка":
                    comboboxQuery = "SELECT * FROM Заготовка";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Изделие":
                    comboboxQuery = "SELECT * FROM Изделие";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Клиент":
                    comboboxQuery = "SELECT * FROM Клиент";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Материал":
                    comboboxQuery = "SELECT * FROM Материал";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Обработка":
                    comboboxQuery = "SELECT * FROM Обработка";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Поставка_материал":
                    comboboxQuery = "SELECT * FROM Поставка_материал";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;
                case "Поставщик":
                    comboboxQuery = "SELECT * FROM Поставщик";
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = false;
                    button8.Visible = false;
                    button9.Visible = false;
                    LoadData();
                    break;

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            comboboxQuery = "SELECT Последний_вход FROM Администратор";
            LoadData();
        }
        private bool redactProfile = false;
        private void button2_Click(object sender, EventArgs e)
        {
            redactProfile = !redactProfile;
            if(redactProfile == true)
            {
                guna2TextBox1.Visible = true;
                guna2TextBox2.Visible = true;
                guna2TextBox3.Visible = true;
                button2.Text = "Отменить";
                guna2TextBox1.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.mail, @"\s +", " ").Trim();
                guna2TextBox2.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.login, @"\s +", " ").Trim();
                guna2TextBox3.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.phone, @"\s +", " ").Trim();
                guna2HtmlLabel9.Visible = false;
                guna2HtmlLabel12.Visible = false;
                guna2HtmlLabel13.Visible = false;
                button4.Visible = true;
            }
            else
            {
                guna2TextBox1.Visible = false;
                guna2TextBox2.Visible = false;
                guna2TextBox3.Visible = false;
                button2.Text = "Редактировать профиль";
                guna2HtmlLabel9.Visible = true;
                guna2HtmlLabel12.Visible = true;
                guna2HtmlLabel13.Visible = true;
                button4.Visible = false;
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mail = guna2TextBox1.Text;
            string phone = guna2TextBox3.Text;
            string login = guna2TextBox2.Text;

            if (mail.ToLower().Contains("@gmail.com"))
            {
                if (phone.Length < 13 && phone.Contains("79"))
                {
                    using (NpgsqlConnection conn3 = new NpgsqlConnection(connectionString))
                    {
                        conn3.Open();
                        string query3 = "SELECT EXISTS(SELECT * FROM Администратор WHERE Номер_телефона=@Phone OR Почта=@Mail OR Логин=@Login)";
                        using (NpgsqlCommand cmd3 = new NpgsqlCommand(query3, conn3))
                        {
                            cmd3.Parameters.AddWithValue("@Login", login);
                            cmd3.Parameters.AddWithValue("@Phone", phone);
                            cmd3.Parameters.AddWithValue("@Mail", mail);
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
                                    string query = "UPDATE Администратор SET Логин = @login,Почта = @mail,Номер_телефона = @phone WHERE Логин = @oldLogin AND Пароль = @password ;";
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@login", guna2TextBox2.Text);
                                        cmd.Parameters.AddWithValue("@oldLogin", ClassStorage.login);
                                        cmd.Parameters.AddWithValue("@password", ClassStorage.password);
                                        cmd.Parameters.AddWithValue("@phone", guna2TextBox3.Text);
                                        cmd.Parameters.AddWithValue("@mail", guna2TextBox1.Text);
                                        cmd.Parameters.AddWithValue("@role", ClassStorage.role);
                                        int rowsAffected = cmd.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show($"Данные обновлены!");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Не удалось изменить данные");
                                        }
                                        conn.Close();

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный номер телефона");
                }
            }
            else
            {
                MessageBox.Show("Некорректный почтовый адрес");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCaptcha form = new FormCaptcha();
            form.Show();
        }

        private string connectionString = "Server = localhost;port = 5432;username=postgres;password=123;database=postgres";

        private void guna2Button1_Click(object sender, EventArgs e)
        {
           
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            guna2Panel2.Visible = false;
            guna2Panel3.Visible = true;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Логин,Пароль,Почта,Номер_телефона,Роль FROM Администратор WHERE Логин = @login AND Пароль = @password ;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", ClassStorage.authlogin);
                    cmd.Parameters.AddWithValue("@password", ClassStorage.authpassword);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string login = reader.GetString(0);
                            string password = reader.GetString(1);
                            string mail = reader.GetString(2);
                            string phone = reader.GetString(3);
                            string role = reader.GetString(4);

                            ClassStorage.login = System.Text.RegularExpressions.Regex.Replace(login, @"\s +", " ").Trim();
                            ClassStorage.password = System.Text.RegularExpressions.Regex.Replace(password, @"\s +", " ").Trim();
                            ClassStorage.mail = System.Text.RegularExpressions.Regex.Replace(mail, @"\s +", " ").Trim();
                            ClassStorage.phone = System.Text.RegularExpressions.Regex.Replace(phone, @"\s +", " ").Trim();
                            ClassStorage.role = System.Text.RegularExpressions.Regex.Replace(role, @"\s +", " ").Trim();
                        }
                    }

                }

                conn.Close();
            }
            guna2HtmlLabel9.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.login, @"\s +", " ").Trim();
            guna2HtmlLabel12.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.mail, @"\s +", " ").Trim();
            guna2HtmlLabel13.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.phone, @"\s +", " ").Trim();
            if(ClassStorage.capctha = false)
            {
                guna2HtmlLabel10.Text = "       ********";
            }
            else
            {
                guna2HtmlLabel10.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.password, @"\s +", " ").Trim();
            }
            
            guna2HtmlLabel11.Text = System.Text.RegularExpressions.Regex.Replace(ClassStorage.role, @"\s +", " ").Trim();

        }



        private Timer expandTimer = new Timer();
        private Timer collapseTimer = new Timer();
        public FormMenu()
        {
            InitializeComponent();

            guna2DataGridView1.CellClick += DataGridView_CellClick;

            animatedButton1 = new AnimatedGunoButton(guna2Button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton2 = new AnimatedGunoButton(guna2Button2, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton3 = new AnimatedGunoButton(guna2Button3, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton4 = new AnimatedGunoButton(guna2Button4, Color.FromArgb(130, 6, 255), Color.White);
            if(System.Text.RegularExpressions.Regex.Replace(ClassStorage.role, @"\s +", " ").Trim() == "admin")
            {
                guna2Button5.Visible = true;
                animatedButton5 = new AnimatedGunoButton(guna2Button5, Color.FromArgb(130, 6, 255), Color.White);
            }
            else
            {
                guna2Button5.Visible = false;
            }
            animatedButton11 = new AnimatedButton(button1, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton22 = new AnimatedButton(button2, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton33 = new AnimatedButton(button3, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton44 = new AnimatedButton(button4, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton55 = new AnimatedButton(button5, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton66 = new AnimatedButton(button6, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton77 = new AnimatedButton(button7, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton88 = new AnimatedButton(button8, Color.FromArgb(130, 6, 255), Color.White);
            animatedButton99 = new AnimatedButton(button9, Color.FromArgb(130, 6, 255), Color.White);
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

            if (guna2Panel1.Width < 320)
            {
                
                guna2Panel1.Width += 26;
                guna2Panel1.Refresh();
            }
            else
            {
                guna2Button1.Width = guna2Panel1.Width-10;
                guna2Button1.Text = "Главная";
                guna2Button1.Image = null;
                guna2Button2.Width = guna2Panel1.Width-10;
                guna2Button2.Text = "Профиль";
                guna2Button2.Image = null;
                guna2Button3.Width = guna2Panel1.Width-10;
                guna2Button3.Text = "Назад";
                guna2Button3.Image = null;
                guna2Button4.Width = guna2Panel1.Width-10;
                guna2Button4.Text = "График";
                guna2Button4.Image = null;
                guna2Button5.Width = guna2Panel1.Width - 10;
                guna2Button5.Text = "Администрирование";
                guna2Button5.Image = null;
                
                
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

            if (guna2Panel1.Width > 60)
            {
                guna2Panel1.Width -= 26;
                guna2Panel1.Refresh();
            }
            else
            {
                guna2Button1.Width = guna2Panel1.Width-10;
                guna2Button1.Text = "";
                guna2Button1.Image = Properties.Resources.homepage; 
                guna2Button2.Width = guna2Panel1.Width-10;
                guna2Button2.Text = "";
                guna2Button2.Image = Properties.Resources.profile_user;
                guna2Button3.Width = guna2Panel1.Width-10;
                guna2Button3.Text = "";
                guna2Button3.Image = Properties.Resources.back_button;
                guna2Button4.Width = guna2Panel1.Width-10;
                guna2Button4.Text = "";
                guna2Button4.Image = Properties.Resources.chart;
                guna2Button5.Width = guna2Panel1.Width - 10;
                guna2Button5.Text = "";
                guna2Button5.Image = Properties.Resources.administration;
                
                collapseTimer.Stop();
            }
        }

        
    }
}
