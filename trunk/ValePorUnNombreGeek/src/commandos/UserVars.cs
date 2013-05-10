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
            GuiController.Instance.Modifiers.addBoolean("showCollision", "Mostrar colisiones", false);
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
            get { return (bool)GuiController.Instance.Modifiers["showCollision"]; }
        }
    }
}
