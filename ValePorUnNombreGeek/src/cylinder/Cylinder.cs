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
        private const int END_CAPS_RESOLUTION = 30;

        private Vector3 center;
        private Vector3 halfHeight;
        private float radius;
        private int color;

        private Matrix transform;

        private BoundingCylinder boundingCylinder;

        private CustomVertex.PositionColored[] topCapsVertices; //line strip
        private CustomVertex.PositionColored[] bottomCapsVertices; //line strip

        private CustomVertex.PositionColored[] sideTrianglesVertices; //triangle strip
        private CustomVertex.PositionColored[] capsTrianglesVertices; //triangle list

        public Cylinder(Vector3 _center, float _radius, Vector3 _halfHeight)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfHeight = _halfHeight;

            //this.color = Color.Red.ToArgb();
            this.color = Color.FromArgb(150, Color.Red).ToArgb();
            this.transform = Matrix.Identity;

            this.boundingCylinder = new BoundingCylinder(this.center, this.radius, this.halfHeight);

            this.initialize();
        }

        private void initialize()
        {
            int capsResolution = END_CAPS_RESOLUTION;
            this.topCapsVertices = new CustomVertex.PositionColored[capsResolution];
            this.bottomCapsVertices = new CustomVertex.PositionColored[capsResolution];

            //cara lateral: un vertice por cada vertice de cada tapa, mas dos para cerrarla
            this.sideTrianglesVertices = new CustomVertex.PositionColored[2 * capsResolution + 2];

            //tapas: dos vertices por cada vertice de cada tapa, mas uno en el centro
            this.capsTrianglesVertices = new CustomVertex.PositionColored[capsResolution * 3 * 2];

            this.updateDraw();
        }

        private void updateDraw()
        {
            Vector3 n = Vector3.Cross(this.halfHeight, new Vector3(0, 1, 0));
            n.Normalize();
            n *= this.radius;

            int capsResolution = this.topCapsVertices.Length;

            float angleStep = FastMath.TWO_PI / (float)capsResolution;
            Matrix rotationMatrix = Matrix.RotationAxis(this.halfHeight, angleStep);

            for (int i = 0; i < capsResolution; i++)
            {
                //vertices de las tapas
                this.topCapsVertices[i] = new CustomVertex.PositionColored(this.center + this.halfHeight + n, this.color);
                this.bottomCapsVertices[i] = new CustomVertex.PositionColored(this.center - this.halfHeight + n, this.color);

                //triangulos de la cara lateral (strip)
                this.sideTrianglesVertices[2 * i] = this.topCapsVertices[i];
                this.sideTrianglesVertices[2 * i + 1] = this.bottomCapsVertices[i];

                //triangulos de la tapa superior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i] = this.topCapsVertices[i - 1];
                this.capsTrianglesVertices[3 * i + 1] = this.topCapsVertices[i];
                this.capsTrianglesVertices[3 * i + 2] = new CustomVertex.PositionColored(this.center + this.halfHeight, Color.White.ToArgb());

                //triangulos de la tapa inferior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i + 3 * capsResolution] = this.bottomCapsVertices[i - 1];
                this.capsTrianglesVertices[3 * i + 1 + 3 * capsResolution] = this.bottomCapsVertices[i];
                this.capsTrianglesVertices[3 * i + 2 + 3 * capsResolution] = new CustomVertex.PositionColored(this.center - this.halfHeight, Color.White.ToArgb());

                n.TransformNormal(rotationMatrix);
            }

            //cerramos la cara lateral
            this.sideTrianglesVertices[2 * capsResolution] = this.topCapsVertices[0];
            this.sideTrianglesVertices[2 * capsResolution + 1] = this.bottomCapsVertices[0];

            //cerramos la tapa superior
            this.capsTrianglesVertices[0] = this.topCapsVertices[capsResolution - 1];

            //Cerramos la tapa inferior
            this.capsTrianglesVertices[3 * capsResolution] = this.bottomCapsVertices[capsResolution - 1];
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (this.AlphaBlendEnable)
            {
                d3dDevice.RenderState.AlphaBlendEnable = true;
                d3dDevice.RenderState.AlphaTestEnable = true;
            }

            int capsResolution = this.topCapsVertices.Length;

            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2 * capsResolution, this.sideTrianglesVertices);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2 * capsResolution, this.capsTrianglesVertices);

            d3dDevice.RenderState.AlphaTestEnable = false;
            d3dDevice.RenderState.AlphaBlendEnable = false;
        }

        public void dispose()
        {
            this.topCapsVertices = null;
            this.bottomCapsVertices = null;
            this.sideTrianglesVertices = null;
            this.capsTrianglesVertices = null;
        }

        public BoundingCylinder BoundingCylinder { get { return this.boundingCylinder; } }

        public bool AlphaBlendEnable { get; set; }

        public bool AutoTransformEnable { get; set; }

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
