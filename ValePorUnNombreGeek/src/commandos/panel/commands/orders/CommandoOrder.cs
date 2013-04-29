using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands.orders
{
    abstract class CommandoOrder : Command
    {
        private List<Character> characters;

        public CommandoOrder(List<Character> _characters)
        {
            this.characters = _characters;
        }

        protected List<Character> getCommandos()
        {
            List<Character> commandos = this.characters.Where(p => p.userCanMove()).ToList();
            return commandos; //TODO que retorne una lista de Commandos
        }

        public abstract string description
        {
            get;
        }

        public abstract void execute();
    }
}
