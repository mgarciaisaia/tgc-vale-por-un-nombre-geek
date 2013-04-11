using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MiGrupo
{
    class Utils
    {

        public static void desplazarVistaConMouse(float cameraSpeed)
        {
            Vector3 desplazamiento;
            float dx = 0, dz = 0;

            //Mover si el mouse está en un borde.
            if (GuiController.Instance.D3dInput.Xpos <= 100 && GuiController.Instance.D3dInput.Xpos > 0)

                dx = -cameraSpeed * GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Xpos >= GuiController.Instance.Panel3d.Width - 100 && GuiController.Instance.D3dInput.Xpos < GuiController.Instance.Panel3d.Width)

                dx = cameraSpeed * GuiController.Instance.ElapsedTime;

            if (GuiController.Instance.D3dInput.Ypos <= 100 && GuiController.Instance.D3dInput.Ypos > -100)

                dz = cameraSpeed * GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Ypos >= GuiController.Instance.Panel3d.Height - 100 && GuiController.Instance.D3dInput.Ypos < GuiController.Instance.Panel3d.Height)

                dz = -cameraSpeed * GuiController.Instance.ElapsedTime;


            desplazamiento = new Vector3(dx, 0, dz);
            GuiController.Instance.ThirdPersonCamera.Target = GuiController.Instance.ThirdPersonCamera.Target + desplazamiento;
        }
    }
}
