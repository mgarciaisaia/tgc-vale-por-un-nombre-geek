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

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    class MultipleSelection
    {
        SelectionState state;
        List<Character> selectedCharacters;
        List<Character> selectableCharacters;

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

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }

        public void setState(SelectionState _state)
        {
            this.state = _state;
        }

        public void selectCharactersByRay(TgcRay _ray)
        {
            this.selectedCharacters.Clear();
            foreach (Character ch in this.selectableCharacters)
            {
                Vector3 collisionPoint; //useless
                if (TgcCollisionUtils.intersectRayAABB(_ray, ch.BoundingBox(), out collisionPoint))
                {
                    this.selectedCharacters.Add(ch);
                    ch.Selected = true;
                }
                else
                {
                    ch.Selected = false;
                }
            }
        }

        public void selectCharactersInBox(TgcBox _selectionBox)
        {
            this.selectedCharacters.Clear();
            foreach (Character ch in this.selectableCharacters)
            {
                //Colisión de AABB entre área de selección y el modelo
                if (TgcCollisionUtils.testAABBAABB(_selectionBox.BoundingBox, ch.BoundingBox()))
                {
                    this.selectedCharacters.Add(ch);
                    ch.Selected = true;
                }
                else
                {
                    ch.Selected = false;
                }
            }
        }
    }
}
