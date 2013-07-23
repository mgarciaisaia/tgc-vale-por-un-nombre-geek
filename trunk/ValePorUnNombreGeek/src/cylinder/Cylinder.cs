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

        private float halfLength;
        private float radius;
        private int color;
        private BoundingCylinder boundingCylinder;


        private CustomVertex.PositionColored[] topCapsVertices; //line strip
        private CustomVertex.PositionColored[] bottomCapsVertices; //line strip

        private CustomVertex.PositionColored[] sideTrianglesVertices; //triangle strip
        private CustomVertex.PositionColored[] capsTrianglesVertices; //triangle list


        public Cylinder(Vector3 _center, float _radius, float _halfLength)
        {
            this.radius = _radius;
            this.halfLength = _halfLength;
            this.boundingCylinder = new BoundingCylinder(_center, _radius, _halfLength);

            //this.color = Color.Red.ToArgb();
            this.color = Color.FromArgb(150, Color.Red).ToArgb();

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

            this.updateValues();
        }

        private void updateDraw()
        {
            //vectores utilizados para el dibujado
            //Vector3 upVector = new Vector3(0, this.halfLength, 0);
            //Vector3 n = new Vector3(this.radius, 0, 0);
            Vector3 upVector = new Vector3(0, 1, 0);
            Vector3 n = new Vector3(1, 0, 0);

            int capsResolution = this.topCapsVertices.Length;

            //matriz de rotacion del vector de dibujado
            float angleStep = FastMath.TWO_PI / (float)capsResolution;
            Matrix rotationMatrix = Matrix.RotationAxis(upVector, angleStep);

            //transformacion que se le aplicara a cada vertice
            Matrix transformation = this.Transform;

            for (int i = 0; i < capsResolution; i++)
            {
                //establecemos los vertices de las tapas
                Vector3 topCapPoint = Vector3.TransformCoordinate(upVector + n, transformation);
                this.topCapsVertices[i] = new CustomVertex.PositionColored(topCapPoint, this.color);
                Vector3 bottomCapPoint = Vector3.TransformCoordinate(-upVector + n, transformation);
                this.bottomCapsVertices[i] = new CustomVertex.PositionColored(bottomCapPoint, this.color);

                //triangulos de la cara lateral (strip)
                this.sideTrianglesVertices[2 * i] = this.topCapsVertices[i];
                this.sideTrianglesVertices[2 * i + 1] = this.bottomCapsVertices[i];

                //triangulos de la tapa superior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i] = this.topCapsVertices[i - 1];
                this.capsTrianglesVertices[3 * i + 1] = this.topCapsVertices[i];
                topCapPoint = Vector3.TransformCoordinate(upVector, transformation);
                this.capsTrianglesVertices[3 * i + 2] = new CustomVertex.PositionColored(topCapPoint, Color.White.ToArgb());

                //triangulos de la tapa inferior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i + 3 * capsResolution] = this.bottomCapsVertices[i - 1];
                this.capsTrianglesVertices[3 * i + 1 + 3 * capsResolution] = this.bottomCapsVertices[i];
                bottomCapPoint = Vector3.TransformCoordinate(-upVector, transformation);
                this.capsTrianglesVertices[3 * i + 2 + 3 * capsResolution] = new CustomVertex.PositionColored(bottomCapPoint, Color.White.ToArgb());

                //rotamos el vector de dibujado
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
            this.boundingCylinder.dispose();
        }

        public Color Color {
            get { return Color.FromArgb(this.color); }
            set { this.color = value.ToArgb(); }
        }

        public bool AlphaBlendEnable { get; set; }

        #region Transformation

        public bool AutoTransformEnable {
            get { return this.BoundingCylinder.AutoTransformEnable; }
            set { this.BoundingCylinder.AutoTransformEnable = value; }
        }

        public Matrix Transform
        {
            get { return this.BoundingCylinder.Transform; }
            set { this.BoundingCylinder.Transform = value; }
        }

        public Vector3 Position
        {
            get { return this.BoundingCylinder.Position; }
            set { this.BoundingCylinder.Position = value; }
        }

        public Vector3 Rotation
        {
            get { return this.BoundingCylinder.Rotation; }
            set { this.BoundingCylinder.Rotation = value; }
        }

        public Vector3 Scale
        {
            get { return this.BoundingCylinder.Scale; }
            set { this.BoundingCylinder.Scale = value; }
        }

        public void move(Vector3 v)
        {
            this.BoundingCylinder.move(v);
        }

        public void move(float x, float y, float z)
        {
            this.BoundingCylinder.move(x, y, z);
        }

        public void moveOrientedY(float movement)
        {
            this.BoundingCylinder.moveOrientedY(movement);
        }

        public void getPosition(Vector3 pos)
        {
            this.BoundingCylinder.getPosition(pos);
        }

        public void rotateX(float angle)
        {
            this.BoundingCylinder.rotateX(angle);
        }

        public void rotateY(float angle)
        {
            this.BoundingCylinder.rotateY(angle);
        }

        public void rotateZ(float angle)
        {
            this.BoundingCylinder.rotateZ(angle);
        }

        #endregion

        public BoundingCylinder BoundingCylinder { get { return this.boundingCylinder; } }

        public void updateValues()
        {
            this.boundingCylinder.updateValues();
            this.updateDraw();
        }

        public float Height
        {
            get { return this.boundingCylinder.Height; }
            set { this.boundingCylinder.Height = value; }
        }

        public float Radius
        {
            get { return this.boundingCylinder.Radius; }
            set { this.boundingCylinder.Radius = value; }
        }
    }
}
