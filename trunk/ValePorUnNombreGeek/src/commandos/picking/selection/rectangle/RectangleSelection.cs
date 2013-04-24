using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle.states;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle
{
    class RectangleSelection : Selection
    {
        private SelectionState state;
        private List<Character> selectedCharacters;
        //private List<Character> selectableCharacters;

        public RectangleSelection()
        {
            this.state = new Waiting(this);
            this.selectedCharacters = new List<Character>();
        }

        public void setState(SelectionState _state)
        {
            this.state = _state;
        }

        public void update()
        {
            this.state.update();
        }

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        public void deselectAllCharacters()
        {
            this.selectedCharacters.Clear();
        }

        internal void selectCharactersInRectangle(Rectangle rectangle)
        {
            //TODO
        }
    }
}
