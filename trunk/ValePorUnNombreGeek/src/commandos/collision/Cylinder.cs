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
        private int renderColor;

        private const int END_CAPS_RESOLUTION = 30; //cantidad de lineas por cada tapa
        private CustomVertex.PositionColored[] endCapsVertex; //vertices de las tapas
        private CustomVertex.PositionColored[] bordersVertex; //vertices de los bordes
        

        //TODO emprolijar toda la clase! la hice "a las chapas"


        public Cylinder(Vector3 _base, float _height, float _radius)
        {
            this.radius = _radius;
            this.halfHeight = new Vector3(0, _height / 2, 0);
            this.center = _base + this.halfHeight;
            this.renderColor = Color.Yellow.ToArgb();
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

        public void setColor(Color _color)
        {
            this.renderColor = _color.ToArgb();
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
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA + this.halfHeight + this.center, this.renderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB + this.halfHeight + this.center, this.renderColor);
                //tapa inferior
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA - this.halfHeight + this.center, this.renderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB - this.halfHeight + this.center, this.renderColor);
            }

            this.updateBordersDraw();
        }

        private void updateDrawColor()
        {
            for (int i = 0; i < this.endCapsVertex.Count(); i++)
            {
                this.endCapsVertex[i].Color = this.renderColor;
            }
        }

        private void updateBordersDraw()
        {
            Vector3 cameraSeen = GuiController.Instance.CurrentCamera.getPosition() - this.center;
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, this.halfHeight);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            Vector3 puntoA = this.center + this.halfHeight + transversalALaCamara;
            Vector3 puntoB = this.center - this.halfHeight + transversalALaCamara;

            bordersVertex[0] = new CustomVertex.PositionColored(puntoA, this.renderColor);
            bordersVertex[1] = new CustomVertex.PositionColored(puntoB, this.renderColor);

            puntoA = this.center + this.halfHeight - transversalALaCamara;
            puntoB = this.center - this.halfHeight - transversalALaCamara;

            bordersVertex[2] = new CustomVertex.PositionColored(puntoA, this.renderColor);
            bordersVertex[3] = new CustomVertex.PositionColored(puntoB, this.renderColor);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.updateBordersDraw();
            this.updateDrawColor();
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, endCapsVertex.Length / 2, endCapsVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, bordersVertex.Length / 2, bordersVertex);
        }

        public void dispose()
        {
            this.endCapsVertex = null;
            this.bordersVertex = null;
        }

        #endregion

        public bool thereIsCollision(Cylinder collider, out Vector3 n)
        {
            //nota: no se checkea la altura, debido a la naturaleza del juego
            Vector3 distance = collider.Center - this.Center;
            distance.Y = 0;
            if (distance.Length() <= this.radius + collider.radius)
            {
                distance = this.Center - collider.Center;
                n = Vector3.Cross(this.halfHeight, distance);
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

        public bool thereIsCollision(TgcBoundingBox aabb, out Vector3 n)
        {
            //nota: no se checkea la altura, debido a la naturaleza del juego
            if (funesMori(aabb))
            {
                Vector3 boxDimensions = aabb.calculateSize() * 0.5f;
                Vector2 boxDimensions2d = new Vector2(boxDimensions.X, boxDimensions.Z);

                Vector3 boxCenter = aabb.Position + boxDimensions;
                Vector3 centerToCenter = new Vector3(this.Center.X - boxCenter.X, 0, this.Center.Z - boxCenter.Z);
                Vector2 centerToCenter2d = new Vector2(FastMath.Abs(centerToCenter.X), FastMath.Abs(centerToCenter.Z));

                float cross = Vector2.Ccw(centerToCenter2d, boxDimensions2d);

                if (cross > 0) n = new Vector3(centerToCenter.X, 0, 0);
                else if (cross < 0) n = new Vector3(0, 0, centerToCenter.Z);
                else
                {
                    n = new Vector3(centerToCenter.X * boxDimensions2d.Y, 0, centerToCenter.Z * boxDimensions2d.X);
                }
                n.Normalize();
                return true;
            }
            else
            {
                n = Vector3.Empty;
                return false;
            }
        }

        private bool funesMori(TgcBoundingBox aabb) //TODO rename D:
        {
            Vector3 boxDimensions = aabb.calculateSize() * 0.5f;
            Vector3 boxCenter = aabb.Position + boxDimensions;

            Vector3 centerToCenter;
            centerToCenter.X = FastMath.Abs(this.Center.X - boxCenter.X);
            centerToCenter.Z = FastMath.Abs(this.Center.Z - boxCenter.Z);

            //vemos si esta muy lejos
            if (centerToCenter.X > (boxDimensions.X + this.radius)) return false;
            if (centerToCenter.Z > (boxDimensions.Z + this.radius)) return false;

            //vemos si esta dentro del aabb
            if (centerToCenter.X <= boxDimensions.X) return true;
            if (centerToCenter.Z <= boxDimensions.Z) return true;

            //vemos si toca una esquina
            float cornerDistance = FastMath.Pow2(centerToCenter.X - boxDimensions.X) + FastMath.Pow2(centerToCenter.Z - boxDimensions.Z);
            return cornerDistance <= FastMath.Pow2(this.radius);
        }
    }
}
