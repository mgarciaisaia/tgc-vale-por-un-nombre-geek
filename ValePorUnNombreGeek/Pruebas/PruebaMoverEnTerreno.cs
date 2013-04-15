using System;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.PruebaEscenario
{
    /// <summary>
    /// Prueba de movimiento en heightmap
    /// </summary>
    public class PruebaEscenario : TgcExample
    {
        Terrain terrain;
        TgcSkeletalMesh personaje;
        FreeCamera camara;
        string pathHeightmap;
        string pathTextura;
    

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "PruebaMovimientoEnHeightmap";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de movimiento en Heightmap";
        }

        public override void init()
        {
           
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir+"ValePorUnNombreGeek\\";

            pathHeightmap =mediaDir + "Heightmaps\\" + "heightmap.jpg";

            pathTextura = mediaDir + "Heightmaps\\" + "TerrainTexture5.jpg";

           
           //Cargar heightmap          
            terrain = new Terrain();
            terrain.loadHeightmap(pathHeightmap, 20f, 2f, new Vector3(0, 0, 0));
            terrain.loadTexture(pathTextura);
        
            //Cargar personaje con animaciones
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                mediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml",
                new string[] { 
                    mediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Walk-TgcSkeletalAnim.xml",
                    mediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "StandBy-TgcSkeletalAnim.xml",
                      });


            //Configurar animacion inicial
            personaje.playAnimation("StandBy", true);

            //Posicion inicial
            personaje.Position = new Vector3(0, terrain.getHeight(0,0), 0);
           
         
            camara = new FreeCamera(personaje.Position, true);
           
            
            //Configurar camara en Tercer Persona
           /* GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(personaje.Position,80, -120);
            GuiController.Instance.ThirdPersonCamera.TargetDisplacement = new Vector3(0, 45, 0);
            */


            //Modificador para el calculo de movimiento en y
            GuiController.Instance.Modifiers.addFloat("disminucion dy", 1, 300, 25);

            
        }


     
        private void moverPersonajeTeclas(float elapsedTime, float velocidad)
        {
            
           
            float velocidadRotacion = 120f;


            float moveForward;
            float rotate;

            bool moving;
            bool rotating;

            teclasPulsadas(velocidad, velocidadRotacion, out moveForward, out rotate, out moving, out rotating);


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

                personaje.playAnimation("Walk", true);
               
                //Hacer que avance en XZ
                personaje.moveOrientedY(moveForward * elapsedTime);


                //Movimiento en Y: No tiene en cuenta el tema de la inclinación
                float modificador = (float)GuiController.Instance.Modifiers.getValue("disminucion dy");
                float dy = terrain.getHeight(personaje.Position.X, personaje.Position.Z) - personaje.Position.Y;
                personaje.Position = new Vector3(personaje.Position.X, personaje.Position.Y + dy / modificador, personaje.Position.Z);


                //Hacer que la camara siga al personaje en su nueva posicion
                GuiController.Instance.ThirdPersonCamera.Target = personaje.Position;



            }
            
            else
                personaje.playAnimation("StandBy", true);
                
           


        }

        private static void teclasPulsadas(float velocidad, float velocidadRotacion, out float moveForward, out float rotate, out bool moving, out bool rotating)
        {

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
           
            
            moveForward = 0f;
            rotate = 0;
           
            moving = false;
            rotating = false;


            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidad;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = velocidad;
                moving = true;
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

        }

       public override void render(float elapsedTime)

        {
           moverPersonajeTeclas(elapsedTime, 100);
           
           personaje.updateAnimation();
           personaje.render();

           if ((bool)GuiController.Instance.Modifiers.getValue("Terrain"))
           {
                       terrain.renderWireframe();
           }
           else terrain.render();
        }


        public override void close()
        {
            terrain.dispose();
            personaje.dispose();
        }


    }



}
