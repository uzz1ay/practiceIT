using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace MaketUP
{
    internal class HintManager
    {

        public static void SetHint(Guna2TextBox guna2TextBox, string hintText)
        {
            guna2TextBox.Tag = hintText; // Сохраняем подсказку в Tag
            guna2TextBox.GotFocus += TextBox_GotFocus;
            guna2TextBox.LostFocus += TextBox_LostFocus;
            guna2TextBox.Text = hintText;
            guna2TextBox.ForeColor = Color.Gray;
        }

        private static void TextBox_GotFocus(object sender, EventArgs e)
        {
            Guna2TextBox guna2TextBox = (Guna2TextBox)sender;
            // Если текст совпадает с подсказкой, очищаем его
            if (guna2TextBox.Text == guna2TextBox.Tag.ToString())
            {
                guna2TextBox.Text = "";
                guna2TextBox.ForeColor = Color.Black;
            }
        }

        private static void TextBox_LostFocus(object sender, EventArgs e)
        {
            Guna2TextBox guna2TextBox = (Guna2TextBox)sender;
            // Если поле пустое, возвращаем подсказку
            if (string.IsNullOrEmpty(guna2TextBox.Text))
            {
                guna2TextBox.Text = guna2TextBox.Tag.ToString();
                guna2TextBox.ForeColor = Color.Gray;
            }
        }
    }
}
