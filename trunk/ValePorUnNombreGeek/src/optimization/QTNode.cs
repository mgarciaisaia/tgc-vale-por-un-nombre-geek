using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    abstract class QTNode
    {
        private TgcBoundingBox aabb;

        public void setBounds(Vector3 pmin, Vector3 pmax)
        {
            this.aabb = new TgcBoundingBox(pmin, pmax);
        }

        public bool seenByFrustum(TgcFrustum frustum)
        {
            TgcCollisionUtils.FrustumResult result = TgcCollisionUtils.classifyFrustumAABB(frustum, this.aabb);
            return result != TgcCollisionUtils.FrustumResult.OUTSIDE;
        }

        public TgcBoundingBox BoundingBox { get { return this.aabb; } }

        //abstract List<ILevelObject> getObjectsInside();
    }
}
