using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class GraphicalControlPanel
    {
        private Picture controlPanelSprite;
        private List<CommandButton> buttons;

        public GraphicalControlPanel(string path)
        {
            int screenHeight = CommandosUI.Instance.ViewportHeight;
            int screenWidth = CommandosUI.Instance.ViewportWidth;

            this.controlPanelSprite = new Picture(path);
            this.controlPanelSprite.Position = new Vector2(0, screenHeight - this.controlPanelSprite.Height);
            this.controlPanelSprite.Width = screenWidth;

            buttons = new List<CommandButton>();
        }

        private void addButton(CommandButton _button)
        {
            this.buttons.Add(_button);

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
            this.controlPanelSprite.render();
            foreach (CommandButton button in this.buttons) button.render();
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
            return CommandosUI.Instance.MousePosition.Y > this.controlPanelSprite.Position.Y;
        }

        /// <summary>
        /// Metodo que se llama si el mouse esta sobre el panel
        /// </summary>
        public void update()
        {
            Vector2 mousePos = CommandosUI.Instance.MousePosition;
            float mouseX = mousePos.X;
            float mouseY = mousePos.Y;
            CommandButton button;
            if (this.commandUnderMouse(mouseX, mouseY, out button))
            {
                if (CommandosUI.Instance.mousePressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    button.click();
                else
                {
                    //TODO show tooltip
                }
            }
        }
    }
}
