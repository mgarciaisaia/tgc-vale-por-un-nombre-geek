using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    class BoundingCylinder : IRenderObject, ITransformObject
    {
        private Vector3 center;
        private Vector3 halfHeight;
        private float radius;

        private const int END_CAPS_RESOLUTION = 15;
        private const int END_CAPS_VERTEX_COUNT = 2 * (END_CAPS_RESOLUTION * 2);
        private CustomVertex.PositionColored[] vertices; //line list

        public BoundingCylinder(Vector3 _center, float _radius, Vector3 _halfHeight)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfHeight = _halfHeight;

            this.initialize();
        }

        private void initialize()
        {
            this.vertices = new CustomVertex.PositionColored[END_CAPS_VERTEX_COUNT + 4]; //4 para los bordes laterales
            this.updateDraw();
        }

        /// <summary>
        /// Actualiza la posicion de los vertices que componen las tapas.
        /// </summary>
        private void updateDraw()
        {
            int color = Color.Yellow.ToArgb();

            float delta = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;
            Matrix rotationMatrix = Matrix.RotationAxis(this.halfHeight, delta);

            Vector3 n = Vector3.Cross(this.halfHeight, new Vector3(0, 1, 0));

            //dibujado de los bordes de las tapas
            for (int i = 0; i < END_CAPS_VERTEX_COUNT / 2; i += 2)
            {
                this.vertices[i] = new CustomVertex.PositionColored(this.center + this.halfHeight + n, color);
                this.vertices[END_CAPS_VERTEX_COUNT / 2 + i] = new CustomVertex.PositionColored(this.center - this.halfHeight + n, color);

                n.TransformNormal(rotationMatrix);

                this.vertices[i + 1] = new CustomVertex.PositionColored(this.center + this.halfHeight + n, color);
                this.vertices[END_CAPS_VERTEX_COUNT / 2 + i + 1] = new CustomVertex.PositionColored(this.center - this.halfHeight + n, color);
            }
        }

        /// <summary>
        /// Actualiza la posicion de los cuatro vertices que componen los lados.
        /// </summary>
        private void updateBordersDraw()
        {
            Vector3 cameraSeen = GuiController.Instance.CurrentCamera.getPosition() - this.center;
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, this.halfHeight);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            int color = Color.Yellow.ToArgb();
            int firstBorderVertex = END_CAPS_VERTEX_COUNT;

            this.vertices[firstBorderVertex] = new CustomVertex.PositionColored(this.center + this.halfHeight + transversalALaCamara, color);
            this.vertices[firstBorderVertex + 1] = new CustomVertex.PositionColored(this.center - this.halfHeight + transversalALaCamara, color);

            this.vertices[firstBorderVertex + 2] = new CustomVertex.PositionColored(this.center + this.halfHeight - transversalALaCamara, color);
            this.vertices[firstBorderVertex + 3] = new CustomVertex.PositionColored(this.center - this.halfHeight - transversalALaCamara, color);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.updateBordersDraw();
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, this.vertices.Length / 2, this.vertices);
        }

        public void dispose()
        {
            this.vertices = null;
        }

        public bool AlphaBlendEnable
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


        public Matrix Transform
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void move(float x, float y, float z)
        {
            throw new NotImplementedException();
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
