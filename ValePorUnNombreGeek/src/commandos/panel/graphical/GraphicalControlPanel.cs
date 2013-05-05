using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class GraphicalControlPanel
    {
        private SpriteDrawer drawer;
        private Sprite controlPanelSprite;

        public GraphicalControlPanel()
        {
            int screenHeight = GuiController.Instance.D3dDevice.Viewport.Height;
            int screenWidth = GuiController.Instance.D3dDevice.Viewport.Width;

            this.controlPanelSprite = new Sprite(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Sprites\\panelMadera.png", GuiController.Instance.D3dDevice);

            Rectangle drawingArea = new Rectangle(0, 0, this.controlPanelSprite.ImageInformation.Width, this.controlPanelSprite.ImageInformation.Height);
            this.controlPanelSprite.RegionRectangle = drawingArea;
            this.controlPanelSprite.Position = new Vector2(0, screenHeight - this.controlPanelSprite.ImageInformation.Height);
            this.controlPanelSprite.Scale = new Vector2(screenWidth / (1.0f * this.controlPanelSprite.ImageInformation.Width), 1f);
            

            this.drawer = new SpriteDrawer();
            this.drawer.addSpriteToDraw(this.controlPanelSprite);
        }

        public void render()
        {
            this.drawer.drawSprites();
        }

        public void dispose()
        {
            this.controlPanelSprite.dispose();
        }

        /// <summary>
        /// Indica si el mouse esta sobre el panel
        /// </summary>
        public bool mouseIsOverPanel()
        {
            return GuiController.Instance.D3dInput.Ypos > this.controlPanelSprite.Position.Y;
        }

        /// <summary>
        /// Metodo que se llama si el mouse esta sobre el panel
        /// </summary>
        public void update()
        {
            if(GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT)){
                float relativeY = GuiController.Instance.D3dInput.Ypos - this.controlPanelSprite.Position.Y;
                this.click(GuiController.Instance.D3dInput.Xpos, relativeY);
            }
        }

        /// <summary>
        /// Metodo que se llama si hubo un click sobre el panel
        /// </summary>
        private void click(float panelx, float panely)
        {
            //TODO
        }
    }
}
