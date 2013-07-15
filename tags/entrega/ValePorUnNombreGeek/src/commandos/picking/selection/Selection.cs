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
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    class Selection
    {
        private List<SelectionMethod> selectionMethods;
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

            this.selectionMethods = new List<SelectionMethod>();
            this.selectionMethods.Add(new ScreenProjection(this.selectableCharacters));
            this.selectionMethods.Add(new BoxSelection(_terrain, this.selectableCharacters));
            GuiController.Instance.Modifiers.addInterval("Seleccion", this.selectionMethods.ToArray(), 0);
        }

        /// <summary>
        /// Cancela la seleccion si se estaba seleccionando
        /// </summary>
        public void cancelSelection()
        {
            this.selecting = false;
        }

        /// <summary>
        /// Quita de la seleccion todos los personajes
        /// </summary>
        public void deselectAllCharacters()
        {
            foreach (Character ch in this.selectedCharacters) ch.Selected = false;
            this.selectedCharacters.Clear();
        }

        /// <summary>
        /// Si no se esta presionando la tecla Shift, quita de la seleccion todos los personajes
        /// </summary>
        public void deselectIfNotShift()
        {
            if (!CommandosUI.Instance.keyDown(Key.LeftShift))
                this.deselectAllCharacters();
        }

        /// <summary>
        /// Agrega un personaje a la seleccion
        /// </summary>
        public void addSelectedCharacter(Character ch)
        {
            ch.Selected = true;
            this.selectedCharacters.Add(ch);
        }

        /// <summary>
        /// Agrega o quita un personaje a la seleccion, dependiendo de si ya estaba seleccionado
        /// </summary>
        public void selectCharacter(Character ch)
        {
            if (ch.Selected)
            {
                ch.Selected = false;
                this.selectedCharacters.Remove(ch);
            }
            else
            {
                ch.Selected = true;
                this.selectedCharacters.Add(ch);
            }
        }

        #region Update

        /// <summary>
        /// Actualiza la lista de personajes seleccionados, utilizando un metodo de seleccion predefinido
        /// </summary>
        public void update()
        {
            var ui = CommandosUI.Instance;

            this.selectionMethod = (SelectionMethod)GuiController.Instance.Modifiers.getValue("Seleccion");

            if (!this.selecting && ui.mouseDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //arranca a seleccionar
                if (this.selectionMethod.canBeginSelection())
                {
                    this.selectionMethod.updateSelection();
                    this.lastMousePos = ui.ViewportMousePos;
                    this.initMousePos = ui.ViewportMousePos;
                    this.selecting = true;
                }
            }
            else if (this.selecting && ui.mouseUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //termina de seleccionar
                this.deselectIfNotShift();

                Vector2 actualMousePos = ui.ViewportMousePos;

                if (GeneralMethods.isCloseTo(actualMousePos, this.initMousePos, 1))
                { //seleccion simple
                    this.selectCharacterByRay();
                }
                else
                { //seleccion multiple
                    List<Character> newSelectedCharacters = this.selectionMethod.endAndRetSelection();
                    foreach (Character ch in newSelectedCharacters) this.addSelectedCharacter(ch);
                }

                this.selecting = false;
            }
            else if (this.selecting)
            { //esta seleccionando
                Vector2 actualMousePos = ui.ViewportMousePos;
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
        /// Selecciona un unico personaje utilizando el picking ray
        /// </summary>
        private void selectCharacterByRay()
        {
            List<Character> candidateCharacters = new List<Character>();
            PickingRaySingleton.Instance.updateRayByMouse();

            //buscamos los personajes que colisionan con el rayo
            foreach (Character ch in this.selectableCharacters)
                if (ch.collidesWith(PickingRaySingleton.Instance.Ray))
                    candidateCharacters.Add(ch);

            //si hicieron click en la nada...
            if (candidateCharacters.Count == 0) return;

            //buscamos el personaje mas cercano a la camara
            Character closestCharacter = candidateCharacters[0];
            float closestDistance = (CommandosUI.Instance.Camera.getPosition() - closestCharacter.Position).LengthSq();

            for (int i = 1; i < candidateCharacters.Count; i++)
            {
                Character ch = candidateCharacters[i];

                float distance = (CommandosUI.Instance.Camera.getPosition() - ch.Position).LengthSq();
                if (distance < closestDistance)
                {
                    closestCharacter = ch;
                    closestDistance = distance;
                }
            }

            this.selectCharacter(closestCharacter);
        }

        #endregion
    }
}
