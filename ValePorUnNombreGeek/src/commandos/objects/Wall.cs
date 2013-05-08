using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    class Wall : ILevelObject
    {
        TgcBox box;
        float radius;
        Vector3 center;
        public static string TEXTURE_PATH = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Pared\\pared.jpg";
       
        
        public Wall(Vector3 center, Vector3 size){
               
            box = TgcBox.fromSize(center, size, TgcTexture.createTexture(TEXTURE_PATH));

            this.center = box.BoundingBox.calculateBoxCenter();
            radius = box.BoundingBox.calculateBoxRadius();
            
 
        }
        public Vector3 Position
        {
            get { return this.box.Position; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        public Vector3 Center
        {
            get { return this.center; }
        }

        public float Radius
        {
            get { return this.radius; }
        }

        public Effect Effect
        {
            get
            {
                return this.box.Effect;
            }
            set
            {
                this.box.Effect = value;
            }
        }

        public string Technique
        {
            get
            {
                return this.box.Technique;
            }
            set
            {
                this.box.Technique = value;
            }
        }

        public void render()
        {
            //this.box.render();
            this.box.BoundingBox.render();
        }

        public void dispose()
        {
            this.box.dispose();
        }
    }
}
