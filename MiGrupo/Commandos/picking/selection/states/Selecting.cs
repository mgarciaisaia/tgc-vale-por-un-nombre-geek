using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking.selection.states
{
    class Selecting : SelectionState
    {
        private Vector3 initSelectionPoint;
        private TgcBox selectionBox;
        private const float SELECTION_BOX_HEIGHT = 75;

        public Selecting(MultipleSelection _selection, Terrain _terrain, Vector3 _initSelectionPoint)
            : base(_selection, _terrain)
        {
            this.initSelectionPoint = _initSelectionPoint;

            //Inicializamos la selectionBox
            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(110, Color.CadetBlue);
        }



        public override void update()
        {
            PickingRayHome.getInstance().updateRay();
            Vector3 pointA = this.initSelectionPoint;
            Vector3 pointB = PickingRayHome.getInstance().getRayIntersection(this.terrain);
            float selectionBoxHeight = Math.Max(pointA.Y, pointB.Y) + SELECTION_BOX_HEIGHT;

            pointA.Y = 0;
            pointB.Y = 0;

            Vector3 min = Vector3.Minimize(pointA, pointB);
            Vector3 max = Vector3.Maximize(pointA, pointB);
            min.Y = 0;
            max.Y = selectionBoxHeight;

            this.selectionBox.setExtremes(min, max);
            this.selectionBox.updateValues();

            this.selectionBox.render();


            if (GuiController.Instance.D3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                this.selection.selectCharactersInBox(this.selectionBox);
                this.selectionBox.dispose();
                this.selection.setState(new Waiting(this.selection, this.terrain));
            }
        }
    }
}
