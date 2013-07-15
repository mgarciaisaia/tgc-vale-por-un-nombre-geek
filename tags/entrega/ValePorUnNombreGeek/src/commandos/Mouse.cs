using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using System.Drawing;
using System.Windows.Forms;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class Mouse
    {
        public static Point Position
        {
            get { return Cursor.Position; }
            set { Cursor.Position = value; }
        }

        public static bool isOverScreen()
        {
            Point screenPos = GuiController.Instance.Panel3d.PointToScreen(new Point(0, 0));
            int screenHeight = GuiController.Instance.Panel3d.Height;
            int screenWidth = GuiController.Instance.Panel3d.Width;

            return
                Mouse.Position.X > screenPos.X &&
                Mouse.Position.X < screenPos.X + screenWidth &&
                Mouse.Position.Y > screenPos.Y &&
                Mouse.Position.Y < screenPos.Y + screenHeight;
        }

        public static void show()
        {
            Cursor.Show();
        }

        public static void hide()
        {
            Cursor.Hide();
        }


    }
}
