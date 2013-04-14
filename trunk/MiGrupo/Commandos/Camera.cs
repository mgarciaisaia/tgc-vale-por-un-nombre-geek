using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class Camera
    {
        public Camera(Vector3 _posInicial)
        {
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(_posInicial, 300, -300);

            //Checkbox para deshabilitar el movimiento de la camara con el mouse (quedaria solo con las flechitas)
            GuiController.Instance.Modifiers.addBoolean("Camara", "Usar mouse", true);
        }

        public void refresh(float cameraSpeed)
        {
            bool mouseEnabled = (bool)GuiController.Instance.Modifiers.getValue("Camara");
            float dx = 0, dz = 0;
            int screenHeight = GuiController.Instance.Panel3d.Height;
            int screenWidth = GuiController.Instance.Panel3d.Width;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //move up
            if (mouseEnabled && (d3dInput.Ypos <= 100 && d3dInput.Ypos > 0) || d3dInput.keyDown(Key.UpArrow))
            {
                dz = cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move down
            if (mouseEnabled && (d3dInput.Ypos >= screenHeight - 100 && d3dInput.Ypos < screenHeight) || d3dInput.keyDown(Key.DownArrow))
            {
                dz = -cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move left
            if (mouseEnabled && (d3dInput.Xpos <= 100 && d3dInput.Xpos > 0) || d3dInput.keyDown(Key.LeftArrow))
            {
                dx = -cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move right
            if (mouseEnabled && (d3dInput.Xpos >= screenWidth - 100 && d3dInput.Xpos < screenWidth) || d3dInput.keyDown(Key.RightArrow))
            {
                dx = cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            Vector3 desplazamiento = new Vector3(dx, 0, dz);
            GuiController.Instance.ThirdPersonCamera.Target += desplazamiento;
        }


    }
}
