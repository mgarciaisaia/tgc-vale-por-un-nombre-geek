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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.PruebaVision
{
    /// <summary>
    /// Prueba de vision
    /// </summary>
    public class PruebaVision : TgcExample
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
            return "Prueba de vision";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de vision";
        }

       
        Commando pj;
        Enemy enemigo;

        Level nivel;

        public override void init(){
        

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;
            nivel = new Level(
                new Terrain(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Heightmaps\\HeightmapPlano.jpg",
                    GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg",
                    10f,
                    1f
                    )
                 );
            pj = new Commando(new Vector3(0, 0, -10));
            pj.Representation.AutoTransformEnable = true;


            enemigo = new Soldier(new Vector3(0, 0, 0)); //TODO arreglar
            enemigo.ShowConeDirection = true;
            nivel.add(enemigo);
            nivel.add(pj);
            


            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 500,100);
            GuiController.Instance.Modifiers.addFloat("AnguloVision", 0, 90, 45);
            GuiController.Instance.Modifiers.addBoolean("Direccion","Mostrar",false);
            GuiController.Instance.RotCamera.targetObject(enemigo.BoundingBox());
            GuiController.Instance.Modifiers.addVertex3f("posicionTarget", new Vector3(-100, -100, -100), new Vector3(100, 100, 100), new Vector3(0, 0, -20));
          
        }



      


        public override void render(float elapsedTime)
        {

        
          
           enemigo.VisionAngle = FastMath.ToRad((float)GuiController.Instance.Modifiers.getValue("AnguloVision"));
           enemigo.VisionRadius = (float)GuiController.Instance.Modifiers.getValue("RadioVision");
         
           enemigo.ShowConeDirection = (bool) GuiController.Instance.Modifiers.getValue("Direccion");
          
          
           pj.Position = (Vector3)GuiController.Instance.Modifiers.getValue("posicionTarget");



           nivel.render(elapsedTime);

         
        }


        public override void close()
        {
            nivel.dispose();
        }


    }

   
 




}
