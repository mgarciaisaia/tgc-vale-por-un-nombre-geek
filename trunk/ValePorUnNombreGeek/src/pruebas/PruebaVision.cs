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

        TgcBox piso;
        Character pj;
        Enemy enemigo;

        public override void init(){
        

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg");
            
            piso = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1000, 5, 1000), pisoTexture);

            pj = new Commando(new Vector3(0, 0, -10));
            pj.Representation.AutoTransformEnable = true;


            enemigo = new Soldier(new Vector3(0, 0, 0), null); //TODO arreglar
            enemigo.ShowConeDirection = true;



            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 500,100);
            GuiController.Instance.Modifiers.addFloat("AnguloVision", 0, 90, 45);
            GuiController.Instance.Modifiers.addBoolean("Direccion","Mostrar",false);
            GuiController.Instance.RotCamera.targetObject(enemigo.BoundingBox());
            GuiController.Instance.Modifiers.addVertex3f("posicionTarget", new Vector3(-100, -100, -100), new Vector3(100, 100, 100), new Vector3(0, 0, -20));
            GuiController.Instance.UserVars.addVar("PuedeVerlo");
                 

        }



      


        public override void render(float elapsedTime)
        {

        
          
           enemigo.VisionAngle = FastMath.ToRad((float)GuiController.Instance.Modifiers.getValue("AnguloVision"));
           enemigo.VisionRadius = (float)GuiController.Instance.Modifiers.getValue("RadioVision");
         
           piso.render();
           enemigo.ShowConeDirection = (bool) GuiController.Instance.Modifiers.getValue("Direccion");
          
          
           pj.Position = (Vector3)GuiController.Instance.Modifiers.getValue("posicionTarget");
           
           if (enemigo.canSee(pj))
               GuiController.Instance.UserVars.setValue("PuedeVerlo", true);
           else
               GuiController.Instance.UserVars.setValue("PuedeVerlo", false);

           pj.render(elapsedTime);

           pj.BoundingBox().render();

           enemigo.render(elapsedTime);
           enemigo.BoundingBox().render();
        }


        public override void close()
        {
            piso.dispose();
            enemigo.dispose();
        }


    }

   
 




}
