using System;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.EscenarioRio
{
    /// <summary>
    /// Prueba de movimiento en heightmap
    /// </summary>
    public class EscenarioRio : TgcExample
    {
        Terrain terrain;
        TgcBox rio;

        
        string pathHeightmap;
        string pathTextura;
        FreeCamera camera;


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "EjemploRio";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Ejemplo de heightmap con agua.";
        }

        public override void init()
        {

            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir+"ValePorUnNombreGeek\\";

            pathHeightmap = mediaDir + "Heightmaps\\" + "HeightmapRio.jpg";

            pathTextura = mediaDir + "Heightmaps\\" + "TerrainRioTexture.jpg";

            
            //Cargar heightmap          
            terrain = new Terrain();
            terrain.loadHeightmap(pathHeightmap, 5f, 2f, new Vector3(0, 0, 0));
            terrain.loadTexture(pathTextura);

         

            rio = TgcBox.fromSize(terrain.Position, new Vector3(terrain.getWidth() * terrain.getScaleXZ(), 50*terrain.getScaleY(), terrain.getLength() * terrain.getScaleXZ()),Color.LightSlateGray);
            camera = new FreeCamera();
            camera.Enable=true;
         
        
           
            

        }
        
        public override void render(float elapsedTime)
        { 
           
            rio.render();
            terrain.render();
            
           
        }


        public override void close()
        {
            terrain.dispose();
            
            rio.dispose();
        }


    }



}
