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

namespace AlumnoEjemplos.MiGrupo.ValePorUnNombreGeek.Enemy
{
   
    public class Enemy : TgcSkeletalMesh
    {
        

       
        public Enemy(Mesh mesh, string name, MeshRenderType renderType, TgcSkeletalBone[] bones)
            : base(mesh, name, renderType, bones)
        {
            GuiController.Instance.Modifiers.addFloat("RadioVision", 0, 1000, 500);
            GuiController.Instance.Modifiers.addFloat("AnguloVision",0,360,90);
        }

        public bool puedeVer(TgcBox target)
        {

            float radioVision = (float)GuiController.Instance.Modifiers.getValue("RadioVision");
            float anguloVision = (float)GuiController.Instance.Modifiers.getValue("AnguloVision");
            
         
            return false;
        }

     

    }

    public class EnemyFactory : TgcSkeletalLoader.IMeshFactory
    {
        public TgcSkeletalMesh createNewMesh(Mesh d3dMesh, string meshName, TgcSkeletalMesh.MeshRenderType renderType, TgcSkeletalBone[] bones)
        {
            return new Enemy(d3dMesh, meshName, renderType, bones);
        }
    }

}