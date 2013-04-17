using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class StaticCamera
    {
        public static int ANCHO_DESPLAZAMIENTO = 50;

        public StaticCamera(Vector3 _posInicial)
        {
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(_posInicial, 300, -300);

            //Checkbox para deshabilitar el movimiento de la camara con el mouse (quedaria solo con las flechitas)
            GuiController.Instance.Modifiers.addBoolean("Camara", "Usar mouse", true);
        }

        public void update(float cameraSpeed)
        {
            bool mouseEnabled = (bool)GuiController.Instance.Modifiers.getValue("Camara");
            float dx = 0, dz = 0;
            int screenHeight = GuiController.Instance.Panel3d.Height;
            int screenWidth = GuiController.Instance.Panel3d.Width;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            //move up
            if (mouseEnabled && (GuiController.Instance.D3dInput.Ypos <= 20 && GuiController.Instance.D3dInput.Ypos > -20 && Math.Abs(GuiController.Instance.D3dInput.Xpos - screenWidth / 2) < ANCHO_DESPLAZAMIENTO) || d3dInput.keyDown(Key.UpArrow))
            {
                dz = cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move down
            if (mouseEnabled && (GuiController.Instance.D3dInput.Ypos >= screenHeight - 20 && GuiController.Instance.D3dInput.Ypos < screenHeight && Math.Abs(GuiController.Instance.D3dInput.Xpos - screenWidth / 2) < ANCHO_DESPLAZAMIENTO) || d3dInput.keyDown(Key.DownArrow))
            {
                dz = -cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move left
            if (mouseEnabled && (GuiController.Instance.D3dInput.Xpos <= 20 && GuiController.Instance.D3dInput.Xpos > 0 && Math.Abs(GuiController.Instance.D3dInput.Ypos - screenHeight / 2) < ANCHO_DESPLAZAMIENTO) || d3dInput.keyDown(Key.LeftArrow))
            {
                dx = -cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            //move right
            if (mouseEnabled && (GuiController.Instance.D3dInput.Xpos >= screenWidth - 20 && GuiController.Instance.D3dInput.Xpos < screenWidth && Math.Abs(GuiController.Instance.D3dInput.Ypos - screenHeight / 2) < ANCHO_DESPLAZAMIENTO) || d3dInput.keyDown(Key.RightArrow))
            {
                dx = cameraSpeed * GuiController.Instance.ElapsedTime;
            }

            Vector3 desplazamiento = new Vector3(dx, 0, dz);
            GuiController.Instance.ThirdPersonCamera.Target += desplazamiento;
        }


    }
}
