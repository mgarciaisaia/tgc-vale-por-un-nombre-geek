using System;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cone
{
    
    class Cone
    {
        private const int TRANSLUCENCY = 50; //transparencia del cono (solo si se llama a renderTransparent)

        protected const int DEFAULT_TRIANGLES = 24;
        protected const float DEFAULT_ANGLE = 30;
        protected const float DEFAULT_RADIUS = 10;

        protected VertexBuffer vertexBuffer;
        protected Vector3[] circunferencia;
       
        CustomVertex.PositionColored[] vertices;
        protected float angle, length;
        protected int triangles;
    

        public bool AlphaBlendEnabled{ get; set;}
        
      
        int cantVertices;

       
        public bool Enabled{get; set;}
         

        public bool AutoTransformEnabled { get; set; }
        public Matrix Transform { get; set; }
        private Vector3 translation;
        private Vector3 rotation;
        
        public Vector3 Position
        {
            get{ return this.translation;  }

            set { this.translation = value; }
        }
     
        public bool ShowDirection { get; set; }

        /// <summary>
        /// Calcuula y retorna el vector direccion del cono.
        /// </summary>
        public Vector3 Direccion
        {
            get
            {
                //Centro de la circunferencia del final del cono
                Vector3 centroCircunferencia = new Vector3(0, 0, -length);

                //Aplico las transformaciones que sufrio el cono
                Vector3 transCentroCircunferencia = Vector3.TransformCoordinate(centroCircunferencia, this.Transform);

                //Creo el vector que va desde el vertice del cono al centro de la circunferencia
                return transCentroCircunferencia - this.Position;
            }


        }



        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        
        protected bool mustUpdate;
        public float Length { get { return length; } set { length = value; mustUpdate = true; } }
        public float Angle { get { return angle; } set { angle = value; mustUpdate = true; } }
        public int Triangles { get { return triangles; } set { triangles = value; mustUpdate = true; } }
      

        public Cone(Vector3 vertex){

                         
            config(vertex, DEFAULT_RADIUS, FastMath.ToRad(DEFAULT_ANGLE), DEFAULT_TRIANGLES);

        }


        private void config(Vector3 vertex, float radius, float angle, int triangles)
        {
           
         
            this.length = radius;
            this.angle = angle;
            this.triangles = triangles;

            this.AutoTransformEnabled = true;
            this.AlphaBlendEnabled = false;
            this.translation = vertex;
            this.rotation = new Vector3(0, 0, 0);
            this.Enabled = true;
            this.Transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);

            this.ShowDirection = false;
            this.Color1 = Color.BlueViolet;
            this.Color2 = Color.Aqua;
            this.mustUpdate = true;
            
            
        }


        public Cone(Vector3 vertex, float length, float angle)
        {
            config(vertex, length, angle, DEFAULT_TRIANGLES);


        }

        public Cone(Vector3 vertex, float length, float angle, int triangles) {
            config(vertex, length, angle, triangles);
           
        }



        /// <summary>
        /// Actualiza cantidad de triangulos, radio y angulo
        /// </summary>
        public virtual void updateValues()
        {
            int cantPuntos = triangles + 1; //Cant de puntos dibujados
           
                   
            //La circunferencia tiene un punto menos porque no contiene al vertice

            crearCircunferencia(length*FastMath.Tan(angle),cantPuntos-1);

            crearTriangulos();

            mustUpdate = false;
        }


        /// <summary>
        /// Une los puntos de la circunferencia con el vertice del cono para formar los triangulos.
        /// </summary>
        private void crearTriangulos()
        {
            Device d3dDevice = CommandosUI.Instance.d3dDevice;
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

                vertices[i] = new CustomVertex.PositionColored(0, 0, 0, Color1.ToArgb());
                vertices[i + 1] = new CustomVertex.PositionColored(circunferencia[j].X,  circunferencia[j].Y , circunferencia[j].Z , Color2.ToArgb());
                vertices[i + 2] = new CustomVertex.PositionColored(circunferencia[j + 1].X , circunferencia[j + 1].Y , circunferencia[j + 1].Z, Color2.ToArgb());

            }

            vertices[i] = new CustomVertex.PositionColored(0, 0, 0, Color1.ToArgb());
            vertices[i + 1] = new CustomVertex.PositionColored(circunferencia[j].X , circunferencia[j].Y , circunferencia[j].Z, Color2.ToArgb());
            vertices[i + 2] = new CustomVertex.PositionColored(circunferencia[0].X , circunferencia[0].Y , circunferencia[0].Z , Color2.ToArgb());


        }

        /// <summary>
        /// Crea la circunferencia que esta al final del cono.
        /// </summary>
        protected virtual void crearCircunferencia(float cradius,int cantPuntos)
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
                        -length
                   );
           }
            
        }


        public virtual void renderWireframe()
        {
            Device d3dDevice = CommandosUI.Instance.d3dDevice;
            d3dDevice.RenderState.FillMode = FillMode.WireFrame;
            this.render();
            d3dDevice.RenderState.FillMode = FillMode.Solid;
        }

 


        public virtual void render()
        {
            Device d3dDevice = CommandosUI.Instance.d3dDevice;

            if(!Enabled) return;
            if (mustUpdate) updateValues();
            if (this.ShowDirection) renderDirection();

            
            if (AutoTransformEnabled)
            {
                this.Transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);
                
            }


            //Transformo todos los vertices
            CustomVertex.PositionColored[] vTrans = new CustomVertex.PositionColored[cantVertices];

            Color color;
            for (int i = 0; i < cantVertices; i++)
            {
               
               if (i % 3 == 0) color = this.Color1; else color = this.Color2;
               vTrans[i].Position = Vector3.TransformCoordinate(vertices[i].Position, this.Transform);
               vTrans[i].Color = Color.FromArgb(TRANSLUCENCY, 0, 0, 0).ToArgb() + color.ToArgb();

            }
            
            bool alphaBlendEnabled = d3dDevice.RenderState.AlphaBlendEnable;
            d3dDevice.RenderState.AlphaBlendEnable = this.AlphaBlendEnabled;

            vertexBuffer.SetData(vTrans, 0, LockFlags.None);
            
            //Especificar formato de triangulos
            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            //Cargar VertexBuffer a renderizar
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);
            //Dibujar triangulos
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangles);

            d3dDevice.RenderState.AlphaBlendEnable = alphaBlendEnabled;            

        }

        /// <summary>
        /// Renderiza el vector direccion del cono.
        /// </summary>
        public void renderDirection()
        {
            TgcArrow arrow = new TgcArrow();
            arrow.PStart = this.Position;
            arrow.PEnd = this.Position + this.Direccion;
            arrow.Thickness = 2f;
            arrow.HeadSize = new Vector2(0.5f, 0.5f);
            arrow.updateValues();
            arrow.render();
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
            if(vertexBuffer != null) vertexBuffer.Dispose();
        }


    }
}
