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
        private float halfLength;
        private Vector3 halfHeight;
        private float radius;

        private Vector3 rotation;
        private Matrix transform;

        public BoundingCylinder(Vector3 _center, float _radius, float _halfLength)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfLength = _halfLength;

            this.transform = Matrix.Identity;
            this.rotation = new Vector3(0, 0, 0);
            this.AutoTransformEnable = true;

            this.updateValues();
        }

        public void updateValues()
        {
            this.halfHeight = new Vector3(0, this.halfLength, 0);
            Matrix rotationMatrix = Matrix.RotationYawPitchRoll(this.rotation.Y, this.rotation.X, this.rotation.Z);
            this.halfHeight.TransformNormal(rotationMatrix);
        }

        #region Rendering

        private const int END_CAPS_RESOLUTION = 15;
        private const int END_CAPS_VERTEX_COUNT = 2 * (END_CAPS_RESOLUTION * 2);
        private CustomVertex.PositionColored[] vertices; //line list

        /// <summary>
        /// Actualiza la posicion de los vertices que componen las tapas.
        /// </summary>
        private void updateDraw()
        {
            if (this.vertices == null)
                this.vertices = new CustomVertex.PositionColored[END_CAPS_VERTEX_COUNT + 4]; //4 para los bordes laterales

            int color = Color.Yellow.ToArgb();

            //matriz que vamos a usar para girar el vector de dibujado
            float delta = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;
            Vector3 upVector = new Vector3(0, this.halfLength, 0);
            Matrix rotationMatrix = Matrix.RotationAxis(upVector, delta);

            //vector de dibujado
            Vector3 n = new Vector3(this.radius, 0, 0);

            //dibujado de los bordes de las tapas
            for (int i = 0; i < END_CAPS_VERTEX_COUNT / 2; i += 2)
            {
                //vertice inicial
                this.vertices[i] = new CustomVertex.PositionColored(upVector + n, color);
                this.vertices[END_CAPS_VERTEX_COUNT / 2 + i] = new CustomVertex.PositionColored(-upVector + n, color);

                //rotamos el vector de dibujado
                n.TransformNormal(rotationMatrix);

                //vertice final
                this.vertices[i + 1] = new CustomVertex.PositionColored(upVector + n, color);
                this.vertices[END_CAPS_VERTEX_COUNT / 2 + i + 1] = new CustomVertex.PositionColored(-upVector + n, color);
            }

            //rotamos y trasladamos los vertices
            Matrix transformation = this.Transform;
            for (int i = 0; i < END_CAPS_VERTEX_COUNT; i++)
                this.vertices[i].Position = Vector3.TransformCoordinate(this.vertices[i].Position, transformation);
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

            //actualizamos los vertices de las tapas
            this.updateDraw(); //TODO solo actualizar si se movio el centro o hh
            //actualizamos los vertices de las lineas laterales
            this.updateBordersDraw();

            //dibujamos
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, this.vertices.Length / 2, this.vertices);
        }

        public void dispose()
        {
            this.vertices = null;
        }

        public bool AlphaBlendEnable { get; set; } //useless?

        #endregion


        public Matrix Transform
        {
            get
            {
                if (this.AutoTransformEnable)
                    return Matrix.RotationYawPitchRoll(this.rotation.Y, this.rotation.X, this.rotation.Z) * Matrix.Translation(this.center);
                else
                    return this.transform;
            }
            set
            {
                this.transform = value;
            }
        }

        public bool AutoTransformEnable { get; set; }

        public Vector3 Position
        {
            get { return this.center; }
            set { this.center = value; }
        }

        public Vector3 Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        public Vector3 Scale //TODO
        {
            get { return new Vector3(1, 1, 1); }
            set { ; }
        }

        public void move(Vector3 v)
        {
            this.move(v.X, v.Y, v.Z);
        }

        public void move(float x, float y, float z)
        {
            this.center.X += x;
            this.center.Y += y;
            this.center.Z += z;
        }

        public void moveOrientedY(float movement) //TODO
        {
            throw new NotImplementedException();
        }

        public void getPosition(Vector3 pos)
        {
            pos.X = this.center.X;
            pos.Y = this.center.Y;
            pos.Z = this.center.Z;
        }

        public void rotateX(float angle)
        {
            this.rotation.X += angle;
        }

        public void rotateY(float angle)
        {
            this.rotation.Y += angle;
        }

        public void rotateZ(float angle)
        {
            this.rotation.Z += angle;
        }
    }
}
