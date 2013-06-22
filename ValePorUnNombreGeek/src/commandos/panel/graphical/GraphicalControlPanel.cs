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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class GraphicalControlPanel
    {
        private Picture controlPanelSprite;
        private List<IButton> buttons;

        public GraphicalControlPanel(string path)
        {
            int screenHeight = CommandosUI.Instance.ScreenHeight;
            int screenWidth = CommandosUI.Instance.ScreenWidth;

            this.controlPanelSprite = new Picture(path);
            //this.controlPanelSprite.Position = new Vector2(0, screenHeight - this.controlPanelSprite.Height);
            this.controlPanelSprite.Position = new Vector2(0, 0);
            this.controlPanelSprite.Width = screenWidth;

            buttons = new List<IButton>();
        }

        private void addButton(IButton _button)
        {
            this.buttons.Add(_button);

            float X = this.controlPanelSprite.Width*8/10;
            foreach (IButton button in this.buttons)
                X = Math.Max(X, (int)button.Position.X + button.Width + 20);

            Vector2 pos = new Vector2(X, this.controlPanelSprite.Position.Y + this.controlPanelSprite.Height / 2 - _button.Height / 2);
            _button.Position = pos;
        }

        /// <summary>
        /// Agrega un boton al panel
        /// </summary>
        public void addCommand(Command _command, string _picPath)
        {
            this.addButton(new CommandButton(_command, _picPath));
        }

        /// <summary>
        /// Agrega un boton de seleccion de personaje al panel
        /// </summary>
        public void addSelectionButton(Character ch, Selection selection)
        {
            this.buttons.Add(new SelectionButton(ch, selection));
        }

        /// <summary>
        /// Indica si el mouse esta sobre determinado boton
        /// </summary>
        private bool mouseIsOverCommand(float mouseX, float mouseY, IButton command)
        {
            float spriteX = command.Position.X;
            float spriteY = command.Position.Y;
            float spriteWidth = command.Width;
            float spriteHeight = command.Height;

            return mouseX > spriteX && mouseX < spriteX + spriteWidth
                && mouseY > spriteY && mouseY < spriteY + spriteHeight;
        }

        /// <summary>
        /// Inidica si hay un boton bajo el mouse, y si es asi, lo devuelve
        /// </summary>
        private bool commandUnderMouse(float mouseX, float mouseY, out IButton ret)
        {
            foreach (IButton button in this.buttons)
                if (this.mouseIsOverCommand(mouseX, mouseY, button))
                {
                    ret = button;
                    return true;
                }

            ret = null;
            return false;
        }

        public int Height { get { return (int)this.controlPanelSprite.Height; } }
        public int Width { get { return (int)this.controlPanelSprite.Width; } }

        public void render()
        {
            this.controlPanelSprite.render();
            foreach (IButton button in this.buttons) button.render();
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
            //return CommandosUI.Instance.ScreenMousePos.Y > this.controlPanelSprite.Position.Y;
            return CommandosUI.Instance.ScreenMousePos.Y < this.controlPanelSprite.Height;
        }

        /// <summary>
        /// Metodo que se llama si el mouse esta sobre el panel
        /// </summary>
        public void update()
        {
            Vector2 mousePos = CommandosUI.Instance.ScreenMousePos;
            float mouseX = mousePos.X;
            float mouseY = mousePos.Y;
            IButton button;
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
