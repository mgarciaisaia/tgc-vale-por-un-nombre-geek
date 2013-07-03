using TgcViewer.Example;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.PruebaVision
{
    /// <summary>
    /// Prueba de vision
    /// </summary>
    public class PruebaVisionHeightmap : TgcExample
    {

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Prueba vision Heightmap";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de vision del soldado a traves del terreno";
        }

        
      
        Enemy enemigo;
        Selection selection;
        MovementPicking picking;
        Commando commando;
        float previousAngle;
        Level nivel;
        public override void init()
        {


            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String mediaDir = CommandosUI.Instance.MediaDir;
            Terrain terrain = new Terrain(
                mediaDir+"Heightmaps\\"+"HeightmapParedes.jpg",
                mediaDir + "Heightmaps\\"+"TexturaParedes.jpg",
                10f,
                0.4f);

            nivel = new Level(terrain);
           

            commando = new Commando(terrain.getPosition(200, 200));
            nivel.add(commando);
            commando.Life.Infinite = true;


           
            Vector3[] waitpoints = new Vector3[1];
            terrain.heightmapCoordsToXYZ(new Vector2(60, 60), out waitpoints[0]);
            //terrain.heightmapCoordsToXYZ(new Vector2(22, 80), out waitpoints[1]);
            //terrain.heightmapCoordsToXYZ(new Vector2(10, 37), out waitpoints[2]);

            
            enemigo = new Enemy(new Vector3(0,0,0));
            nivel.add(enemigo);
            

            //Seleccion multiple
            selection = new Selection(nivel.Characters, nivel.Terrain);

            //Movimiento por picking
            picking = new MovementPicking(nivel.Terrain);

            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 500, 100);
            GuiController.Instance.Modifiers.addFloat("AnguloVision", 0, 90, 45);
            GuiController.Instance.Modifiers.addBoolean("Direccion", "Mostrar", false);
            GuiController.Instance.Modifiers.addVertex3f("posicionEnemigo", new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000), new Vector3(400, 0, 100));
            GuiController.Instance.Modifiers.addFloat("RotacionEnemigo", 0, 360, 0);



            previousAngle = 0;

            new StandardCamera();
            nivel.CullingEnabled = false;
        }






        public override void render(float elapsedTime)
        {


            picking.update(selection.getSelectedCharacters());

            Vector3 pos =  (Vector3)GuiController.Instance.Modifiers.getValue("posicionEnemigo");
            
            enemigo.VisionAngle = FastMath.ToRad((float)GuiController.Instance.Modifiers.getValue("AnguloVision"));
            enemigo.VisionRadius = (float)GuiController.Instance.Modifiers.getValue("RadioVision");
            enemigo.ShowConeDirection = (bool)GuiController.Instance.Modifiers.getValue("Direccion");
            float angle = (float)GuiController.Instance.Modifiers.getValue("RotacionEnemigo");
            enemigo.Representation.rotate(angle-previousAngle, true);
            enemigo.Position = pos;
            previousAngle = angle;
            nivel.render(elapsedTime);
            enemigo.canSee(commando);
            enemigo.VisionCone.render();



            selection.update();


        }


        public override void close()
        {
            nivel.dispose();
        }


    }







}
