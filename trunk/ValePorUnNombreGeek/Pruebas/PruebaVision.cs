using TgcViewer.Example;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos;


namespace AlumnoEjemplos.ValePorUnNombreGeek.PruebaVision
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

        TgcBox piso, caja;
        Enemy enemigo;

        public override void init(){
        

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\tierra.jpg");
            
            piso = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1000, 5, 1000), pisoTexture);

            caja = TgcBox.fromSize(new Vector3(0, 0, -10), new Vector3(10, 100, 10), Color.Blue);

           


            enemigo = new Enemy(new Vector3(0,0,0));


           

            GuiController.Instance.RotCamera.targetObject(enemigo.BoundingBox());
            GuiController.Instance.Modifiers.addVertex3f("posicionCaja", new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000), new Vector3(0, 0, -20));

                 

        }



      


        public override void render(float elapsedTime)
        {
           piso.render();
           enemigo.render(elapsedTime);
           caja.Position = (Vector3)GuiController.Instance.Modifiers.getValue("posicionCaja");
           if (enemigo.puedeVer(caja)) caja.Color = Color.Green; else caja.Color = Color.Red;
           caja.updateValues();
           caja.render();

        }


        public override void close()
        {
            piso.dispose();
            enemigo.dispose();
        }


    }

   
 




}
