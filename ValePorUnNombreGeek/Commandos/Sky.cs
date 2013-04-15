using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    class Sky : TgcSkyBox
    {
        //private TgcSkyBox skyBox;

        public Sky() : base()
        {
            this.Center = new Vector3(0, 0, 0);
            this.Size = new Vector3(10000, 10000, 10000);
            
            string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox1\\";
            this.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "phobos_up.jpg");
            this.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "phobos_dn.jpg");
            this.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "phobos_lf.jpg");
            this.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "phobos_rt.jpg");
            this.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "phobos_ft.jpg");
            this.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "phobos_bk.jpg");

            /*
            string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox3\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
             */

            this.updateValues();
        }
    }
}
