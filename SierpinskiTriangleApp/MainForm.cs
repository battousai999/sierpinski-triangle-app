using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SierpinskiTriangleApp
{
    public partial class MainForm : Form
    {
        private Graphics gc;
        private Point lastPoint;
        private Random random = new Random();
        private Point[] cornerPoints = new Point[3];
        private Brush brush = Brushes.Black;
        private Thread drawingThread;

        public MainForm()
        {
            InitializeComponent();

            SetCornerPoints();

            drawingThread = new Thread(DrawingThreadFunc)
            {
                IsBackground = true
            };

            drawingThread.Start();
        }

        private void DrawingThreadFunc()
        {
            while (true)
            {
                Invoke((Action)(() =>
                {
                    var nextPoint = GetNextPoint();
                    gc.FillRectangle(brush, nextPoint.X, nextPoint.Y, 1, 1);
                }));
            }
        }

        private void SetCornerPoints()
        {
            int size = Math.Max(100, Math.Min(Width - 50, Height - 50));

            cornerPoints[0] = new Point(0, size);
            cornerPoints[1] = new Point(size, size);
            cornerPoints[2] = new Point(size / 2, 0);

            gc = CreateGraphics();
            lastPoint = cornerPoints[0];
        }

        private Point GetNextPoint()
        {
            var targetPoint = cornerPoints[random.Next(3)];

            var x = Math.Abs(targetPoint.X + ((lastPoint.X - targetPoint.X) / 2));
            var y = Math.Abs(targetPoint.Y + ((lastPoint.Y - targetPoint.Y) / 2));

            lastPoint = new Point(x, y);
            return lastPoint;
        }

        private void ClearSurface()
        {
            gc.Clear(BackColor);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            SetCornerPoints();

            ClearSurface();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Form1_ResizeEnd(this, EventArgs.Empty);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            drawingThread.Abort();
        }
    }
}
