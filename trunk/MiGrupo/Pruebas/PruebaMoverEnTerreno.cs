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
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.Terrain;
using AlumnoEjemplos.MiGrupo.Pruebas;

namespace AlumnoEjemplos.MiGrupo.PruebaEscenario
{
    /// <summary>
    /// Prueba de control y animacion de personaje.
    /// </summary>
    public class PruebaEscenario : TgcExample
    {
        Terrain terrain;
        TgcSkeletalMesh personaje;
        string pathHeightmap;
        string pathTextura;
        float escalaXZ;
        float escalaY;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "PruebaEscenario";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de carga de escenarios";
        }

        public override void init()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            GuiController.Instance.Modifiers.addFloat("disminucion dy", 1, 300, 25);
            pathHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmaps\\" + "heightmap.jpg";

            pathTextura = GuiController.Instance.AlumnoEjemplosMediaDir+ "Heightmaps\\" + "TerrainTexture5.jpg";

           
            escalaXZ = 20f;
           
            escalaY = 2f;
            
            terrain = new Terrain();
            terrain.loadHeightmap(pathHeightmap, escalaXZ, escalaY, new Vector3(0, 0, 0));
            terrain.loadTexture(pathTextura);
        
            //Cargar personaje con animaciones
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.AlumnoEjemplosMediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml",
                new string[] { 
                    GuiController.Instance.AlumnoEjemplosMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Walk-TgcSkeletalAnim.xml",
                    GuiController.Instance.AlumnoEjemplosMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "StandBy-TgcSkeletalAnim.xml",
                      });

            //Configurar animacion inicial
            personaje.playAnimation("StandBy", true);
            
            personaje.Position = new Vector3(0, terrain.getHeight(0,0), 0);
            //Rotarlo 180° porque esta mirando para el otro lado
            personaje.rotateY(Geometry.DegreeToRadian(180f));

            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(personaje.Position,80, -120);
            GuiController.Instance.ThirdPersonCamera.TargetDisplacement = new Vector3(0, 45, 0); 
        }


     
        private void moverPersonajeTeclas(float elapsedTime, float speed)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;


            float velocidad = speed;
            float velocidadRotacion = 120f;


            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;


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
               
                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = personaje.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado

                personaje.moveOrientedY(moveForward * elapsedTime);



                float modificador = (float)GuiController.Instance.Modifiers.getValue("disminucion dy");


                //Movimiento en altura: No tiene en cuenta el tema de la inclinación
                personaje.Position = new Vector3(personaje.Position.X, personaje.Position.Y + (terrain.getHeight(personaje.Position.X, personaje.Position.Z) - personaje.Position.Y) / modificador, personaje.Position.Z);





                //Hacer que la camara siga al personaje en su nueva posicion
                GuiController.Instance.ThirdPersonCamera.Target = personaje.Position;
            }
            else
            {
                personaje.playAnimation("StandBy", true);
                
            }


        }

       public override void render(float elapsedTime)

        {
           moverPersonajeTeclas(elapsedTime, 100);
           personaje.updateAnimation();
           personaje.render();
            
            terrain.render();
        }


        public override void close()
        {
            terrain.dispose();
            personaje.dispose();
        }


    }



}
