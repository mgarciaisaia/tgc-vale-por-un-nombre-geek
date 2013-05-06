using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.cilindro
{
    class Cylinder
    {
        private Vector3 center;
        private float radius;
        private Vector3 halfLength;
        private int renderColor;

        private const int END_CAPS_RESOLUTION = 30; //cantidad de lineas por cada tapa
        private CustomVertex.PositionColored[] endCapsVertex; //vertices de las tapas
        private CustomVertex.PositionColored[] bordersVertex; //vertices de los bordes

        public Cylinder(Vector3 _center, float _halfLength, float _radius)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfLength = new Vector3(0, _halfLength, 0);
            this.renderColor = Color.Yellow.ToArgb();
            this.updateDraw();
        }

        public Vector3 Position
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
            get { return this.halfLength.Y; }
            set { this.halfLength.Y = value; }
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
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA + this.halfLength + this.center, this.renderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB + this.halfLength + this.center, this.renderColor);
                //tapa inferior
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoA - this.halfLength + this.center, this.renderColor);
                endCapsVertex[index++] = new CustomVertex.PositionColored(tapaPuntoB - this.halfLength + this.center, this.renderColor);
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
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, this.halfLength);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            Vector3 puntoA = this.center + this.halfLength + transversalALaCamara;
            Vector3 puntoB = this.center - this.halfLength + transversalALaCamara;

            bordersVertex[0] = new CustomVertex.PositionColored(puntoA, this.renderColor);
            bordersVertex[1] = new CustomVertex.PositionColored(puntoB, this.renderColor);

            puntoA = this.center + this.halfLength - transversalALaCamara;
            puntoB = this.center - this.halfLength - transversalALaCamara;

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
        }

        #endregion


        public bool thereIsCollisionCySp(TgcBoundingSphere sphere)
        {
            if (FastMath.Abs(this.center.Y - sphere.Center.Y) <= this.halfLength.Y)
            {
                Vector3 distance = sphere.Center - this.Position;
                distance.Y = 0;
                if (distance.Length() <= this.radius + sphere.Radius) return true;
            }
            //TODO colision tapas
            return false;
        }

        public bool thereIsCollisionCyCy(Cylinder collider)
        {
            if (FastMath.Abs(this.Position.Y - collider.Position.Y) <= collider.HalfHeight + this.HalfHeight)
            {
                Vector3 distance = collider.Position - this.Position;
                distance.Y = 0;
                if (distance.Length() <= this.radius + collider.radius) return true;
            }
            return false;
        }

        public bool thereIsCollisionCyBB(TgcBoundingBox box)
        {
            Vector3 boxDimensions = box.calculateSize() * 0.5f;
            Vector3 boxCenter = box.Position + boxDimensions;
            float height = boxDimensions.Y;

            if (FastMath.Abs(this.Position.Y - boxCenter.Y) <= boxDimensions.Y + this.HalfHeight)
                if (FastMath.Abs(this.Position.X - boxCenter.X) <= boxDimensions.X + this.radius)
                    if (FastMath.Abs(this.Position.Z - boxCenter.Z) <= boxDimensions.Z + this.radius)
                        return true;

            return false;
        }
    }
}
