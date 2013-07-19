﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    class BoundingCylinder
    {
        private Vector3 center;
        private float radius;
        private Vector3 halfHeight;
        private int borderColor;

        /* "Bounding Cylinder" - ValePorUnNombreGeek 2013
         * Desarrollado para ajustarse mejor a ciertos objetos del Commandos.
         * Basado principalmente en el paper de Michael Sünkel
         * http://www10.informatik.uni-erlangen.de/Publications/Theses/2010/Suenkel_BA_10.pdf
         * 
         * Nota: dada la naturaleza del juego para el que fue desarrollado esta clase,
         * se trata al cilindro como una superficie infinita a lo alto, sin tapas.
         * Esto aplica para las colisiones cilindro-aabb y cilindro-cilindro.
         * Para el resto de las funcionalidades (proyeccion en pantalla, interseccion
         * cilindro-rayo, punto mas cercano, etc) se tienen en cuenta dichas tapas.
         */


        public BoundingCylinder(Vector3 _center, float _halfHeight, float _radius)
            : this(_center, _halfHeight, _radius, Color.Yellow)
        {
            //nothing to do
        }

        public BoundingCylinder(Vector3 _center, float _halfHeight, float _radius, Color _color)
        {
            this.radius = _radius;
            this.halfHeight = new Vector3(0, _halfHeight, 0);
            this.center = _center;
            this.borderColor = _color.ToArgb();
            this.updateDraw();
        }

        #region Getters

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

        public float Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }

        #endregion

        #region Draw

        public Color Color
        {
            get { return Color.FromArgb(this.borderColor); }
            set { this.borderColor = value.ToArgb(); this.updateDraw(); }
        }

        private const int END_CAPS_RESOLUTION = 30; //cantidad de lineas por cada tapa
        private CustomVertex.PositionColored[] topCapVertex; //vertices de la tapa superior
        private CustomVertex.PositionColored[] bottomCapVertex; //vertices de la tapa inferior
        private CustomVertex.PositionColored[] bordersVertex; //vertices de los bordes

        /// <summary>
        /// Actualiza la posicion de los vertices que componen las tapas.
        /// </summary>
        private void updateDraw()
        {
            int vertexCount = END_CAPS_RESOLUTION + 1; //el +1 es para cerrar las tapas
            int color = this.borderColor;

            if (this.topCapVertex == null)
            {
                this.topCapVertex = new CustomVertex.PositionColored[vertexCount];
                this.bottomCapVertex = new CustomVertex.PositionColored[vertexCount];
                this.bordersVertex = new CustomVertex.PositionColored[4]; //bordes laterales
            }


            Vector3 aPoint;
            float angle;
            float step = FastMath.TWO_PI / (float)(vertexCount - 1);

            for (int index = 0; index < vertexCount - 1; index++)
            {
                    angle = step * index;
                    aPoint = new Vector3(FastMath.Cos(angle) * this.radius, 0, FastMath.Sin(angle) * this.radius);
                    
                    this.topCapVertex[index] = new CustomVertex.PositionColored(aPoint + this.halfHeight + this.center, color);
                    this.bottomCapVertex[index] = new CustomVertex.PositionColored(aPoint - this.halfHeight + this.center, color);
            }

            //cerramos las tapas
            this.topCapVertex[vertexCount - 1] = this.topCapVertex[0];
            this.bottomCapVertex[vertexCount - 1] = this.bottomCapVertex[0];


            //bordes laterales
            this.updateBordersDraw();
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

            int color = this.borderColor;

            Vector3 pointA = this.center + this.halfHeight + transversalALaCamara;
            Vector3 pointB = this.center - this.halfHeight + transversalALaCamara;

            this.bordersVertex[0] = new CustomVertex.PositionColored(pointA, color);
            this.bordersVertex[1] = new CustomVertex.PositionColored(pointB, color);

            pointA = this.center + this.halfHeight - transversalALaCamara;
            pointB = this.center - this.halfHeight - transversalALaCamara;

            this.bordersVertex[2] = new CustomVertex.PositionColored(pointA, color);
            this.bordersVertex[3] = new CustomVertex.PositionColored(pointB, color);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.updateBordersDraw();
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineStrip, this.topCapVertex.Length - 1, this.topCapVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineStrip, this.bottomCapVertex.Length - 1, this.bottomCapVertex);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, bordersVertex.Length / 2, bordersVertex);
        }

        public void dispose()
        {
            this.topCapVertex = null;
            this.bottomCapVertex = null;
            this.bordersVertex = null;
        }

        #endregion

        #region Collision

        /// <summary>
        /// Indica si existe colision con una esfera. En tal caso devuelve la normal de colision.
        /// </summary>
        public bool thereIsCollisionCySp(TgcBoundingSphere collider, out Vector3 n)
        {
            Vector3 distance = collider.Center - this.Center;
            if (FastMath.Pow2(distance.X) + FastMath.Pow2(distance.Z) <= FastMath.Pow2(this.radius + collider.Radius))
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

        /// <summary>
        /// Indica si existe colision con otro cilindro. En tal caso devuelve la normal de colision.
        /// </summary>
        public bool thereIsCollisionCyCy(BoundingCylinder collider, out Vector3 n)
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

        /// <summary>
        /// Indica si existe colision con un AABB. En tal caso devuelve la normal de colision.
        /// </summary>
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
            if (absCenterToCenter.X <= boxDimensions.X) goto calculateNormal;
            if (absCenterToCenter.Z <= boxDimensions.Z) goto calculateNormal;

            //vemos si toca una esquina
            float cornerDistance = FastMath.Pow2(absCenterToCenter.X - boxDimensions.X) + FastMath.Pow2(absCenterToCenter.Z - boxDimensions.Z);
            if (cornerDistance <= FastMath.Pow2(this.radius)) goto calculateNormal;

            return false;

        calculateNormal:
            Vector2 boxDimensions2d = fromVector3(boxDimensions);
            Vector2 centerToCenter2d = fromVector3(absCenterToCenter);

            float cross = Vector2.Ccw(centerToCenter2d, boxDimensions2d);

            if (cross > 0) n = new Vector3(centerToCenter.X, 0, 0);
            else if (cross < 0) n = new Vector3(0, 0, centerToCenter.Z);
            else n = new Vector3(centerToCenter.X * boxDimensions2d.Y, 0, centerToCenter.Z * boxDimensions2d.X);
            n.Normalize();
            return true;
        }

        /// <summary>
        /// Indica si existe colision con un AABB rapidamente, perdiendo precision.
        /// Eso se debe a que trata al cilindro como un cubo para ganar performance obviando colisiones raras.
        /// </summary>
        public bool fastThereIsCollisionCyBB(TgcBoundingBox aabb)
        {
            Vector3 boxDimensions = aabb.calculateSize() * 0.5f;
            Vector3 boxCenter = aabb.Position + boxDimensions;

            Vector3 centerToCenter = new Vector3(this.Position.X - boxCenter.X, 0, this.Position.Z - boxCenter.Z);
            Vector3 absCenterToCenter = new Vector3();
            absCenterToCenter.X = FastMath.Abs(centerToCenter.X);
            absCenterToCenter.Z = FastMath.Abs(centerToCenter.Z);

            //vemos si esta muy lejos
            if (absCenterToCenter.X > (boxDimensions.X + this.radius)) return false;
            if (absCenterToCenter.Z > (boxDimensions.Z + this.radius)) return false;

            return true;
        }

        /// <summary>
        /// Indica si un rayo atraviesa el cilindro.
        /// </summary>
        public bool thereIsCollisionCyRay(TgcRay ray)
        {
            //Hallo la normal del plano que corta a la mitad el cilindro
            Vector3 planeNormal = Vector3.Cross(ray.Direction, this.halfHeight);
            planeNormal = Vector3.Cross(planeNormal, this.halfHeight);

            //Usamos el centro del cilindro para hallar D
            float planeD = Vector3.Dot(planeNormal, this.center);

            //Buscamos el punto de interseccion rayo-plano
            float t = planeD - Vector3.Dot(planeNormal, ray.Origin);
            t /= Vector3.Dot(planeNormal, ray.Direction);
            Vector3 planeIntersection = ray.Origin + t * ray.Direction;
            
            //Verificamos que el punto pertenezca al cilindro a lo ancho
            Vector3 distance = planeIntersection - this.Center;
            if (FastMath.Pow2(distance.X) + FastMath.Pow2(distance.Z) > FastMath.Pow2(this.radius)) return false;

            //Verificamos que el punto pertenezca al cilindro a lo alto
            if (FastMath.Abs(planeIntersection.Y - this.center.Y) > this.HalfHeight) return false;

            return true;
        }

        private Vector2 fromVector3(Vector3 v3)
        {
            return new Vector2(v3.X, v3.Z);
        }

        #endregion

        #region OtherGoodies

        /// <summary>
        /// Devuelve el punto dentro del volumen del cilindro mas cercano a determinado punto.
        /// </summary>
        public Vector3 closestCyPointToPoint(Vector3 point)
        {
            Vector3 ret;
            Vector3 distance = point - this.center;

            float y = distance.Y;
            distance.Y = 0;
            if (FastMath.Pow2(distance.X) + FastMath.Pow2(distance.Z) > FastMath.Pow2(this.radius))
                ret = Vector3.Normalize(distance) * this.radius;
            else
                ret = distance;

            if (FastMath.Abs(y) > this.HalfHeight)
                ret.Y = Math.Sign(y) * this.HalfHeight;
            else
                ret.Y = y;

            ret += this.center;
            return ret;
        }

        /// <summary>
        /// Proyecta el cilindro como un rectangulo en pantalla.
        /// Basado en la proyeccion de AABB del TgcViewer.
        /// </summary>
        public Rectangle projectToScreen()
        {
            Device device = GuiController.Instance.D3dDevice;
            Viewport viewport = device.Viewport;
            Matrix world = device.Transform.World;
            Matrix view = device.Transform.View;
            Matrix proj = device.Transform.Projection;

            Vector3 cameraSeen = GuiController.Instance.CurrentCamera.getPosition() - this.center;
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, this.halfHeight);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            //Buscamos las cuatro esquinas del clindro
            Vector3[] projVertices = new Vector3[4];
            projVertices[0] = this.center + this.halfHeight + transversalALaCamara;
            projVertices[1] = this.center + this.halfHeight - transversalALaCamara;
            projVertices[2] = this.center - this.halfHeight + transversalALaCamara;
            projVertices[3] = this.center - this.halfHeight - transversalALaCamara;

            for (int i = 0; i < projVertices.Length; i++)
            {
                projVertices[i] = Vector3.Project(projVertices[i], viewport, proj, view, world);
            }

            //Establecemos los puntos extremos
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            foreach (Vector3 v in projVertices)
            {
                if (v.X < min.X)
                {
                    min.X = v.X;
                }
                if (v.Y < min.Y)
                {
                    min.Y = v.Y;
                }
                if (v.X > max.X)
                {
                    max.X = v.X;
                }
                if (v.Y > max.Y)
                {
                    max.Y = v.Y;
                }
            }
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        #endregion
    }
}
