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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.methods
{
    class BoxSelection : SelectionMethod
    {
        private List<Character> selectableCharacters;
        private ITerrain terrain;
        private TgcBox selectionBox;

        private const float SELECTION_BOX_HEIGHT = 75;


        public BoxSelection(ITerrain _terrain, List<Character> _selectableCharacters)
        {
            this.selectableCharacters = _selectableCharacters;
            this.terrain = _terrain;

            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(110, Color.CadetBlue);
        }

        private List<Character> getCharactersInBox(TgcBox _selectionBox)
        {
            List<Character> ret = new List<Character>();
            Vector3 n; //useless

            foreach (Character ch in this.selectableCharacters)
                if (ch.BoundingCylinder.thereIsCollisionCyBB(_selectionBox.BoundingBox, out n))
                    ret.Add(ch);

            return ret;
        }

        #region Update

        private Vector3 initTerrainPoint;
        public bool canBeginSelection()
        {
            PickingRaySingleton.Instance.updateRayByMouse();
            return PickingRaySingleton.Instance.terrainIntersection(this.terrain, out this.initTerrainPoint);
        }

        public void updateSelection()
        {
            Vector3 terrainPointB;
            PickingRaySingleton.Instance.updateRayByMouse();
            if (!PickingRaySingleton.Instance.terrainIntersection(this.terrain, out terrainPointB)) return;
            Vector3 terrainPointA = this.initTerrainPoint;

            float selectionBoxHeight = FastMath.Max(terrainPointA.Y, terrainPointB.Y) + SELECTION_BOX_HEIGHT;

            terrainPointA.Y = 0;
            terrainPointB.Y = 0;

            Vector3 min = Vector3.Minimize(terrainPointA, terrainPointB);
            Vector3 max = Vector3.Maximize(terrainPointA, terrainPointB);
            min.Y = this.terrain.minY;
            max.Y = selectionBoxHeight;

            this.selectionBox.setExtremes(min, max);
            this.selectionBox.updateValues();
        }

        public void renderSelection()
        {
            this.selectionBox.render();
        }

        public List<Character> endAndRetSelection()
        {
            return this.getCharactersInBox(this.selectionBox);
        }

        #endregion
    }
}
