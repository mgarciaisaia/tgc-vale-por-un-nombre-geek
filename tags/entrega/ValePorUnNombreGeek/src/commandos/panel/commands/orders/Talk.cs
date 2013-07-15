using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands.orders
{
    class SwitchCrouch : CommandoOrder
    {
        public SwitchCrouch(List<Character> _characters)
            : base(_characters)
        {
            //nothing to do
        }

        public override string description
        {
            get { return "Agarcharse/Pararse"; }
        }

        public override void execute()
        {
            foreach (Commando ch in this.getCommandos())
            {
                ch.switchCrouch();
            }
        }
    }
}
