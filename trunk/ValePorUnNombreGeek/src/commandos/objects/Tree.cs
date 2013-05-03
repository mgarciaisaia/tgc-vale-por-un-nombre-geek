using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    class Tree: LevelObject
    {
        TgcBoundingBox boundingBox;
        static string PATH = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\ArbolSelvatico2\\ArbolSelvatico2-TgcScene.xml";
        public Tree(Vector3 position, Vector3 scale)
            : base(PATH, position, scale)
        {

            
            //Hago que el bounding box sólo cubra el tronco
            this.boundingBox = mesh.BoundingBox.clone();
            Vector3 bBScale = new Vector3(0.1f*scale.X, 1f*scale.Y, 0.1f*scale.Z);
            this.boundingBox.scaleTranslate(position, bBScale);
            center = this.boundingBox.calculateBoxCenter();
            radius = this.boundingBox.calculateBoxRadius();
          
        }

        public override TgcBoundingBox BoundingBox
        {
            get { return this.boundingBox; }
        }
        
    }
}
