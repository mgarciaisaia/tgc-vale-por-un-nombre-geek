using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands;
using TgcViewer.Utils._2D;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.text
{
    class TextControlPanel
    {
        private TgcText2d text;
        private bool active;
        private List<BindedCommand> bindedCommands;

        public TextControlPanel()
        {
            this.active = false;
            this.bindedCommands = new List<BindedCommand>();

            this.text = new TgcText2d();
            this.text.Align = TgcText2d.TextAlign.LEFT;
            this.text.Position = new Point(2, 100);
            this.text.Size = new Size(300, 300);
            this.text.Color = Color.LightPink;
            this.text.Text = "Panel de control\n(no gráfico por el momento :p)\nOpciones >>\n\n";
        }

        public void addCommand(Command _command, Key _key)
        {
            this.bindedCommands.Add(new BindedCommand(_command, _key));
            this.text.Text += _key.ToString() + " - " + _command.description + "\n";
        }

        public void render()
        {
            if (GuiController.Instance.D3dInput.keyPressed(Key.Tab)) this.active = !this.active;

            if (this.active)
            {
                text.render();
                foreach (BindedCommand bc in this.bindedCommands)
                    bc.checkForPressedKey();
            }
        }
    }
}
