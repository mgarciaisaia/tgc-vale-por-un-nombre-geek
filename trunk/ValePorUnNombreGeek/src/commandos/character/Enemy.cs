using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Walker
    {
        private ConoDeVision cono;
      
        public Enemy(Vector3 _position)
            : base(_position)
        {
            float alturaCabeza = (this.personaje.BoundingBox.PMax.Y - this.personaje.BoundingBox.PMin.Y)*9/10;
            cono = new ConoDeVision(new Vector3(this.getPosition().X,this.getPosition().Y + alturaCabeza,this.getPosition().Z) , 500, 90);
            cono.Enabled = false;
            cono.AutoTransformEnable = false;
            

        }

        protected new static string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "CS_Arctic-TgcSkeletalMesh.xml";
      
        }

        public bool puedeVer(TgcBox target)
        {
            
            return cono.colisionaCon(target);
        }

        protected override void update()
        {
            base.update();
            this.cono.Transform = this.personaje.Transform; //Hacer que el cono se mueva con el personaje
            if (cono.Enabled) cono.render();
        }


        

        public float VisionAngle { get { return cono.Angle; } set { cono.Angle = value; } }

        public float VisionRadius { get { return cono.Radius; } set { cono.Radius = value; } }

        public bool ConeEnabled { get { return cono.Enabled; } set { cono.Enabled = value; } }
    }


    /* class EnemyFactory : TgcSkeletalLoader.IMeshFactory
    {
        public TgcSkeletalMesh createNewMesh(Mesh d3dMesh, string meshName, TgcSkeletalMesh.MeshRenderType renderType, TgcSkeletalBone[] bones)
        {
            return new Enemy(d3dMesh, meshName, renderType, bones);
        }
    }*/

}