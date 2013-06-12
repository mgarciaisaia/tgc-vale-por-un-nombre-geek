using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.methods;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    class Selection
    {
        private SelectionMethod selectionMethod;
        private List<Character> selectableCharacters;
        private List<Character> selectedCharacters;
        private bool selecting;
        private Vector2 initMousePos;
        private Vector2 lastMousePos;

        public Selection(List<Character> _selectableCharacters, ITerrain _terrain)
        {
            this.selectableCharacters = _selectableCharacters;
            this.selectedCharacters = new List<Character>();
            this.selecting = false;
            //this.selectionMethod = new BoxSelection(_terrain, this.selectableCharacters);
            this.selectionMethod = new ScreenProjection(this.selectableCharacters);
        }

        /// <summary>
        /// Cancela la seleccion si se estaba seleccionando
        /// </summary>
        public void cancelSelection()
        {
            this.selecting = false;
        }

        #region Update

        /// <summary>
        /// Actualiza la lista de personajes seleccionados, utilizando un metodo de seleccion predefinido
        /// </summary>
        public void update()
        {
            var ui = CommandosUI.Instance;

            if (!this.selecting && ui.mouseDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //arranca a seleccionar
                if (this.selectionMethod.canBeginSelection())
                {
                    this.selectionMethod.updateSelection();
                    this.lastMousePos = ui.MousePosition;
                    this.initMousePos = ui.MousePosition;
                    this.selecting = true;
                }
            }
            else if (this.selecting && ui.mouseUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //termina de seleccionar
                if (!ui.keyDown(Key.LeftShift))
                {
                    foreach (Character ch in this.selectedCharacters) ch.Selected = false;
                    this.selectedCharacters.Clear();
                }

                Vector2 actualMousePos = ui.MousePosition;

                if (GeneralMethods.isCloseTo(actualMousePos, this.initMousePos, 1))
                { //seleccion simple
                    this.selectCharacterByRay();
                }
                else
                { //seleccion multiple
                    List<Character> newSelectedCharacters = this.selectionMethod.endAndRetSelection();
                    foreach (Character ch in newSelectedCharacters) ch.Selected = true;
                    this.selectedCharacters.AddRange(newSelectedCharacters);
                }

                this.selecting = false;
            }
            else if (this.selecting)
            { //esta seleccionando
                Vector2 actualMousePos = ui.MousePosition;
                if(!GeneralMethods.isCloseTo(actualMousePos, this.lastMousePos, 1))
                {
                    this.lastMousePos = actualMousePos;
                    this.selectionMethod.updateSelection();
                }
                this.selectionMethod.renderSelection();
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// Retorna los personajes seleccionados
        /// </summary>
        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        #endregion

        #region SingleSelection

        /// <summary>
        /// Selecciona un personaje utilizando el picking ray
        /// </summary>
        private void selectCharacterByRay()
        {
            foreach (Character ch in this.selectableCharacters)
            {
                PickingRaySingleton.Instance.updateRayByMouse();
                if(ch.collidesWith(PickingRaySingleton.Instance.Ray))
                {
                    ch.Selected = true;
                    this.selectedCharacters.Add(ch);
                    break;
                }
            }
        }

        #endregion
    }
}
