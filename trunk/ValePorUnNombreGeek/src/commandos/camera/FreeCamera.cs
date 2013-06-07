using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    /// <summary>
    /// Camara rotacional levemente modificada
    /// Controles:
    /// 
    /// Pan: Flechas si desplazamientoFechas = true (default)
    ///      Poniendo mouse en el borde si desplazamientoMouse = true (default)
    /// Zoom: Mouse scroll
    /// Rotacion: Mantener apretado boton central del mouse
    /// Top view: tecla T
    /// </summary>
    class FreeCamera:TgcCamera
    {
        
       public static float DEFAULT_ZOOM_FACTOR = 0.15f;
        public static float DEFAULT_CAMERA_DISTANCE = 600f;
        public static float DEFAULT_ROTATION_SPEED = 100f;
        public static int ANCHO_DESPLAZAMIENTO = 50;

        Vector3 upVector;
        public Vector3 cameraCenter;
        Vector3 nextPos;
        float cameraDistance;
        float zoomFactor;
        float diffX;
        float diffY;
        float diffZ;
        Matrix viewMatrix;
        float rotationSpeed;
        float panSpeed;
        
      

        public FreeCamera()
        {
            resetValues();
        }

        public FreeCamera(ITerrain terrain, bool enabled) : this()
        {
            this.CameraCenter = terrain.getPosition(0, 150);
            this.Terrain = terrain;
            this.Enable = enabled;
        }


        #region Getters y Setters

        bool enable;
  
        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set{ 
                enable = value;

                //Si se habilito la camara, cargar como la cámara actual
                if (value)
                {
                    GuiController.Instance.CurrentCamera = this;
                }
            }
        }

        /// <summary>
        /// Centro de la camara sobre la cual se rota
        /// </summary>
        public Vector3 CameraCenter
        {
            get { return cameraCenter; }
            set { cameraCenter = value; }
        }

        /// <summary>
        /// Distance entre la camara y el centro
        /// </summary>
        public float CameraDistance
        {
            get { return cameraDistance; }
            set { cameraDistance = value; }
        }

        /// <summary>
        /// Velocidad con la que se hace Zoom
        /// </summary>
        public float ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; }
        }

        /// <summary>
        /// Velocidad de rotacion de la camara
        /// </summary>
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        /// <summary>
        /// Velocidad de paneo
        /// </summary>
        public float PanSpeed
        {
            get { return panSpeed; }
            set { panSpeed = value; }
        }
        

        /// <summary>
        /// Configura el centro de la camara, la distancia y la velocidad de zoom
        /// </summary>
        public void setCamera(Vector3 cameraCenter, float cameraDistance, float zoomFactor)
        {
            this.cameraCenter = cameraCenter;
            this.cameraDistance = cameraDistance;
            this.zoomFactor = zoomFactor;
        }

        /// <summary>
        /// Configura el centro de la camara, la distancia
        /// </summary>
        public void setCamera(Vector3 cameraCenter, float cameraDistance)
        {
            this.cameraCenter = cameraCenter;
            this.cameraDistance = cameraDistance;
            this.zoomFactor = DEFAULT_ZOOM_FACTOR;
        }


        #endregion

        /// <summary>
        /// Carga los valores default de la camara
        /// </summary>
        internal void resetValues()
        {
            upVector = new Vector3(0.0f, 1.0f, 0.0f);
            cameraCenter = new Vector3(0, 0, 0);
            nextPos = new Vector3(0, 0, 0);
            cameraDistance = DEFAULT_CAMERA_DISTANCE;
            zoomFactor = DEFAULT_ZOOM_FACTOR;
            rotationSpeed = DEFAULT_ROTATION_SPEED;
            rotationButton = TgcD3dInput.MouseButtons.BUTTON_MIDDLE;
            desplazamientoMouse = true;
            desplazamientoFlechas = true;
            diffX = 0f;
            diffY = 3.6f;
            diffZ = 1.0f;
            viewMatrix = Matrix.Identity;
            panSpeed = 1f;
        }

        /// <summary>
        /// Actualiza los valores de la camara
        /// </summary>
        public void updateCamera()
        {
            if (!enable)
            {
                return;
            }

            // FIXME: las orig_diff_? apestan (pero funcionan)
            float orig_diff_x = diffX;
            float orig_diff_y = diffY;
            float orig_diff_z = diffZ;

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            float elapsedTime = GuiController.Instance.ElapsedTime;

            //Obtener variacion XY del mouse
            float mouseX = 0f;
            float mouseY = 0f;

            if (d3dInput.buttonDown(rotationButton))
            {
                mouseX = d3dInput.XposRelative;
                mouseY = d3dInput.YposRelative;

                diffX += mouseX * elapsedTime * rotationSpeed;
                diffY += mouseY * elapsedTime * rotationSpeed;

               
            }
            else
            {
               
                diffX += mouseX;
                diffY += mouseY;
            }

            if (d3dInput.keyDown(Key.T))
            {
              
                nextPos = new Vector3(0, 0, 0);
                diffX = 0f;
                diffY = 3.6f;
                diffZ = 1.0f;
                cameraDistance = DEFAULT_CAMERA_DISTANCE * 2;
              

               
             }
          


            //Calcular rotacion a aplicar
            float rotX = (-diffY / FastMath.PI);
            float rotY = (diffX / FastMath.PI);

            //Truncar valores de rotacion fuera de rango
            if (rotX > FastMath.PI * 2 || rotX < -FastMath.PI * 2)
            {
                diffY = 0;
                rotX = 0;
            }

            //Invertir Y de UpVector segun el angulo de rotacion
            if (rotX < -FastMath.PI / 2 && rotX > -FastMath.PI * 3 / 2)
            {
                upVector.Y = -1;
            }
            else if (rotX > FastMath.PI / 2 && rotX < FastMath.PI * 3 / 2)
            {
                upVector.Y = -1;
            }
            else
            {
                upVector.Y = 1;
            }


            //Determinar distancia de la camara o zoom segun el Mouse Wheel
            if (d3dInput.WheelPos != 0)
            {
                diffZ += zoomFactor * d3dInput.WheelPos * -1;
            }
           
            float distance = -cameraDistance * diffZ;

            //Limitar el zoom a 0
            if (distance > 0)
            {
                distance = 0;
            }

            
            //Realizar Transformacion: primero alejarse en Z, despues rotar en X e Y y despues ir al centro de la cmara
            Matrix m = Matrix.Translation(0, 0, -distance)
                * Matrix.RotationX(rotX)
                * Matrix.RotationY(rotY)
            * Matrix.Translation(cameraCenter);


            //Extraer la posicion final de la matriz de transformacion
            nextPos.X = m.M41;
            nextPos.Y = m.M42;
            nextPos.Z = m.M43;


            if (desplazamientoMouse)
            {
                desplazarCuandoMouseEstaEnBorde(distance);
            }
            if(desplazamientoFlechas)
            {
                desplazarConFlechas(distance);
            }

            if (nextPos.Y > this.Terrain.getPosition(nextPos.X, nextPos.Z).Y + 40)
            {


                //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
                viewMatrix = Matrix.LookAtLH(nextPos, cameraCenter, upVector);

            }
            else
            {
                // FIXME: esto apesta (pero funciona)
                diffX = orig_diff_x;
                diffY = orig_diff_y;
                diffZ = orig_diff_z;
            }

        }

        private void desplazarConFlechas(float distance)
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            float dx = 0;
            float dy = 0;

            //Mover si el mouse está en un borde.
            if ( d3dInput.keyDown(Key.LeftArrow))

                dx = -GuiController.Instance.ElapsedTime;

            else if ( d3dInput.keyDown(Key.RightArrow))

                dx = GuiController.Instance.ElapsedTime;

            if ( d3dInput.keyDown(Key.UpArrow))

                dy = GuiController.Instance.ElapsedTime;

            else if ( d3dInput.keyDown(Key.DownArrow))

                dy = -GuiController.Instance.ElapsedTime;

            if (dx != 0 || dy != 0)
            {
                float panSpeedZoom = panSpeed * FastMath.Abs(distance);

                Vector3 d = cameraCenter - nextPos;
                d.Normalize();

                d.Y = 0;


                Vector3 n = Vector3.Cross(d, upVector);
                n.Normalize();



                Vector3 up = Vector3.Cross(n, d);
                Vector3 desf = Vector3.Scale(d, dy * panSpeedZoom) - Vector3.Scale(n, dx * panSpeedZoom);

                //desf.Y = 0;
                nextPos = nextPos + desf;
                cameraCenter = cameraCenter + desf;
            }
        }

        private void desplazarCuandoMouseEstaEnBorde(float distance)
        {
            float dx = 0;
            float dy = 0;

            //Mover si el mouse está en un borde.
            if (GuiController.Instance.D3dInput.Xpos <= 20 && GuiController.Instance.D3dInput.Xpos > 0 && Math.Abs(GuiController.Instance.D3dInput.Ypos - GuiController.Instance.Panel3d.Height/2)<ANCHO_DESPLAZAMIENTO)

                dx = -GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Xpos >= GuiController.Instance.Panel3d.Width - 20 && GuiController.Instance.D3dInput.Xpos < GuiController.Instance.Panel3d.Width && Math.Abs(GuiController.Instance.D3dInput.Ypos - GuiController.Instance.Panel3d.Height/2) < ANCHO_DESPLAZAMIENTO)

                dx = GuiController.Instance.ElapsedTime;

            if (GuiController.Instance.D3dInput.Ypos <= 20 && GuiController.Instance.D3dInput.Ypos > -20 && Math.Abs(GuiController.Instance.D3dInput.Xpos - GuiController.Instance.Panel3d.Width/2) < ANCHO_DESPLAZAMIENTO)

                dy = GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Ypos >= GuiController.Instance.Panel3d.Height - 20 && GuiController.Instance.D3dInput.Ypos < GuiController.Instance.Panel3d.Height && Math.Abs(GuiController.Instance.D3dInput.Xpos - GuiController.Instance.Panel3d.Width/2) < ANCHO_DESPLAZAMIENTO)

                dy = -GuiController.Instance.ElapsedTime;

            if (dx != 0 || dy != 0)
            {
                float panSpeedZoom = panSpeed * FastMath.Abs(distance);

                Vector3 d = cameraCenter - nextPos;
                d.Normalize();

                d.Y = 0;


                Vector3 n = Vector3.Cross(d, upVector);
                n.Normalize();



                Vector3 up = Vector3.Cross(n, d);
                Vector3 desf = Vector3.Scale(d, dy * panSpeedZoom) - Vector3.Scale(n, dx * panSpeedZoom);

                //desf.Y = 0;
                nextPos = nextPos + desf;
                nextPos.Y = Math.Max(nextPos.Y, this.Terrain.getPosition(nextPos.X, nextPos.Z).Y);
                cameraCenter = cameraCenter + desf;
            }
        }


        /// <summary>
        /// Actualiza la ViewMatrix, si es que la camara esta activada
        /// </summary>
        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }


        public Vector3 getPosition()
        {
            return nextPos;
        }

        public Vector3 getLookAt()
        {
            return cameraCenter;
        }

        /// <summary>
        /// Configura los parámetros de la cámara en funcion del BoundingBox de un modelo
        /// </summary>
        /// <param name="boundingBox">BoundingBox en base al cual configurar</param>
       
        
        public void targetObject(TgcBoundingBox boundingBox)
        {
            cameraCenter = boundingBox.calculateBoxCenter();
            float r = boundingBox.calculateBoxRadius();
            cameraDistance = 2 * r;
        }

        public void targetPoint(Vector3 point){
            
            cameraCenter = point;
           
        }




        public TgcD3dInput.MouseButtons rotationButton { get; set; }

        public bool desplazamientoMouse { get; set; }

        public bool desplazamientoFlechas { get; set; }

        public ITerrain Terrain { get; set; }
    }
}
