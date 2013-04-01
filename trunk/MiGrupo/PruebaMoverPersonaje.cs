using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcKeyFrameLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.PruebaMoverPersonaje
{
    /// <summary>
    /// Prueba de control y animacion de personaje.
    /// </summary>
    public class PruebaMoverPersonaje : TgcExample
    {
        TgcKeyFrameMesh personaje;
        TgcBox piso;
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "PruebaMoverPersonaje";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Mover personaje animado  Consotroles: ASDWQE";
        }

        public override void init()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;


            //Crear loader
            TgcKeyFrameLoader loader = new TgcKeyFrameLoader();

            //Crear piso
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg");
            piso = TgcBox.fromSize(new Vector3(0, -60, 0), new Vector3(1000, 5, 1000), pisoTexture);



           //Cargar modelo con una animación Key Frame
            string pathMesh = GuiController.Instance.AlumnoEjemplosMediaDir + "pez-TgcKeyFrameMesh.xml";
            string[] animationsPath = new string[] { GuiController.Instance.AlumnoEjemplosMediaDir + "Animaciones//nadar-TgcKeyFrameAnim.xml" };
            personaje = (TgcKeyFrameMesh)loader.loadMeshAndAnimationsFromFile(pathMesh, animationsPath);

            personaje.playAnimation("nadar");

            //Posicion inicial
            personaje.Position = new Vector3(0, -45, 0);
            personaje.rotateY(Geometry.DegreeToRadian(180f));
             

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(personaje.Position, 30, -30);
            
         }


        public override void render(float elapsedTime)
        {
            this.moverPersonaje(elapsedTime);
            personaje.updateAnimation();
            piso.render();
            personaje.render();

        }

        private void moverPersonaje(float elapsedTime)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
           
            float velocidadNadar = 40f;
            float velocidadRotacion = 120f;


            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float moveUp = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
          

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidadNadar;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = velocidadNadar;
                moving = true;
            }

             //Arriba

           if(d3dInput.keyDown(Key.Q)){
               moveUp =- velocidadNadar;
               moving = true;
           }

            if(d3dInput.keyDown(Key.E)){
                moveUp = velocidadNadar;
                moving =true;
            }
            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

          

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                personaje.rotateY(rotAngle);
                
            }

            //Si hubo desplazamiento
            if (moving)
            {
               //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = personaje.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                personaje.moveOrientedY(moveForward * elapsedTime);


               personaje.move(0, moveUp * elapsedTime, 0);

               if (personaje.Rotation.X == 0) //Rotar al subir o bajar
               {
                   if (moveUp != 0)
                   {
                       if (moveUp > 0)

                           personaje.rotateX(Geometry.DegreeToRadian(30));



                       else if (moveUp < 0f)

                           personaje.rotateX(Geometry.DegreeToRadian(-30));
                   }
               }
               else if (moveUp == 0f) //Enderezar 
               {
                   if (personaje.Rotation.X == Geometry.DegreeToRadian(30)) personaje.rotateX(Geometry.DegreeToRadian(-30));
                   else personaje.rotateX(Geometry.DegreeToRadian(30));
               }
                
            }

           
            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = personaje.Position;
        }

        public override void close()
        {
            personaje.dispose();
            piso.dispose();
        }

    }

   
 

}
