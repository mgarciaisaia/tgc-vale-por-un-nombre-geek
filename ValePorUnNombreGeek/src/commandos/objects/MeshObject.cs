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
    class MeshObject : LevelObject
    {
        protected TgcMesh mesh;
        protected float radius;
        protected Vector3 center;

        public MeshObject(string path, Vector3 position, Vector3 scale, Vector3 rotation)
        {

           
            TgcSceneLoader loader = new TgcSceneLoader();
                   
            TgcScene sceneOriginal = loader.loadSceneFromFile(path);
           
            mesh = sceneOriginal.Meshes[0];
            mesh.Position = position;
            mesh.Scale = scale;
            mesh.Rotation = rotation;
            center = mesh.BoundingBox.calculateBoxCenter();
            radius = mesh.BoundingBox.calculateBoxRadius();
        }

        public override Vector3 Position
        {
            get { return mesh.Position; }
            set { this.mesh.Position = value; } //setter necesario para ILevelObject (solo sirve en SkeletalRepresentation)
        }

        public override TgcBoundingBox BoundingBox
        {
            get { return mesh.BoundingBox; }
        }

        public override Vector3 Center
        {
            get { return this.center; }
        }

        public override float Radius
        {
            get { return this.radius; }
        }

        public override Effect Effect
        {
            get{ return mesh.Effect;}
            set{mesh.Effect = value;}
        }

        public override string Technique
        {
            get{ return mesh.Technique;}
            set{mesh.Technique = value;}
        }

        public override void render()
        {
            mesh.render();                     
        }

        public override void dispose()
        {
            mesh.dispose();
        }
    }
}
