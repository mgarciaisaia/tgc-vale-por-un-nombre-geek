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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    class MultipleSelection
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

        public void update()
        {
            this.state.update();
        }

        internal void setState(SelectionState _state)
        {
            this.state = _state;
        }

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        private void addSelectedCharacter(Character ch)
        {
            this.selectedCharacters.Add(ch);
        }

        public void deselectAllCharacters()
        {
            foreach (Character ch in this.selectedCharacters)
            {
                ch.Selected = false;
            }
            this.selectedCharacters.Clear();
        }

        public void selectCharactersByRay(TgcRay _ray)
        {
            //this.deselectAllCharacters();
            foreach (Character ch in this.selectableCharacters)
            {
                Vector3 collisionPoint; //useless
                if (TgcCollisionUtils.intersectRayAABB(_ray, ch.BoundingBox(), out collisionPoint))
                {
                    this.addSelectedCharacter(ch);
                    ch.Selected = true;
                    break;
                }
            }
        }

        public void selectCharactersInBox(TgcBox _selectionBox)
        {
            //this.deselectAllCharacters();
            foreach (Character ch in this.selectableCharacters)
            {
                //Colisión de AABB entre área de selección y el modelo
                if (TgcCollisionUtils.testAABBAABB(_selectionBox.BoundingBox, ch.BoundingBox()))
                {
                    this.addSelectedCharacter(ch);
                    ch.Selected = true;
                }
            }
        }
    }
}
