using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical;

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

        #region Viewport

        public Vector2 ScreenMousePos { get { return new Vector2(GuiController.Instance.D3dInput.Xpos, GuiController.Instance.D3dInput.Ypos); } }
        public int ScreenHeight { get { return GuiController.Instance.D3dDevice.Viewport.Height; } }
        public int ScreenWidth { get { return GuiController.Instance.D3dDevice.Viewport.Width; } }
        public bool mouseIsOverScreen()
        {
            return Mouse.isOverScreen();
        }

        public GraphicalControlPanel Panel { get; set; }

        public Vector2 ViewportMousePos { get { return this.ScreenMousePos; } }
        public int ViewportHeight { get { return this.ScreenHeight; } }
        public int ViewportWidth { get { return this.ScreenWidth; } }
        public bool mouseIsOverViewport()
        {
            return this.mouseIsOverScreen() && !this.Panel.mouseIsOverPanel();
        }

        #endregion

        #region Keyboard

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

        #endregion

        #region Mouse

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

        #endregion

        public ICamera Camera { get; set; }
        public TgcFrustum CameraFrustum { get { return GuiController.Instance.Frustum; } }

        public Microsoft.DirectX.Direct3D.Device d3dDevice { get { return GuiController.Instance.D3dDevice; } }

        public float ElapsedTime { get { return GuiController.Instance.ElapsedTime; } }

        public void log(string text)
        {
            GuiController.Instance.Logger.log(text); //TODO usar en el commandos
        }

        public string MediaDir { get { return GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\"; } }
        public string SrcDir { get { return GuiController.Instance.AlumnoEjemplosDir + "ValePorUnNombreGeek\\"; } }
        public string ShadersDir { get { return GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\"; } }
    }
}
