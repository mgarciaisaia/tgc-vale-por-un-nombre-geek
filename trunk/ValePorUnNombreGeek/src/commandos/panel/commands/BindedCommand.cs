using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands
{
    class BindedCommand
    {
        private Command command;
        private Key key;

        internal BindedCommand(Command _command, Key _key)
        {
            this.key = _key;
            this.command = _command;
        }
        
        public void checkForPressedKey()
        {
            if (GuiController.Instance.D3dInput.keyPressed(this.key))
                this.command.execute();
        }
    }
}
