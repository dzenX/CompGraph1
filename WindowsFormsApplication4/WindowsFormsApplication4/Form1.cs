using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace RasterAlgorithms
{
    public partial class Form1 : Form
    {
        //Метод Main() - точка входа в программу
        public static void Main()
        {
            using (Form1 frm = new Form1())
            {
                
                Application.Run(frm);
            }
        }
        //Конструктор пользовательского класса, наследующего класс Form
        public Form1()
        {
            InitializeComponent();
            ResizeRedraw = true;
        }

        //Обработчик события OnPaint. Отрисовывает на форме её содержимое
        protected override void OnPaint(PaintEventArgs e)
        {
           
            //Отрисовка окружности, построенной по алгоритму Брезенхема
            RasterAlgorithm.BresenhamCircle(
                e.Graphics,
                Color.Black,
                150, 150, 75);
            RasterAlgorithm.DDA(
                e.Graphics,
                Color.Black,
                100, 100, 200, 200);
            RasterAlgorithm.WuLine(
                e.Graphics,
                Color.Black,
                300, 100, 400, 200);
        }   
    }

    //Статический класс
    static class RasterAlgorithm
    {
        
        public static void BresenhamCircle(Graphics g, Color clr, int _x, int _y, int radius)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int x = 0, y = radius, gap = 0, delta = (2 - 2 * radius);
            while (y >= 0)
            {
                PutPixel(g, clr, _x + x, _y + y, 255);
                PutPixel(g, clr, _x + x, _y - y, 255);
                PutPixel(g, clr, _x - x, _y - y, 255);
                PutPixel(g, clr, _x - x, _y + y, 255);
                gap = 2 * (delta + y) - 1;
                if (delta < 0 && gap <= 0)
                {
                    x++;
                    delta += 2 * x + 1;
                    continue;
                }
                if (delta > 0 && gap > 0)
                {
                    y--;
                    delta -= 2 * y + 1;
                    continue;
                }
                x++;
                delta += 2 * (x - y);
                y--;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine();
        }
        
        private static void Swap(ref int x, ref int y)
        {
            int a = x;
            x = y;
            y = a;
        }
        private static void DrawPoint(Graphics g, Color col, bool steep, int x, int y, float intensive)
        {
            int x0 = x;
            int y0 = y;
            if (steep)
                Swap(ref x0, ref y0);
            PutPixel(g, col, x0, y0, (int)(255 - intensive));
        }
        public static void WuLine(Graphics g, Color col, int x0, int y0, int x1, int y1)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            DrawPoint(g, col, steep, x0, y0, 1); // Эта функция автоматом меняет координаты местами в зависимости от переменной steep
            DrawPoint(g, col, steep, x1, y1, 1); // Последний аргумент — интенсивность в долях единицы
            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dy / dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x <= x1 - 1; x++)
            {
                DrawPoint(g, col, steep, x, (int)y, 1 - (y - (int)y));
                DrawPoint(g, col, steep, x, (int)y + 1, y - (int)y);
                y += gradient;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine();
        }
        public static void DDA(Graphics g, Color col, int x1, int y1, int x2, int y2)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int i, L;
            float dX, dY;
            float[] x = new float[2000];
            float[] y = new float[2000];
            L = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
            dX = (x2 - x1) / L;
            dY = (y2 - y1) / L;
            i = 0;
            x[i] = x1;
            y[i] = y1;
            i++;
            while(i < L)
            {
                x[i] = x[i - 1] + dX;
                y[i] = y[i - 1] + dY;
                i++;
            }
            x[i] = x2;
            y[i] = y2;

            i = 0;
            while(i <= L)
            {
                PutPixel(g, col, (int)x[i], (int)y[i], 255);
                i++;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            Console.WriteLine();
        }
        //Метод, устанавливающий пиксел на форме с заданными цветом и прозрачностью
        private static void PutPixel(Graphics g, Color col, int x, int y, int alpha = 255)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, col)), x, y, 2, 2);
        }
    }
}
