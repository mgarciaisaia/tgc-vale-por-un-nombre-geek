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
        private List<Character> selectableCharacters;

        public RectangleSelection(List<Character> _selectableCharacters)
        {
            this.state = new Waiting(this);
            this.selectedCharacters = new List<Character>();
            this.selectableCharacters = _selectableCharacters;
        }

        #region State

        public void update()
        {
            this.state.update();
        }

        public void setState(SelectionState _state)
        {
            this.state = _state;
        }

        #endregion

        #region SelectedCharacterList

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        public void deselectAllCharacters()
        {
            foreach (Character ch in this.selectedCharacters)
                ch.Selected = false;
            this.selectedCharacters.Clear();
        }

        private void addSelectedCharacter(Character ch)
        {
            this.selectedCharacters.Add(ch);
            ch.Selected = true;
        }

        #endregion

        internal void selectCharactersInRectangle(Rectangle rectangle)
        {
            foreach (Character ch in this.selectableCharacters)
            {
                Rectangle characterRectangle = ch.BoundingBox().projectToScreen();
                if (rectangle.IntersectsWith(characterRectangle))
                    this.addSelectedCharacter(ch);
            }
        }
    }
}
