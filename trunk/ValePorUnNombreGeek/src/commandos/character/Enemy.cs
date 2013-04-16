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

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Walker
    {

        public Enemy(Vector3 _position)
            : base(_position)
        {
            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 1000, 500);
            GuiController.Instance.Modifiers.addFloat("AnguloVision", 0, 360, 90);
        }

        protected new static string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "CS_Arctic-TgcSkeletalMesh.xml";
      
        }

        public bool puedeVer(TgcBox target)
        {
            float radioVision = (float)GuiController.Instance.Modifiers.getValue("RadioVision");
            float anguloVision = (float)GuiController.Instance.Modifiers.getValue("AnguloVision");
            //TODO
            return false;
        }

     

    }


    /* class EnemyFactory : TgcSkeletalLoader.IMeshFactory
    {
        public TgcSkeletalMesh createNewMesh(Mesh d3dMesh, string meshName, TgcSkeletalMesh.MeshRenderType renderType, TgcSkeletalBone[] bones)
        {
            return new Enemy(d3dMesh, meshName, renderType, bones);
        }
    }*/

}