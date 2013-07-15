using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    abstract class AABBObject : ILevelObject
    {
        public abstract Vector3 Position { get; set; }

        public abstract Vector3 Center { get; }

        public abstract float Radius { get; }

        public abstract Effect Effect { get; set; }

        public abstract string Technique { get; set; }

        public abstract void render();

        public abstract void dispose();

        public abstract TgcBoundingBox BoundingBox { get; }

        public bool collidesWith(Character ch, out Vector3 n)
        {
            return ch.collidesWith(this.BoundingBox, out n);
        }

        public bool collidesWith(TgcBoundingBox aabb)
        {
            return TgcCollisionUtils.testAABBAABB(aabb, this.BoundingBox);
        }

        public bool collidesWith(TgcRay ray)
        {
            Vector3 q; //useless
            return TgcCollisionUtils.intersectRayAABB(ray, this.BoundingBox, out q);
        }

        public bool isOver(Vector3 _position)
        {
            TgcBox marcaDePosicion = new TgcBox();
            marcaDePosicion = TgcBox.fromSize(_position, new Vector3(10, 10, 10));
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(this.BoundingBox, marcaDePosicion.BoundingBox);
            return result != TgcCollisionUtils.BoxBoxResult.Afuera;
        }
    }
}
