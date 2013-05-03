using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    class LevelObject:ILevelObject
    {
        TgcMesh mesh;
        float radius;
        Vector3 center;

        public LevelObject(string path, Vector3 position, Vector3 scale)
        {

           
            TgcSceneLoader loader = new TgcSceneLoader();

                   
            TgcScene sceneOriginal = loader.loadSceneFromFile(path);
           
            mesh = sceneOriginal.Meshes[0];
            mesh.Position = position;
            mesh.Scale = scale;
            center = mesh.BoundingBox.calculateBoxCenter();
            radius = mesh.BoundingBox.calculateBoxRadius();
        }
        public Vector3 Position
        {
            get { return mesh.Position; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return mesh.BoundingBox; }
        }

        public Vector3 Center
        {
            get { return this.center; }
        }

        public float Radius
        {
            get { return this.radius; }
        }

        public Effect effect
        {
            get{ return mesh.Effect;}
            set{mesh.Effect = value;}
        }

        public string Technique
        {
            get{ return mesh.Technique;}
            set{mesh.Technique = value;}
        }

        public void render()
        {
            mesh.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }
    }
}
