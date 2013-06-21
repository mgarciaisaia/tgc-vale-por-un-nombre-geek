using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class CommandButton : Picture, IButton
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
