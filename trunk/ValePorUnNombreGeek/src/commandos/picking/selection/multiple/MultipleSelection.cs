using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;
using System.Collections;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.multiple.states;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.multiple
{
    class MultipleSelection : Selection
    {
        private SelectionState state;
        private List<Character> selectedCharacters;
        private List<Character> selectableCharacters;

        public MultipleSelection(Terrain _terrain, List<Character> _selectableCharacters)
        {
            this.state = new Waiting(this, _terrain);

            this.selectedCharacters = new List<Character>();
            this.selectableCharacters = _selectableCharacters;
        }

        #region State

        public void update()
        {
            this.state.update();
        }

        internal void setState(SelectionState _state)
        {
            this.state = _state;
        }

        #endregion

        #region SelectedCharactersList

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        private void addSelectedCharacter(Character ch)
        {
            this.selectedCharacters.Add(ch);
            ch.Selected = true;
        }

        public void deselectAllCharacters()
        {
            foreach (Character ch in this.selectedCharacters)
                ch.Selected = false;
            this.selectedCharacters.Clear();
        }

        #endregion

        public void selectCharactersByRay(TgcRay _ray)
        {
            foreach (Character ch in this.selectableCharacters)
            {
                Vector3 collisionPoint; //useless
                if (TgcCollisionUtils.intersectRayAABB(_ray, ch.BoundingBox(), out collisionPoint))
                {
                    this.addSelectedCharacter(ch);
                    break;
                }
            }
        }

        public void selectCharactersInBox(TgcBox _selectionBox)
        {
            foreach (Character ch in this.selectableCharacters)
                if (TgcCollisionUtils.testAABBAABB(_selectionBox.BoundingBox, ch.BoundingBox()))
                    this.addSelectedCharacter(ch);
        }
    }
}
