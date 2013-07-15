using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands.orders
{
    class StandBy : CommandoOrder
    {
        public StandBy(List<Character> _characters)
            : base(_characters)
        {
            //nothing to do
        }

        public override string description
        {
            get { return "Quietito"; }
        }

        public override void execute()
        {
            foreach (Character ch in this.getCommandos())
            {
                ch.abortMove();
            }
        }
    }
}
