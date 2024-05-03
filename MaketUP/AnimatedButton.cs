using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace MaketUP
{
    public class AnimatedButton
    {
        private System.Windows.Forms.Timer animationTimer;
        private Color targetColor;
        private Color originalColor;
        private Color targetColorbg;
        private Color originalColorbg;
        private float animationProgress;
        private float animationSpeed = 0.05f;
        private Button button;
        private int animationDirection = 1; // 1 - в сторону цвета кнопки, -1 - обратно

        public AnimatedButton(Button button, Color targetColor, Color targetColorbg)
        {
            this.button = button;
            this.targetColor = targetColor;
            this.targetColorbg = targetColorbg;

            // Устанавливаем цвета по умолчанию
            originalColor = button.ForeColor;
            originalColorbg = button.BackColor;

            // Инициализируем таймер анимации
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 10; // Задаем интервал анимации в миллисекундах
            animationTimer.Tick += AnimationTimer_Tick;

            // Подписываемся на события MouseEnter и MouseLeave для кнопки
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            // Запускаем анимацию при наведении
            animationProgress = 0f;
            animationDirection = 1; // Устанавливаем направление анимации в сторону цвета кнопки
            animationTimer.Start();
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            // Продолжаем анимацию при уходе, чтобы цвет плавно вернулся к исходному состоянию
            animationDirection = -1; // Устанавливаем направление анимации обратно
            if (!animationTimer.Enabled) // Если таймер не запущен, запускаем его
            {
                animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Интерполируем цвет кнопки
            animationProgress += animationSpeed * animationDirection; // Изменяем прогресс в зависимости от направления анимации
            if (animationProgress <= 0f || animationProgress >= 1f) // Если прогресс достиг начала или конца, останавливаем анимацию
            {
                animationTimer.Stop();
                animationProgress = Math.Max(0f, Math.Min(1f, animationProgress)); // Ограничиваем прогресс между 0 и 1
            }

            // Рассчитываем текущий цвет
            Color currentColor = InterpolateColor(originalColor, targetColor, animationProgress);
            button.ForeColor = currentColor;
            Color currentColorbg = InterpolateColor(originalColorbg, targetColorbg, animationProgress);
            button.BackColor = currentColorbg;
        }

        private Color InterpolateColor(Color color1, Color color2, float fraction)
        {
            byte r = (byte)(color1.R + (color2.R - color1.R) * fraction);
            byte g = (byte)(color1.G + (color2.G - color1.G) * fraction);
            byte b = (byte)(color1.B + (color2.B - color1.B) * fraction);
            return Color.FromArgb(r, g, b);
        }
    }
}
