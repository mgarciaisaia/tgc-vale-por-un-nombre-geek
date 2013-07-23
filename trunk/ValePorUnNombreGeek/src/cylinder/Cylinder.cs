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
        private float halfLength;
        private float radius;
        private int color;
        private BoundingCylinder boundingCylinder;

        private const int END_CAPS_RESOLUTION = 1000;
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

            //cara lateral: un vertice por cada vertice de cada tapa, mas dos para cerrarla
            this.sideTrianglesVertices = new CustomVertex.PositionColored[2 * capsResolution + 2];

            //tapas: dos vertices por cada vertice de cada tapa, mas uno en el centro
            this.capsTrianglesVertices = new CustomVertex.PositionColored[capsResolution * 3 * 2];

            this.updateValues();
        }

        private void updateDraw()
        {
            //vectores utilizados para el dibujado
            Vector3 upVector = new Vector3(0, 1, 0);
            Vector3 n = new Vector3(1, 0, 0);

            int capsResolution = END_CAPS_RESOLUTION;

            //matriz de rotacion del vector de dibujado
            float angleStep = FastMath.TWO_PI / (float)capsResolution;
            Matrix rotationMatrix = Matrix.RotationAxis(upVector, angleStep);

            //transformacion que se le aplicara a cada vertice
            Matrix transformation = this.Transform;

            //arrays donde guardamos los puntos dibujados
            Vector3[] topCapDraw = new Vector3[capsResolution];
            Vector3[] bottomCapDraw = new Vector3[capsResolution];

            Vector3 topCapCenter = Vector3.TransformCoordinate(upVector, transformation);
            Vector3 bottomCapCenter = Vector3.TransformCoordinate(-upVector, transformation);

            for (int i = 0; i < capsResolution; i++)
            {
                //establecemos los vertices de las tapas
                topCapDraw[i] = Vector3.TransformCoordinate(upVector + n, transformation);
                bottomCapDraw[i] = Vector3.TransformCoordinate(-upVector + n, transformation);

                //triangulos de la cara lateral (strip)
                this.sideTrianglesVertices[2 * i] = new CustomVertex.PositionColored(topCapDraw[i], color);
                this.sideTrianglesVertices[2 * i + 1] = new CustomVertex.PositionColored(bottomCapDraw[i], color);

                //triangulos de la tapa superior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i] = new CustomVertex.PositionColored(topCapDraw[i - 1], color);
                this.capsTrianglesVertices[3 * i + 1] = new CustomVertex.PositionColored(topCapDraw[i], color);
                this.capsTrianglesVertices[3 * i + 2] = new CustomVertex.PositionColored(topCapCenter, Color.White.ToArgb());

                //triangulos de la tapa inferior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i + 3 * capsResolution] = new CustomVertex.PositionColored(bottomCapDraw[i - 1], color);
                this.capsTrianglesVertices[3 * i + 1 + 3 * capsResolution] = new CustomVertex.PositionColored(bottomCapDraw[i], color); ;
                this.capsTrianglesVertices[3 * i + 2 + 3 * capsResolution] = new CustomVertex.PositionColored(bottomCapCenter, Color.White.ToArgb());

                //rotamos el vector de dibujado
                n.TransformNormal(rotationMatrix);
            }

            //cerramos la cara lateral
            this.sideTrianglesVertices[2 * capsResolution] = new CustomVertex.PositionColored(topCapDraw[0], color);
            this.sideTrianglesVertices[2 * capsResolution + 1] = new CustomVertex.PositionColored(bottomCapDraw[0], color);

            //cerramos la tapa superior
            this.capsTrianglesVertices[0] = new CustomVertex.PositionColored(topCapDraw[capsResolution - 1], color);

            //Cerramos la tapa inferior
            this.capsTrianglesVertices[3 * capsResolution] = new CustomVertex.PositionColored(bottomCapDraw[capsResolution - 1], color);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (this.AlphaBlendEnable)
            {
                d3dDevice.RenderState.AlphaBlendEnable = true;
                d3dDevice.RenderState.AlphaTestEnable = true;
            }

            int capsResolution = END_CAPS_RESOLUTION;

            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2 * capsResolution, this.sideTrianglesVertices);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2 * capsResolution, this.capsTrianglesVertices);

            d3dDevice.RenderState.AlphaTestEnable = false;
            d3dDevice.RenderState.AlphaBlendEnable = false;
        }

        public void dispose()
        {
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
            get { return this.boundingCylinder.AutoTransformEnable; }
            set { this.boundingCylinder.AutoTransformEnable = value; }
        }

        public Matrix Transform
        {
            get { return this.boundingCylinder.Transform; }
            set { this.boundingCylinder.Transform = value; }
        }

        public Vector3 Position
        {
            get { return this.boundingCylinder.Position; }
            set { this.boundingCylinder.Position = value; }
        }

        public Vector3 Rotation
        {
            get { return this.boundingCylinder.Rotation; }
            set { this.boundingCylinder.Rotation = value; }
        }

        public Vector3 Scale
        {
            get { return this.boundingCylinder.Scale; }
            set { this.boundingCylinder.Scale = value; }
        }

        public void move(Vector3 v)
        {
            this.boundingCylinder.move(v);
        }

        public void move(float x, float y, float z)
        {
            this.boundingCylinder.move(x, y, z);
        }

        public void moveOrientedY(float movement)
        {
            this.boundingCylinder.moveOrientedY(movement);
        }

        public void getPosition(Vector3 pos)
        {
            this.boundingCylinder.getPosition(pos);
        }

        public void rotateX(float angle)
        {
            this.boundingCylinder.rotateX(angle);
        }

        public void rotateY(float angle)
        {
            this.boundingCylinder.rotateY(angle);
        }

        public void rotateZ(float angle)
        {
            this.boundingCylinder.rotateZ(angle);
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
