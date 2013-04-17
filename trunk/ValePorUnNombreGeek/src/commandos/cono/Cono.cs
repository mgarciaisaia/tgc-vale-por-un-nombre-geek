using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    
    class Cono
    {
        private const int DEFAULT_TRIANGLES = 24;
        private const float DEFAULT_ANGLE = 30;
        private const float DEFAULT_RADIUS = 10;

        VertexBuffer vertexBuffer;
        Vector3 vertex;
        Vector3[] circunferencia;
       
        CustomVertex.PositionColored[] vertices;
        float angle, radius;
        int triangles;
        private bool autoTransformEnable;
      
        private Matrix transform;
        private Vector3 translation;
        private Vector3 rotation;
        int cantVertices;

        private bool enabled;
        /// <summary>
        /// Indica si el cono esta habilitado para ser renderizado
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public bool AutoUpdateEnabled
        {
            get { return autoUpdateEnabled; }
            set { autoUpdateEnabled = value; }
        }
        private bool dirtyValues=true;
        private bool autoUpdateEnabled;

        public bool AutoTransformEnable
        {
            get { return autoTransformEnable; }
            set { autoTransformEnable = value; }
        }

        public Matrix Transform
        {
            get { return transform; }
            set { transform = value; }
        }


        public Vector3 Position
        {
            get
            {
                return Vector3.TransformCoordinate(vertex, this.transform);
                

            }

            set { this.vertex = value; }
        }



        public float Radius { get { return radius; } set { radius = value; } }

        public float Angle { get { return angle; } set { angle = value; } }

        public int Triangles { get { return triangles; } set { triangles = value; } }
      

        public Cono(Vector3 vertex){

                         
            config(vertex, DEFAULT_RADIUS, DEFAULT_ANGLE, DEFAULT_TRIANGLES);

        }


        private void config(Vector3 vertex, float radius, float angle, int triangles)
        {
            this.vertex = vertex;
            this.radius = radius;
            this.angle = angle;
            this.triangles = triangles;

            this.autoTransformEnable = true;
            this.transform = Matrix.Identity;
            this.translation = new Vector3(0, 0, 0);
            this.rotation = new Vector3(0, 0, 0);
            this.enabled = true;
            this.autoUpdateEnabled = true;
            
            
        }

        public Cono(Vector3 vertex, float radius, float angle)
        {
            config(vertex, radius, angle, DEFAULT_TRIANGLES);

            this.autoTransformEnable = true;
            this.transform = Matrix.Identity;
            this.translation = new Vector3(0, 0, 0);
            this.rotation = new Vector3(0, 0, 0);

        }

        public Cono(Vector3 vertex, float radius, float angle, int triangles) {
            config(vertex, radius, angle,triangles);
           
        }



        /// <summary>
        /// Actualiza cantidad de triangulos, radio y angulo
        /// </summary>
        public void updateValues()


        {
            int cantPuntos = triangles + 1; //Cant de puntos dibujados
           
                   
            //La circunferencia tiene un punto menos porque no contiene al vertice

            crearCircunferencia(radius*FastMath.Atan(FastMath.ToRad(angle)),cantPuntos-1);

            crearTriangulos();

            dirtyValues = false;
        }

        private void crearTriangulos()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            cantVertices = triangles * 3;

          
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), cantVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);

            vertices = new CustomVertex.PositionColored[cantVertices];

            int i, j;

  

            for (i = 0, j = 0; j < triangles - 1; i += 3, j++)
           {

                              /*                  Creo los triangulos 
                               *
                               *                         vertex
                               *                           /\
                               *                          /  \
                               *                         /    \
                               *                        /      \
                               *                       /        \
                               *                      /          \
                               *     circunferencia[j]------------ circunferencia[j+1]  
                               */

                vertices[i] = new CustomVertex.PositionColored(vertex.X, vertex.Y, vertex.Z, Color.LimeGreen.ToArgb());
                vertices[i + 1] = new CustomVertex.PositionColored(circunferencia[j].X + vertex.X, circunferencia[j].Y + vertex.Y, circunferencia[j].Z + vertex.Z, Color.Aqua.ToArgb());
                vertices[i + 2] = new CustomVertex.PositionColored(circunferencia[j + 1].X + vertex.X, circunferencia[j + 1].Y + vertex.Y, circunferencia[j + 1].Z + vertex.Z, Color.Aqua.ToArgb());

            }

            vertices[i] = new CustomVertex.PositionColored(vertex.X, vertex.Y, vertex.Z, Color.LimeGreen.ToArgb());
            vertices[i + 1] = new CustomVertex.PositionColored(circunferencia[j].X + vertex.X, circunferencia[j].Y + vertex.Y, circunferencia[j].Z + vertex.Z, Color.Aqua.ToArgb());
            vertices[i + 2] = new CustomVertex.PositionColored(circunferencia[0].X + vertex.X, circunferencia[0].Y + vertex.Y, circunferencia[0].Z + vertex.Z, Color.Aqua.ToArgb());


        }

        private void crearCircunferencia(float cradius,int cantPuntos)
        {
 	       float theta;
           float dtheta = 2*FastMath.PI/cantPuntos;
           int i;

          circunferencia = new Vector3[cantPuntos];

           for (i = 0, theta = 0; i < cantPuntos; i++, theta += dtheta)
           {

              circunferencia[i] = new Vector3(
                        cradius*FastMath.Cos(theta),
                        cradius*FastMath.Sin(theta),
                        -radius
                   );
           }

           


        }





        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if(!enabled) return;
            if (dirtyValues || autoUpdateEnabled) updateValues();

         

            
            if (autoTransformEnable)
            {
                this.transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);
                
            }


            //Transformo todos los vertices
            CustomVertex.PositionColored[] vTrans = new CustomVertex.PositionColored[cantVertices];

            for (int i = 0; i < cantVertices; i++)
            {
               vTrans[i].Position = Vector3.TransformCoordinate(vertices[i].Position, this.transform);
               vTrans[i].Color = vertices[i].Color;

            }



            vertexBuffer.SetData(vTrans, 0, LockFlags.None);

            //Especificar formato de triangulos
            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            //Cargar VertexBuffer a renderizar
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);
            //Dibujar triangulos
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangles);

        }


        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(Vector3 v)
        {
            this.move(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(float x, float y, float z)
        {
            this.translation.X += x;
            this.translation.Y += y;
            this.translation.Z += z;

         
        }

        /// <summary>
        /// Mueve la malla en base a la orientacion actual de rotacion.
        /// Es necesario rotar la malla primero
        /// </summary>
        /// <param name="movement">Desplazamiento. Puede ser positivo (hacia adelante) o negativo (hacia atras)</param>
        public void moveOrientedY(float movement)
        {
            float z = (float)Math.Cos((float)rotation.Y) * movement;
            float x = (float)Math.Sin((float)rotation.Y) * movement;

            move(x, 0, z);
        }

        /// <summary>
        /// Obtiene la posicion absoluta de la malla, recibiendo un vector ya creado para
        /// almacenar el resultado
        /// </summary>
        /// <param name="pos">Vector ya creado en el que se carga el resultado</param>
        public void getPosition(Vector3 pos)
        {
            pos.X = translation.X;
            pos.Y = translation.Y;
            pos.Z = translation.Z;
        }

        /// <summary>
        /// Rota la malla respecto del eje X
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateX(float angle)
        {
            this.rotation.X += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            this.rotation.Y += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Z
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateZ(float angle)
        {
            this.rotation.Z += angle;
        }


        public void dispose()
        {
            //liberar VertexBuffer
            vertexBuffer.Dispose();
        }


    }
}
