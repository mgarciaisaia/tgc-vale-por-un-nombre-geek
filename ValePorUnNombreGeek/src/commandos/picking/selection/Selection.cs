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
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

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

        public Selection(List<Character> _selectableCharacters, Terrain _terrain)
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
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (!this.selecting && input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //arranca a seleccionar
                if (this.selectionMethod.canBeginSelection())
                {
                    this.selectionMethod.updateSelection();
                    this.lastMousePos = new Vector2(input.Xpos, input.Ypos);
                    this.initMousePos = new Vector2(input.Xpos, input.Ypos);
                    this.selecting = true;
                }
            }
            else if (this.selecting && input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            { //termina de seleccionar
                if (!input.keyDown(Key.LeftShift))
                {
                    foreach (Character ch in this.selectedCharacters) ch.Selected = false;
                    this.selectedCharacters.Clear();
                }

                Vector2 actualMousePos = new Vector2(input.Xpos, input.Ypos);

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
                Vector2 actualMousePos = new Vector2(input.Xpos, input.Ypos);
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
                Vector3 collisionPoint; //useless
                PickingRaySingleton.Instance.updateRayByMouse();
                if (TgcCollisionUtils.intersectRayAABB(PickingRaySingleton.Instance.Ray, ch.BoundingBox, out collisionPoint))
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
