using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;

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

            this.controlPanelSprite = new Sprite(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Sprites\\Barrita.jpg", GuiController.Instance.D3dDevice);

            Rectangle drawingArea = new Rectangle(0, 0, this.controlPanelSprite.ImageInformation.Width, this.controlPanelSprite.ImageInformation.Height);
            this.controlPanelSprite.RegionRectangle = drawingArea;
            this.controlPanelSprite.Position = new Vector2(0, screenHeight - this.controlPanelSprite.ImageInformation.Height);
            this.controlPanelSprite.Scale = new Vector2(((screenWidth / 900f) * 0.9f), 1f);

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
    }
}
