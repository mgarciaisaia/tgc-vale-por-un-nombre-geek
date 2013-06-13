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

        public static bool isOverViewport()
        {
            Point viewportPos = GuiController.Instance.Panel3d.PointToScreen(new Point(0, 0));
            int viewportHeight = GuiController.Instance.Panel3d.Height;
            int viewportWidth = GuiController.Instance.Panel3d.Width;

            return
                Mouse.Position.X > viewportPos.X &&
                Mouse.Position.X < viewportPos.X + viewportWidth &&
                Mouse.Position.Y > viewportPos.Y &&
                Mouse.Position.Y < viewportPos.Y + viewportHeight;
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
