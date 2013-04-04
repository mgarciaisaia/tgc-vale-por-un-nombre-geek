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
        TgcPickingRay pickingRay;
        Vector3 newPosition;
        bool applyMovement;
        TgcBox collisionPointMesh;
        String currentControl;
        Vector3 originalMeshRot;
        Matrix meshRotationMatrix;


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


            //Picking
            pickingRay = new TgcPickingRay();
            GuiController.Instance.Modifiers.addFloat("speed", 10, 100, 40);
            collisionPointMesh = TgcBox.fromSize(new Vector3(3, 1, 3), Color.Red);

            //Crear un modifier para un ComboBox con opciones
            string[] opciones = new string[] { "Teclas", "Picking"};
            GuiController.Instance.Modifiers.addInterval("Controles", opciones, 0);
            currentControl = "Teclas";

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(personaje.Position, 100, -30);

            //Coordenadas mouse
            GuiController.Instance.UserVars.addVar("Posicion Mouse");

           
           
         }


        public override void render(float elapsedTime)
        {
            this.moverPersonaje(elapsedTime);
            GuiController.Instance.UserVars.setValue("Posicion Mouse", "(" + GuiController.Instance.D3dInput.Xpos+ "," + GuiController.Instance.D3dInput.Ypos.ToString()+")");
            personaje.updateAnimation();
            piso.render();
            personaje.render();

        }

        private void moverPersonaje(float elapsedTime)
        {

            if (GuiController.Instance.Modifiers.getValue("Controles").Equals("Teclas")){

                if(!currentControl.Equals("Teclas")){
                    personaje.AutoTransformEnable=true;
                }
                this.moverPersonajeTeclas(elapsedTime);
            }

                
            else
            {
                if (!currentControl.Equals("Picking"))
                {
                    newPosition = personaje.Position;
                    applyMovement = false;
                    //Rotación original de la malla, hacia -Z
                    originalMeshRot = new Vector3(0, 0, -1);

                    //Manipulamos los movimientos del mesh a mano
                    personaje.AutoTransformEnable = false;
                    meshRotationMatrix = Matrix.Identity;
                }
                this.moverPersonajePicking(elapsedTime);
            }
            currentControl = (string) GuiController.Instance.Modifiers.getValue("Controles");
        }

        private void moverPersonajeTeclas(float elapsedTime)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;


            float velocidadNadar = (float)GuiController.Instance.Modifiers.getValue("speed");
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


               //Hacer que la camara siga al personaje en su nueva posicion
               GuiController.Instance.ThirdPersonCamera.Target = personaje.Position;
            }

         
        }

        private void moverPersonajePicking(float elapsedTime)
        {
            
            desplazarVista(40);
            
            //Si hacen clic con el mouse, ver si hay colision con el suelo
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();

                //Detectar colisión Ray-AABB
                if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, piso.BoundingBox, out newPosition))
                {
                    //Fijar nueva posición destino
                    applyMovement = true;

                    collisionPointMesh.Position = newPosition;
                   
                    //Rotar modelo en base a la nueva dirección a la que apunta
                     Vector3 direction = Vector3.Normalize(newPosition - personaje.Position);
                    float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction));
                    Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction);
                    meshRotationMatrix = Matrix.RotationAxis(axisRotation, angle);
                }
            }


            float speed = (float)GuiController.Instance.Modifiers["speed"];


            //Interporlar movimiento, si hay que mover
            if (applyMovement)
            {
                //Ver si queda algo de distancia para mover
                Vector3 posDiff = newPosition - personaje.Position;
                float posDiffLength = posDiff.LengthSq();
                if (posDiffLength > float.Epsilon)
                {
                    //Movemos el mesh interpolando por la velocidad
                    float currentVelocity = speed * elapsedTime;
                    posDiff.Normalize();
                    posDiff.Multiply(currentVelocity);

                    //Ajustar cuando llegamos al final del recorrido
                    Vector3 newPos = personaje.Position + posDiff;
                    if (posDiff.LengthSq() > posDiffLength)
                    {
                        newPos = newPosition;
                    }

                  
                    //Actualizar posicion del mesh
                     personaje.Position = newPos;

                    //Como desactivamos la transformacion automatica, tenemos que armar nosotros la matriz de transformacion
                    personaje.Transform = meshRotationMatrix * Matrix.Translation(personaje.Position);

                    //Actualizar camara
                    GuiController.Instance.ThirdPersonCamera.Target = personaje.Position;
                }
                //Se acabo el movimiento
                else
                {
                    applyMovement = false;
                }
                //Mostrar caja con lugar en el que se hizo click, solo si hay movimiento
                if (applyMovement)
                {
                    collisionPointMesh.render();
                    
                }

            }


        }

        private static void desplazarVista(float cameraSpeed)
        {
            Vector3 desplazamiento;
            float dx=0, dz=0;
           
            //Mover si el mouse está en un borde.
            if (GuiController.Instance.D3dInput.Xpos <= 10 && GuiController.Instance.D3dInput.Xpos > 0)
            
                dx = -cameraSpeed * GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Xpos >= GuiController.Instance.Panel3d.Width-10 && GuiController.Instance.D3dInput.Xpos < GuiController.Instance.Panel3d.Width)

                dx = cameraSpeed * GuiController.Instance.ElapsedTime;

            if (GuiController.Instance.D3dInput.Ypos <= 10 && GuiController.Instance.D3dInput.Ypos > -10)

                dz = cameraSpeed * GuiController.Instance.ElapsedTime;

            else if (GuiController.Instance.D3dInput.Ypos >= GuiController.Instance.Panel3d.Height-10 && GuiController.Instance.D3dInput.Ypos < GuiController.Instance.Panel3d.Height)

                dz = -cameraSpeed * GuiController.Instance.ElapsedTime;


            desplazamiento = new Vector3(dx,0,dz);
            GuiController.Instance.ThirdPersonCamera.Target = GuiController.Instance.ThirdPersonCamera.Target + desplazamiento;
        }
        
        public override void close()
        {
            personaje.dispose();
            collisionPointMesh.dispose();
            piso.dispose();
        }

    }

   
 

}
