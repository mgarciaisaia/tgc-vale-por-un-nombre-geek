using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class CommandosUI
    {
        private static CommandosUI instance;

        private CommandosUI()
        {
            this.Camera = new TgcCameraAdapter(GuiController.Instance.CurrentCamera);
            //singleton
        }

        public static CommandosUI Instance {
            get
            {
                if (instance == null) instance = new CommandosUI();
                return instance;
            }
        }


        public ICamera Camera { get; set; }

        public int ViewportHeight { get { return GuiController.Instance.D3dDevice.Viewport.Height; } }
        public int ViewportWidth { get { return GuiController.Instance.D3dDevice.Viewport.Width; } }
        public Vector2 MousePosition { get { return Mouse.ViewportPosition; } }

        public float ElapsedTime { get { return GuiController.Instance.ElapsedTime; } }

        public bool keyDown(Key key)
        {
            return GuiController.Instance.D3dInput.keyDown(key);
        }

        public bool keyUp(Key key)
        {
            return GuiController.Instance.D3dInput.keyUp(key);
        }

        public bool keyPressed(Key key)
        {
            return GuiController.Instance.D3dInput.keyPressed(key);
        }

        public bool mouseDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons button)
        {
            return GuiController.Instance.D3dInput.buttonDown(button);
        }

        public bool mouseUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons button)
        {
            return GuiController.Instance.D3dInput.buttonUp(button);
        }

        public bool mousePressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons button)
        {
            return GuiController.Instance.D3dInput.buttonPressed(button);
        }

        public float DeltaWheelPos { get { return GuiController.Instance.D3dInput.WheelPos; } }



        public Microsoft.DirectX.Direct3D.Device d3dDevice { get { return GuiController.Instance.D3dDevice; } }
    }
}
