using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaketUP
{
    internal class ComponentRounder
    {
        public static void SetRoundedShape(Control control, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddLine(radius, 0, control.Width - radius, 0);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddLine(control.Width, radius, control.Width, control.Height - radius);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddLine(control.Width - radius, control.Height, radius, control.Height);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.AddLine(0, control.Height - radius, 0, radius);
            path.AddArc(0, 0, radius, radius, 180, 90);
            control.Region = new Region(path);
            Bitmap bmp = new Bitmap(control.Width, control.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Устанавливаем сглаживание
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Закрашиваем путь
                using (Brush brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, path);
                }

                // Устанавливаем Region для control
                control.Region = new Region(path);
            }

            // Если control является PictureBox, например, вы можете установить Image
            if (control is PictureBox)
            {
                ((PictureBox)control).Image = bmp;
            }
        }
    }
}
