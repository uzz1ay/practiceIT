using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    internal class HintManager
    {

        public static void SetHint(TextBox textBox, string hintText)
        {
            textBox.Tag = hintText; // Сохраняем подсказку в Tag
            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.Text = hintText;
            textBox.ForeColor = Color.Gray;
        }

        private static void TextBox_GotFocus(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Если текст совпадает с подсказкой, очищаем его
            if (textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private static void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Если поле пустое, возвращаем подсказку
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.ForeColor = Color.Gray;
            }
        }
    }
}
