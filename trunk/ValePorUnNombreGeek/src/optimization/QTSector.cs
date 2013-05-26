using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QTSector
    {
        private TerrainPatch tp;
        private List<ILevelObject> objs;

        public QTSector(TerrainPatch _tp)
        {
            this.tp = _tp;
            this.objs = new List<ILevelObject>();
        }

        public void addObjectIfCollides(ILevelObject obj)
        {
            if (obj.collidesWith(this.tp.BoundingBox))
                this.objs.Add(obj);
        }

        public bool collidesWithFrustum(TgcFrustum frustum)
        {
            TgcCollisionUtils.FrustumResult result = TgcCollisionUtils.classifyFrustumAABB(frustum, this.tp.BoundingBox);
            if (result != TgcCollisionUtils.FrustumResult.OUTSIDE) return true;
            return false;
        }

        public List<ILevelObject> Objects { get { return this.objs; } }
        public TerrainPatch TerrainPatch { get { return this.tp; } }
    }
}
