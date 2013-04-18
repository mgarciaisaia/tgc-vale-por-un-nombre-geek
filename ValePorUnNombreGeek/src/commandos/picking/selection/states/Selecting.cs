using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states
{
    class Selecting : SelectionState
    {
        private Vector3 initSelectionPoint;
        private Vector3 lastSelectionPoint;
        private TgcBox selectionBox;
        private const float SELECTION_BOX_HEIGHT = 75;

        public Selecting(MultipleSelection _selection, Terrain _terrain, Vector3 _initSelectionPoint)
            : base(_selection, _terrain)
        {
            this.initSelectionPoint = _initSelectionPoint;
            this.lastSelectionPoint = Vector3.Empty;

            //Inicializamos la selectionBox
            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(110, Color.CadetBlue);
        }



        public override void update()
        {
            Vector3 pointA = this.initSelectionPoint;

            //verificamos que el rayo halla variado su posicion. si no, volver a calcular todo es al pedo.
            //Vector3 pointB = TerrainPickingRaySingleton.Instance.getRayGroundIntersection(this.terrain); //usamos getRayGroundIntersection por que es MUCHO mas rapido que getRayIntersection
            //if (!GeneralMethods.isCloseTo(pointB, this.lastSelectionPoint))
            //{
                //this.lastSelectionPoint = pointB; //guardamos la nueva posicion para en el proximo render volver a comparar
            Vector3 pointB;
                if (TerrainPickingRaySingleton.Instance.terrainIntersection(this.terrain, out pointB))
                {
                    float selectionBoxHeight = Math.Max(pointA.Y, pointB.Y) + SELECTION_BOX_HEIGHT;

                    pointA.Y = 0;
                    pointB.Y = 0;

                    Vector3 min = Vector3.Minimize(pointA, pointB);
                    Vector3 max = Vector3.Maximize(pointA, pointB);
                    min.Y = 0;
                    max.Y = selectionBoxHeight;

                    this.selectionBox.setExtremes(min, max);
                    this.selectionBox.updateValues();
                }
            //}

            this.selectionBox.render();


            if (GuiController.Instance.D3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                if(GeneralMethods.isCloseTo(pointA, pointB)){
                    this.selection.selectCharactersByRay(TerrainPickingRaySingleton.Instance.Ray);
                }
                else
                {
                    this.selection.selectCharactersInBox(this.selectionBox);
                }
                this.selectionBox.dispose();
                this.selection.setState(new Waiting(this.selection, this.terrain));
            }
        }
    }
}
