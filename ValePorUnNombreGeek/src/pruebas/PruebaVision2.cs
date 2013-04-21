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


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.PruebaVision
{
    /// <summary>
    /// Prueba de vision
    /// </summary>
    public class PruebaVision2 : TgcExample
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
            return "Prueba de vision2";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de vision";
        }

        Terrain terrain;
        Character pj;
        Enemy enemigo;

        public override void init()
        {


            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg");

            terrain = new Terrain();

            pj = new Commando(terrain.getPosition(200, 200), terrain);


            pj.Representation.AutoTransformEnable = true;


            

            Vector3[] waitpoints = new Vector3[1];
            terrain.heightmapCoordsToXYZ(new Vector2(60, 60), out waitpoints[0]);
            //terrain.heightmapCoordsToXYZ(new Vector2(22, 80), out waitpoints[1]);
            //terrain.heightmapCoordsToXYZ(new Vector2(10, 37), out waitpoints[2]);

            
            enemigo = new Soldier(waitpoints, terrain);

            enemigo.ShowConeDirection = true;

            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 500, 100);
            GuiController.Instance.Modifiers.addFloat("AnguloVision", 0, 90, 45);

            GuiController.Instance.RotCamera.targetObject(enemigo.BoundingBox());
            GuiController.Instance.Modifiers.addVertex3f("posicionTarget", new Vector3(-600, -600, -600), new Vector3(600, 600, 600), new Vector3(200, 0, 200));
            GuiController.Instance.UserVars.addVar("PuedeVerlo");


        }






        public override void render(float elapsedTime)
        {



            enemigo.VisionAngle = FastMath.ToRad((float)GuiController.Instance.Modifiers.getValue("AnguloVision"));
            enemigo.VisionRadius = (float)GuiController.Instance.Modifiers.getValue("RadioVision");

            terrain.render();
            enemigo.render(elapsedTime);
            enemigo.BoundingBox().render();

            Vector3 pos = (Vector3)GuiController.Instance.Modifiers.getValue("posicionTarget");
            pj.Position = terrain.getPosition(pos.X, pos.Z);
            if (enemigo.canSee(pj))
                GuiController.Instance.UserVars.setValue("PuedeVerlo", true);
            else
                GuiController.Instance.UserVars.setValue("PuedeVerlo", false);

            pj.render(elapsedTime);

            pj.BoundingBox().render();
            enemigo.renderVision();


        }


        public override void close()
        {
            terrain.dispose();
            enemigo.dispose();
        }


    }







}
