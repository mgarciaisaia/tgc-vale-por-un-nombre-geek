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

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking
{
    class MultipleSelection
    {
        private bool selecting = false;
        private Vector3 initSelectionPoint;

        TgcPickingRay pickingRay;
        TgcBox selectionBox;

        Terrain terrain;
        List<Character> selectedCharacters;
        List<Character> selectableCharacters;

        private const float SELECTION_BOX_HEIGHT = 75;


        public MultipleSelection(Terrain _terrain, List<Character> _selectableCharacters)
        {
            //Crear caja para marcar en que lugar hubo colision
            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(150, Color.Green);

            //Iniciarlizar PickingRay
            this.pickingRay = new TgcPickingRay();

            this.terrain = _terrain;
            this.selectedCharacters = new List<Character>();
            this.selectableCharacters = _selectableCharacters;
        }

        public void update()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;


            //Si hacen clic con el mouse, ver si hay colision con el suelo
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Detectar nuevo punto de colision con el piso
                PickingRayHome.getInstance().updateRay();


                //primera vez
                if (!selecting)
                {
                    this.initSelectionPoint = PickingRayHome.getInstance().getRayIntersection(this.terrain);
                    selecting = true;
                }
                //Si se está seleccionado, generar box de seleccion
                else
                {
                    Vector3 pointA = this.initSelectionPoint;
                    Vector3 pointB = PickingRayHome.getInstance().getRayIntersection(this.terrain);
                    float selectionBoxHeight = Math.Max(pointA.Y, pointB.Y) + SELECTION_BOX_HEIGHT;

                    pointA.Y = 0;
                    pointB.Y = 0;

                    Vector3 min = Vector3.Minimize(pointA, pointB);
                    Vector3 max = Vector3.Maximize(pointA, pointB);
                    min.Y = 0;
                    max.Y = selectionBoxHeight;

                    //Configurar BOX
                    selectionBox.setExtremes(min, max);
                    selectionBox.updateValues();

                    selectionBox.render();
                }
            }




            //Solto el clic del mouse, terminar la selección
            if (GuiController.Instance.D3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                selecting = false;
                this.selectedCharacters.Clear();

                //Ver que modelos quedaron dentro del area de selección seleccionados
                foreach (Character ch in this.selectableCharacters)
                {
                    //Colisión de AABB entre área de selección y el modelo
                    if (TgcCollisionUtils.testAABBAABB(this.selectionBox.BoundingBox, ch.BoundingBox()))
                    {
                        this.selectedCharacters.Add(ch);
                        ch.drawBoundingBox = true;
                    }
                    else
                    {
                        ch.drawBoundingBox = false;
                    }
                }
            }
        }

        public void dispose()
        {
            selectionBox.dispose();
        }

        public List<Character> getSelectedCharacters()
        {
            return this.selectedCharacters;
        }
    }
}
