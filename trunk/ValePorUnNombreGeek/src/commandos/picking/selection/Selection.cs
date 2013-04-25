using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.multiple;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    class Selection
    {
        private SelectionMethod selectionMethod;
        private List<Character> selectableCharacters;
        private List<Character> selectedCharacters;
        private bool selecting;

        public Selection(List<Character> _selectableCharacters, Terrain _terrain)
        {
            this.selectableCharacters = _selectableCharacters;
            this.selectedCharacters = new List<Character>();
            this.selecting = false;
            //this.selectionMethod = new MultipleSelection(_terrain, this.selectableCharacters);
            this.selectionMethod = new RectangleSelection(this.selectableCharacters);
        }

        public void update()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (!this.selecting && input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //arranca a seleccionar
                this.selecting = this.selectionMethod.canBeginSelection();
            }
            else if (this.selecting && input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //termina de seleccionar
                List<Character> newSelectedCharacters = this.selectionMethod.endAndRetSelection();
                if (!input.keyDown(Key.LeftShift))
                {
                    foreach (Character ch in this.selectedCharacters) ch.Selected = false;
                    this.selectedCharacters.Clear();
                }
                foreach (Character ch in newSelectedCharacters) ch.Selected = true;
                this.selectedCharacters.AddRange(newSelectedCharacters);
                this.selecting = false;
            }
            else if (this.selecting)
            { //esta seleccionando
                this.selectionMethod.renderSelection();
            }
        }

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }
    }
}
