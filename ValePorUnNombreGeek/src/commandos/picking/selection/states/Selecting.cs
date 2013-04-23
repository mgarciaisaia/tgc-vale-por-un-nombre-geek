using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states
{
    class Selecting : SelectionState
    {
        private Vector3 initTerrainPoint;
        private Vector3 initGroundPoint;
        //private Vector3 lastTerrainPoint;
        private Vector3 lastGroundPoint;

        private TgcBox selectionBox;

        private const float SELECTION_BOX_HEIGHT = 75;


        public Selecting(MultipleSelection _selection, Terrain _terrain, Vector3 _initTerrainPoint, Vector3 _initGroundPoint)
            : base(_selection, _terrain)
        {
            this.initTerrainPoint = _initTerrainPoint;
            this.initGroundPoint = _initGroundPoint;
            this.lastGroundPoint = _initGroundPoint;

            //Inicializamos la selectionBox
            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(110, Color.CadetBlue);
        }

        private void calculateSelectionBox()
        {
            Vector3 actualGroundPoint = PickingRaySingleton.Instance.getRayGroundIntersection(this.terrain);
            if (GeneralMethods.isCloseTo(actualGroundPoint, this.lastGroundPoint, 1)) return; else this.lastGroundPoint = actualGroundPoint;

            Vector3 terrainPointB;
            if(!PickingRaySingleton.Instance.terrainIntersection(this.terrain, out terrainPointB)) return;
            Vector3 terrainPointA = this.initTerrainPoint;

            float selectionBoxHeight = Math.Max(terrainPointA.Y, terrainPointB.Y) + SELECTION_BOX_HEIGHT;

            terrainPointA.Y = 0;
            terrainPointB.Y = 0;

            Vector3 min = Vector3.Minimize(terrainPointA, terrainPointB);
            Vector3 max = Vector3.Maximize(terrainPointA, terrainPointB);
            min.Y = 0;
            max.Y = selectionBoxHeight;

            this.selectionBox.setExtremes(min, max);
            this.selectionBox.updateValues();
        }

        public override void update()
        {
            this.calculateSelectionBox();
            this.selectionBox.render();

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            if (d3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                if(!d3dInput.keyDown(Key.LeftShift)) this.selection.deselectAllCharacters();
                if(GeneralMethods.isCloseTo(this.initGroundPoint, this.lastGroundPoint, 1))
                    this.selection.selectCharactersByRay(PickingRaySingleton.Instance.Ray);
                else
                    this.selection.selectCharactersInBox(this.selectionBox);

                this.selectionBox.dispose();
                this.selection.setState(new Waiting(this.selection, this.terrain));
            }
        }
    }
}
