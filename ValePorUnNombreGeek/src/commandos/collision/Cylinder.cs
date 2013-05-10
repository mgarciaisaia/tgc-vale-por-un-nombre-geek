using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.collision
{
    class Cylinder
    {
        private Vector3 center;
        private float radius;
        private Vector3 halfHeight;
        private int borderColor;

        private const int END_CAPS_RESOLUTION = 30; //cantidad de lineas por cada tapa
        private CustomVertex.PositionColored[] endCapsVertex; //vertices de las tapas
        private CustomVertex.PositionColored[] bordersVertex; //vertices de los bordes


        public Cylinder(Vector3 _center, float _halfHeight, float _radius)
            : this(_center, _halfHeight, _radius, Color.Yellow)
        {
            //nothing to do
        }

        public Cylinder(Vector3 _center, float _halfHeight, float _radius, Color _color)
        {
            this.radius = _radius;
            this.halfHeight = new Vector3(0, _halfHeight, 0);
            this.center = _center;
            this.borderColor = _color.ToArgb();
            this.updateDraw();
        }

        public Vector3 Position
        {
            get { return this.Center - this.halfHeight; }
            set { this.Center = value + this.halfHeight; }
        }

        public Vector3 Center
        {
            get { return this.center; }
            set
            {
                this.center = value;
                this.updateDraw();
            }
        }

        public float HalfHeight
        {
            get { return this.halfHeight.Y; }
            set { this.halfHeight.Y = value; }
        }

        #region Draw

        public Color Color
        {
            get { return Color.FromArgb(this.borderColor); }
            set { this.borderColor = value.ToArgb(); this.updateDraw(); }
        }

        private void updateDraw()
        {
            if (endCapsVertex == null)
            {
                int verticesCount = (END_CAPS_RESOLUTION * 2 + 2) * 3;
                verticesCount = verticesCount * 2; //por las dos tapas
                this.endCapsVertex = new CustomVertex.PositionColored[verticesCount];
                this.bordersVertex = new CustomVertex.PositionColored[4]; //bordes laterales
            }

            float step = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;
            int index = 0;

            for (float a = 0f; a <= FastMath.TWO_PI; a += step)
            {
                Vector3 tapaPuntoA = new Vector3(FastMath.Cos(a) * this.radius, 0, FastMath.Sin(a) * this.radius);
                Vector3 tapaPuntoB = new Vector3(FastMath.Cos(a + step) * this.radius, 0, FastMath.Sin(a + step) * this.radius);
                //tapa superior
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA + this.halfHeight + this.center, this.borderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB + this.halfHeight + this.center, this.borderColor);
                //tapa inferior
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA - this.halfHeight + this.center, this.borderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB - this.halfHeight + this.center, this.borderColor);
            }
            //bordes laterales
            this.updateBordersDraw();
        }

        private void updateBordersDraw()
        {
            Vector3 cameraSeen = GuiController.Instance.CurrentCamera.getPosition() - this.center;
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, this.halfHeight);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            Vector3 puntoA = this.center + this.halfHeight + transversalALaCamara;
            Vector3 puntoB = this.center - this.halfHeight + transversalALaCamara;

            bordersVertex[0] = new CustomVertex.PositionColored(puntoA, this.borderColor);
            bordersVertex[1] = new CustomVertex.PositionColored(puntoB, this.borderColor);

            puntoA = this.center + this.halfHeight - transversalALaCamara;
            puntoB = this.center - this.halfHeight - transversalALaCamara;

            bordersVertex[2] = new CustomVertex.PositionColored(puntoA, this.borderColor);
            bordersVertex[3] = new CustomVertex.PositionColored(puntoB, this.borderColor);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.updateBordersDraw();
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, endCapsVertex.Length / 2, endCapsVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, bordersVertex.Length / 2, bordersVertex);
        }

        public void dispose()
        {
            this.endCapsVertex = null;
            this.bordersVertex = null;
        }

        #endregion

        public bool thereIsCollisionCyCy(Cylinder collider, out Vector3 n)
        {
            Vector3 distance = collider.Center - this.Center;
            if (FastMath.Pow2(distance.X) + FastMath.Pow2(distance.Z) <= FastMath.Pow2(this.radius + collider.radius))
            {
                n = Vector3.Cross(distance, this.halfHeight);
                n = Vector3.Cross(n, this.halfHeight);
                n.Normalize();
                return true;
            }
            else
            {
                n = Vector3.Empty;
                return false;
            }
        }

        public bool thereIsCollisionCyBB(TgcBoundingBox aabb, out Vector3 n)
        {
            n = Vector3.Empty;

            Vector3 boxDimensions = aabb.calculateSize() * 0.5f;
            Vector3 boxCenter = aabb.Position + boxDimensions;

            Vector3 centerToCenter = new Vector3(this.Position.X - boxCenter.X, 0, this.Position.Z - boxCenter.Z);
            Vector3 absCenterToCenter = new Vector3();
            absCenterToCenter.X = FastMath.Abs(centerToCenter.X);
            absCenterToCenter.Z = FastMath.Abs(centerToCenter.Z);

            //vemos si esta muy lejos
            if (absCenterToCenter.X > (boxDimensions.X + this.radius)) return false;
            if (absCenterToCenter.Z > (boxDimensions.Z + this.radius)) return false;

            //vemos si esta dentro del aabb
            if (absCenterToCenter.X <= boxDimensions.X) goto celculateNormal;
            if (absCenterToCenter.Z <= boxDimensions.Z) goto celculateNormal;

            //vemos si toca una esquina
            float cornerDistance = FastMath.Pow2(absCenterToCenter.X - boxDimensions.X) + FastMath.Pow2(absCenterToCenter.Z - boxDimensions.Z);
            if (cornerDistance <= FastMath.Pow2(this.radius)) goto celculateNormal;

            return false;

        celculateNormal:
            Vector2 boxDimensions2d = fromVector3(boxDimensions);
            Vector2 centerToCenter2d = fromVector3(absCenterToCenter);

            float cross = Vector2.Ccw(centerToCenter2d, boxDimensions2d);

            if (cross > 0) n = new Vector3(centerToCenter.X, 0, 0);
            else if (cross < 0) n = new Vector3(0, 0, centerToCenter.Z);
            else n = new Vector3(centerToCenter.X * boxDimensions2d.Y, 0, centerToCenter.Z * boxDimensions2d.X);
            n.Normalize();
            return true;
        }

        private Vector2 fromVector3(Vector3 v3)
        {
            return new Vector2(v3.X, v3.Z);
        }
    }
}
