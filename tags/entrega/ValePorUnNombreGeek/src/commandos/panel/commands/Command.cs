using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands
{
    interface Command
    {
        string description
        {
            get;
        }
        void execute();
    }
}
