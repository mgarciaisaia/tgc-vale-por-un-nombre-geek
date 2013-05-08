﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    class MeshObject:ILevelObject
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

        public Vector3 Position
        {
            get { return mesh.Position; }
        }

        public virtual TgcBoundingBox BoundingBox
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

        public Effect Effect
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
