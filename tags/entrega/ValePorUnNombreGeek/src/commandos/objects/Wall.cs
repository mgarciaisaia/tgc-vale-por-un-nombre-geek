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
    class Wall : AABBObject
    {
        TgcBox box;
        float radius;
        Vector3 position;
        public static string TEXTURE_PATH = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Pared\\pared.jpg";
       
        
        public Wall(Vector3 position, Vector3 size){
            TgcTexture textura = TgcTexture.createTexture(TEXTURE_PATH);
            box = TgcBox.fromSize(position+new Vector3(0, size.Y/2,0), size, textura);

            box.UVTiling = new Vector2(size.X / textura.Width*3, size.Y / textura.Height*3);
            box.updateValues();

            this.position = position;
            radius = box.BoundingBox.calculateBoxRadius();

        }
        public override Vector3 Position
        {
            get { return this.position; }
            set { this.box.Position = value + new Vector3(0, this.Size.Y / 2, 0); this.position = value; }
        }

        public override TgcBoundingBox BoundingBox
        {
            get { return this.box.BoundingBox; }
        }

        public override Vector3 Center
        {
            get { return this.box.Position; }
          
        }

        public override float Radius
        {
            get { return this.radius; }
        }

        public Vector3 Size
        {
            get { return this.box.Size; }
            set { this.box.Size = value; this.Position = position; this.box.updateValues(); }
        }

        public override Effect Effect
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

        public override string Technique
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

        public override void render()
        {
            this.box.render();
           
        }

        public override void dispose()
        {
            this.box.dispose();
        }
    }
}
