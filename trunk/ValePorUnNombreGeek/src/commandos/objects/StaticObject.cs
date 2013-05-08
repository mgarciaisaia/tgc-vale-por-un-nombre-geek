using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.collision;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    abstract class AABBObject : ILevelObject
    {
        public abstract Vector3 Position { get; }

        public abstract Vector3 Center { get; }

        public abstract float Radius { get; }

        public abstract Effect Effect { get; set; }

        public abstract string Technique { get; set; }

        public abstract void render();

        public abstract void dispose();

        public abstract TgcBoundingBox BoundingBox
        {
            get;
        }

        public bool collidesWith(Cylinder cyl, out Vector3 n)
        {
            return cyl.thereIsCollision(this.BoundingBox, out n);
        }
    }
}
