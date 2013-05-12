using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class UserVars
    {
        private static UserVars instance;

        private UserVars()
        {
            //singleton or something like that
            GuiController.Instance.Modifiers.addBoolean("showCylinder", "Ver cilindros", false);
            GuiController.Instance.Modifiers.addFloat("ZoomMapa", 0.5f, 5, 2);

        }

        public static void initialize()
        {
            instance = new UserVars();
        }

        public static UserVars Instance
        {
            get
            {
                //if (instance == null) instance = new UserVars();
                return instance;
            }
        }

        public bool renderCollisionNormal
        {
            get { return (bool)GuiController.Instance.Modifiers["showCylinder"]; }
        }

        public float zoomMapa { get { return (float)GuiController.Instance.Modifiers.getValue("ZoomMapa"); } }
    }
}
