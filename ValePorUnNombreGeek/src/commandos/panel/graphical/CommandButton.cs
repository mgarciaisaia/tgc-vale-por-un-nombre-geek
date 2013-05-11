using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class CommandButton : Sprite
    {
        private Command command;

        public CommandButton(Command _command, string _filePath)
            : base(_filePath)
        {
            this.command = _command;
        }

        public void click()
        {
            this.command.execute();
        }
    }
}
