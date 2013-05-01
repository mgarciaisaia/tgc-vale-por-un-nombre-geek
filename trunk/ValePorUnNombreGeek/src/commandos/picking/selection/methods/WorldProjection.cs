using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle
{
    class WorldProjection : ScreenProjection
    {
        private Terrain terrain;

        public WorldProjection(Terrain _terrain, List<Character> _selectableCharacters)
            : base(_selectableCharacters)
        {
            this.terrain = _terrain;
        }

        private List<Character> getCharactersInBox(TgcBox _selectionBox)
        {
            List<Character> ret = new List<Character>();

            foreach (Character ch in this.selectableCharacters)
                if (TgcCollisionUtils.testAABBAABB(_selectionBox.BoundingBox, ch.BoundingBox))
                    ret.Add(ch);

            return ret;
        }

        #region Update

        private Vector3 initTerrainPoint;
        public override bool canBeginSelection()
        {
            PickingRaySingleton.Instance.updateRayByMouse();
            if (!PickingRaySingleton.Instance.terrainIntersection(this.terrain, out this.initTerrainPoint))
                return false;

            return base.canBeginSelection();
        }


        private Vector3 actualTerrainPoint;
        public override void updateSelection()
        {
            Vector3 intersectionPoint;
            PickingRaySingleton.Instance.updateRayByMouse();
            if (PickingRaySingleton.Instance.terrainIntersection(this.terrain, out intersectionPoint))
                this.actualTerrainPoint = intersectionPoint;
            else
                return;

            base.updateSelection();
        }


        public override List<Character> endAndRetSelection()
        {
            Vector3 boxSize = new Vector3(this.max.X - this.min.X, 300, this.max.Y - this.min.Y);

            PickingRaySingleton.Instance.updateRayByPos(this.min.X, this.min.Y);
            Vector3 minPoint = PickingRaySingleton.Instance.getRayGroundIntersection(this.terrain);
            PickingRaySingleton.Instance.updateRayByPos(this.max.X, this.max.Y);
            Vector3 maxPoint = PickingRaySingleton.Instance.getRayGroundIntersection(this.terrain);

            Vector3 boxCenter = (maxPoint - minPoint) * 0.5f + minPoint;

            TgcBox selectionBox = TgcBox.fromSize(boxSize);
            selectionBox.Position = boxCenter;


            PickingRaySingleton.Instance.updateRayByMouse();
            Vector3 direction = PickingRaySingleton.Instance.Ray.Direction;
            direction.Normalize();

            float angleX = FastMath.Acos(Vector3.Dot(new Vector3(0, 0, 1), direction));
            selectionBox.rotateX(angleX + 0.5f * FastMath.PI);

            float angleZ = FastMath.Acos(Vector3.Dot(new Vector3(1, 0, 0), direction));
            selectionBox.rotateZ(angleZ + 0.5f * FastMath.PI);


            //daniela, help! estoy estancado!

            ThingsToRender.getInstace().boxes.Add(selectionBox);
            return this.getCharactersInBox(selectionBox);
        }

        #endregion
    }
}