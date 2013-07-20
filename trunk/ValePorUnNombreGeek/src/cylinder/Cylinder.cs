using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    class Cylinder : IRenderObject, ITransformObject
    {
        private const int END_CAPS_RESOLUTION = 45;

        private Vector3 center;
        private Vector3 halfHeight;
        private float radius;
        private int color;

        private Matrix transform;

        private CustomVertex.PositionColored[] topCapsVertex; //line strip
        private CustomVertex.PositionColored[] bottomCapsVertex; //line strip

        private CustomVertex.PositionColored[] sideTrianglesVertex; //triangle strip
        private CustomVertex.PositionColored[] capsTrianglesVertex; //triangle list

        public Cylinder(Vector3 _center, float _radius, Vector3 _halfHeight)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfHeight = _halfHeight;

            this.color = Color.Red.ToArgb();
            this.transform = Matrix.Identity;

            this.initialize();
        }

        private void initialize()
        {
            int capsResolution = END_CAPS_RESOLUTION;
            this.topCapsVertex = new CustomVertex.PositionColored[capsResolution];
            this.bottomCapsVertex = new CustomVertex.PositionColored[capsResolution];

            //cara lateral: un vertice por cada vertice de cada tapa, mas dos para cerrarla
            this.sideTrianglesVertex = new CustomVertex.PositionColored[2 * capsResolution + 2];

            //tapas: dos vertices por cada vertice de cada tapa, mas uno central
            this.capsTrianglesVertex = new CustomVertex.PositionColored[capsResolution * 3 * 2];

            this.updateDraw();
        }

        private void updateDraw()
        {
            Vector3 n = Vector3.Cross(this.halfHeight, new Vector3(0, 1, 0));
            n.Normalize();
            n *= this.radius;

            int capsResolution = this.topCapsVertex.Length;

            float angleStep = FastMath.TWO_PI / (float)capsResolution;
            Matrix rotationMatrix = Matrix.RotationAxis(this.halfHeight, angleStep);

            for (int i = 0; i < capsResolution; i++)
            {
                int tmpColor = i * 255 / capsResolution; //TODO quitar esto

                //vertices de las tapas
                this.topCapsVertex[i] = new CustomVertex.PositionColored(this.center + this.halfHeight + n, tmpColor);
                this.bottomCapsVertex[i] = new CustomVertex.PositionColored(this.center - this.halfHeight + n, tmpColor);

                //triangulos de la cara lateral (strip)
                this.sideTrianglesVertex[2 * i] = this.topCapsVertex[i];
                this.sideTrianglesVertex[2 * i + 1] = this.bottomCapsVertex[i];

                //triangulos de la tapa superior (list)
                if (i > 0) this.capsTrianglesVertex[3 * i] = this.topCapsVertex[i - 1];
                this.capsTrianglesVertex[3 * i + 1] = this.topCapsVertex[i];
                this.capsTrianglesVertex[3 * i + 2] = new CustomVertex.PositionColored(this.center + this.halfHeight, Color.White.ToArgb());

                //triangulos de la tapa inferior (list)
                if (i > 0) this.capsTrianglesVertex[3 * i + 3 * capsResolution] = this.bottomCapsVertex[i - 1];
                this.capsTrianglesVertex[3 * i + 1 + 3 * capsResolution] = this.bottomCapsVertex[i];
                this.capsTrianglesVertex[3 * i + 2 + 3 * capsResolution] = new CustomVertex.PositionColored(this.center - this.halfHeight, Color.White.ToArgb());

                n.TransformNormal(rotationMatrix);
            }

            this.sideTrianglesVertex[2 * capsResolution] = this.topCapsVertex[0];
            this.sideTrianglesVertex[2 * capsResolution + 1] = this.bottomCapsVertex[0];

            this.capsTrianglesVertex[0] = this.topCapsVertex[capsResolution - 1];
            this.capsTrianglesVertex[3 * capsResolution] = this.bottomCapsVertex[capsResolution - 1];
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            int capsResolution = this.topCapsVertex.Length;

            d3dDevice.DrawUserPrimitives(PrimitiveType.LineStrip, capsResolution - 1, this.topCapsVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineStrip, capsResolution - 1, this.bottomCapsVertex);

            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2 * capsResolution, this.sideTrianglesVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2 * capsResolution, this.capsTrianglesVertex);
        }

        public void dispose()
        {
            this.topCapsVertex = null;
            this.bottomCapsVertex = null;
            this.sideTrianglesVertex = null;
            this.capsTrianglesVertex = null;
        }

        public bool AlphaBlendEnable { get; set; } //TODO

        public Matrix Transform
        {
            get
            {
                return this.transform;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoTransformEnable
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.center;
            }
            set
            {
                if (value == this.center) return;
                this.center = value;
                this.updateDraw();
            }
        }

        public Vector3 Rotation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector3 Scale
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void move(Vector3 v)
        {
            this.center += v;
            this.updateDraw();
        }

        public void move(float x, float y, float z)
        {
            this.move(new Vector3(x, y, z));
        }

        public void moveOrientedY(float movement)
        {
            throw new NotImplementedException();
        }

        public void getPosition(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        public void rotateX(float angle)
        {
            throw new NotImplementedException();
        }

        public void rotateY(float angle)
        {
            throw new NotImplementedException();
        }

        public void rotateZ(float angle)
        {
            throw new NotImplementedException();
        }
    }
}
