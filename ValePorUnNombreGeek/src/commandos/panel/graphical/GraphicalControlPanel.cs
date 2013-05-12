using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class GraphicalControlPanel
    {
        private SpriteDrawer drawer;
        private Sprite controlPanelSprite;
        private List<CommandButton> buttons;

        public GraphicalControlPanel()
        {
            int screenHeight = GuiController.Instance.D3dDevice.Viewport.Height;
            int screenWidth = GuiController.Instance.D3dDevice.Viewport.Width;

            this.controlPanelSprite = new Sprite(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Sprites\\panelMadera.png");

            //Rectangle drawingArea = new Rectangle(0, 0, this.controlPanelSprite.ImageInformation.Width, this.controlPanelSprite.ImageInformation.Height);
            //this.controlPanelSprite.RegionRectangle = drawingArea;
            this.controlPanelSprite.Position = new Vector2(0, screenHeight - this.controlPanelSprite.ImageInformation.Height);
            this.controlPanelSprite.Scale = new Vector2(screenWidth / (1.0f * this.controlPanelSprite.ImageInformation.Width), 1f);

            buttons = new List<CommandButton>();

            this.drawer = new SpriteDrawer();
            this.drawer.addSpriteToDraw(this.controlPanelSprite);
        }

        private void addButton(CommandButton _button)
        {
            this.buttons.Add(_button);
            this.drawer.addSpriteToDraw(_button);

            float X = this.controlPanelSprite.Width / 10;
            foreach (CommandButton button in this.buttons)
                X = Math.Max(X, (int)button.Position.X + button.Width + 5);

            Vector2 pos = new Vector2(X, this.controlPanelSprite.Position.Y + this.controlPanelSprite.Height / 2 - _button.Height / 2);
            _button.Position = pos;
        }

        public void addCommand(Command _command, string _picPath)
        {
            this.addButton(new CommandButton(_command, _picPath));
        }

        private bool mouseIsOverCommand(float mouseX, float mouseY, CommandButton command)
        {
            float spriteX = command.Position.X;
            float spriteY = command.Position.Y;
            float spriteWidth = command.Width;
            float spriteHeight = command.Height;

            return mouseX > spriteX && mouseX < spriteX + spriteWidth
                && mouseY > spriteY && mouseY < spriteY + spriteHeight;
        }

        private bool commandUnderMouse(float mouseX, float mouseY, out CommandButton ret)
        {
            foreach (CommandButton button in this.buttons)
                if (this.mouseIsOverCommand(mouseX, mouseY, button))
                {
                    ret = button;
                    return true;
                }

            ret = null;
            return false;
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
            float mouseX = GuiController.Instance.D3dInput.Xpos;
            float mouseY = GuiController.Instance.D3dInput.Ypos;
            CommandButton button;
            if (this.commandUnderMouse(mouseX, mouseY, out button))
            {
                if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    button.click();
                else
                {
                    //TODO show tooltip
                }
            }
        }
    }
}
