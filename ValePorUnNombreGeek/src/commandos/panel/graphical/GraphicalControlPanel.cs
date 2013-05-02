using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class GraphicalControlPanel
    {
        private SpriteDrawer drawer;
        private Sprite controlPanelSprite;

        public GraphicalControlPanel()
        {
            this.controlPanelSprite = new Sprite("fruta", GuiController.Instance.D3dDevice);
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
