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
        private float radius;
        private float halfLength;

        private Vector3 center;
        private Vector3 rotation;
        private Matrix autoTransformationMatrix;
        private Matrix manualTransformationMatrix;


        public BoundingCylinder(Vector3 _center, float _radius, float _halfLength)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfLength = _halfLength;

            //this.autoTransformationMatrix = Matrix.Identity;
            this.manualTransformationMatrix = Matrix.Identity;
            this.rotation = new Vector3(0, 0, 0);
            this.AutoTransformEnable = true;

            this.updateValues();
        }

        /// <summary>
        /// Actualiza la matriz de transformacion
        /// </summary>
        public void updateValues()
        {
            this.autoTransformationMatrix =
                Matrix.Scaling(this.radius, this.halfLength, this.radius) *
                Matrix.RotationYawPitchRoll(this.rotation.Y, this.rotation.X, this.rotation.Z) *
                Matrix.Translation(this.center);
        }

        /// <summary>
        /// Devuelve el vector HalfHeight (va del centro a la tapa superior del cilindro)
        /// </summary>
        private Vector3 HalfHeight
        {
            get
            {
                Vector3 halfHeight = new Vector3(0, this.halfLength, 0);
                halfHeight.TransformNormal(this.Transform);
                return halfHeight;
            }
        }

        #region Rendering

        private const int END_CAPS_RESOLUTION = 15;
        private const int END_CAPS_VERTEX_COUNT = 2 * (END_CAPS_RESOLUTION * 2);
        private CustomVertex.PositionColored[] vertices; //line list

        /// <summary>
        /// Actualiza la posicion de los vertices que componen las tapas
        /// </summary>
        private void updateDraw()
        {
            if (this.vertices == null)
                this.vertices = new CustomVertex.PositionColored[END_CAPS_VERTEX_COUNT + 4]; //4 para los bordes laterales

            int color = Color.Yellow.ToArgb();

            //matriz que vamos a usar para girar el vector de dibujado
            float angle = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;
            Vector3 upVector = new Vector3(0, 1, 0);
            Matrix rotationMatrix = Matrix.RotationAxis(upVector, angle);

            //vector de dibujado
            Vector3 n = new Vector3(1, 0, 0);

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
        /// Actualiza la posicion de los cuatro vertices que componen los lados
        /// </summary>
        private void updateBordersDraw()
        {
            //Device device = GuiController.Instance.D3dDevice;
            //Viewport viewport = device.Viewport;
            //Matrix world = device.Transform.World;
            //Matrix view = device.Transform.View;
            //Matrix proj = device.Transform.Projection;

            ////a es el punto mas a la izquierda de la tapa superior
            ////b es el punto mas a la izquierda de la tapa inferior
            ////c es el punto mas a la derecha de la tapa superior
            ////d es el punto mas a la derecha de la tapa inferior

            //Vector3 a, b, c, d, aProj, bProj, cProj, dProj;

            //Vector3 topCapVertexZero = this.vertices[0].Position;
            //a = topCapVertexZero;
            //c = topCapVertexZero;
            //Vector3 bottomCapVertexZero = this.vertices[END_CAPS_VERTEX_COUNT / 2].Position;
            //b = bottomCapVertexZero;
            //d = bottomCapVertexZero;
            //Vector3 topCapVertexZeroProj = Vector3.Project(topCapVertexZero, viewport, proj, view, world);
            //aProj = topCapVertexZeroProj;
            //cProj = topCapVertexZeroProj;
            //Vector3 bottomCapVertexZeroProj = Vector3.Project(bottomCapVertexZero, viewport, proj, view, world);
            //bProj = bottomCapVertexZeroProj;
            //dProj = bottomCapVertexZeroProj;

            //for (int i = 1; i < END_CAPS_VERTEX_COUNT / 2; i++)
            //{
            //    Vector3 point = Vector3.Project(this.vertices[i].Position, viewport, proj, view, world);
            //    if (point.X < aProj.X)
            //    {
            //        a = this.vertices[i].Position;
            //        aProj = point;
            //    }
            //    else if (point.X > cProj.X)
            //    {
            //        c = this.vertices[i].Position;
            //        cProj = point;
            //    }
            //}
            //for (int i = END_CAPS_VERTEX_COUNT / 2 + 1; i < END_CAPS_VERTEX_COUNT; i++)
            //{
            //    Vector3 point = Vector3.Project(this.vertices[i].Position, viewport, proj, view, world);
            //    if (point.X < bProj.X)
            //    {
            //        b = this.vertices[i].Position;
            //        bProj = point;
            //    }
            //    else if (point.X > dProj.X)
            //    {
            //        d = this.vertices[i].Position;
            //        dProj = point;
            //    }
            //}
            
            //int color = Color.Yellow.ToArgb();
            //int firstBorderVertex = END_CAPS_VERTEX_COUNT;
            //this.vertices[firstBorderVertex] = new CustomVertex.PositionColored(a, color);
            //this.vertices[firstBorderVertex + 1] = new CustomVertex.PositionColored(b, color);
            //this.vertices[firstBorderVertex + 2] = new CustomVertex.PositionColored(c, color);
            //this.vertices[firstBorderVertex + 3] = new CustomVertex.PositionColored(d, color);


            Vector3 cameraSeen = GuiController.Instance.CurrentCamera.getPosition() - this.center; //mmmm
            Vector3 upVector = new Vector3(0, 1, 0);

            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, upVector);
            transversalALaCamara.Normalize();

            int color = Color.Yellow.ToArgb();
            int firstBorderVertex = END_CAPS_VERTEX_COUNT;

            Matrix transformation = this.Transform;

            //vertice superior izquierdo
            Vector3 point = Vector3.TransformCoordinate(upVector + transversalALaCamara, transformation);
            this.vertices[firstBorderVertex] = new CustomVertex.PositionColored(point, color);

            //vertice inferior izquierdo
            point = Vector3.TransformCoordinate(-upVector + transversalALaCamara, transformation);
            this.vertices[firstBorderVertex + 1] = new CustomVertex.PositionColored(point, color);

            //vertice superior derecho
            point = Vector3.TransformCoordinate(upVector - transversalALaCamara, transformation);
            this.vertices[firstBorderVertex + 2] = new CustomVertex.PositionColored(point, color);

            //vertice inferior derecho
            point = Vector3.TransformCoordinate(-upVector - transversalALaCamara, transformation);
            this.vertices[firstBorderVertex + 3] = new CustomVertex.PositionColored(point, color);
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
                    return this.autoTransformationMatrix;
                else
                    return this.manualTransformationMatrix;
            }
            set
            {
                this.manualTransformationMatrix = value;
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

        public float Height
        {
            get { return 2 * this.halfLength; }
            set { this.halfLength = value / 2; }
        }

        public float Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }
    }
}
