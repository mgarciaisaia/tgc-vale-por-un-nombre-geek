using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map
{
    class Map
    {
        private Texture texDiffuseMap;
        private Texture textHeightmap;
        private CustomVertex.TransformedTextured[] vertices;
        private Vector2 position;
        private Effect effect;
        private bool mustUpdate = true;
        private string technique;
         private Level level;
        private float zoom;
        private Vector3 previousCameraPosition;
        private float width;
        private float height;
        private bool enabled = true;


        public Effect Effect
        {

            set { this.effect = value; }
            get { return this.effect; }
        }
        public string Technique
        {

            set { this.technique = value; }
            get { return this.technique; }
        }
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        
        public Vector2 Position{

            get{ return this.position;}
            set { this.position = value; mustUpdate = true; }
        }
        
        public Map(Level level)
        {
            this.level = level;
            this.zoom = 2;
            this.width = 100;
            this.height = 100;
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(level.Terrain.TexturePath);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            texDiffuseMap = Texture.FromBitmap(GuiController.Instance.D3dDevice, bitmap, Usage.None, Pool.Managed);
                 
            bitmap = (Bitmap)Bitmap.FromFile(level.Terrain.HeightmapPath);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            textHeightmap = Texture.FromBitmap(GuiController.Instance.D3dDevice, bitmap, Usage.None, Pool.Managed);
            
            this.position = new Vector2(GuiController.Instance.Panel3d.Width-this.width-10,10);
            this.effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\mapa.fx");
            this.technique = "MAPA";
           
        }

        private void crearRectangulo(){


            vertices =  new CustomVertex.TransformedTextured[4];
          
          
            
             //Arriba izq
            this.vertices[0] = new CustomVertex.TransformedTextured(position.X, position.Y, 0, 1, 0, 0 );
            //Arriba der
            this.vertices[1] = new CustomVertex.TransformedTextured(position.X+Width, position.Y, 0, 1, 0, 0);
            //Abajo izq
            this.vertices[2] = new CustomVertex.TransformedTextured(position.X, position.Y+Height, 0, 1, 0, 0);
            //Abajo der
            this.vertices[3] = new CustomVertex.TransformedTextured(position.X+Width, position.Y+Height, 0, 1, 0, 0);

            mustUpdate = false;
        }

        public void render()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            if (!enabled) return;
            if (mustUpdate) this.crearRectangulo();

            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            actualizarVista();
                                  
            //Renderizo textura
            effect.Technique = technique;
            effect.SetValue("texDiffuseMap", texDiffuseMap);
            effect.SetValue("texHeightMap", textHeightmap);
            texturesManager.clear(1);
            int passes = effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                effect.BeginPass(i);
                d3dDevice.VertexFormat = CustomVertex.TransformedTextured.Format;
                d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                effect.EndPass();
            }
            effect.End();
            
        }



        private void actualizarVista()
        {
            Vector3 cameraPosition = GuiController.Instance.CurrentCamera.getPosition();
            if (previousCameraPosition != null && previousCameraPosition.Equals(cameraPosition)) return;
           
            Vector2 cameraCoords;
            if (this.level.Terrain.xzToHeightmapCoords(cameraPosition.X, cameraPosition.Z, out cameraCoords))
            {
                float width = level.Terrain.getWidth();
                float height = level.Terrain.getLength();
                float widthFactor = width / 2/ zoom;
                float heightFactor = height / 2 / zoom;

                
                float minX = (cameraCoords.X + widthFactor) / width;
                float maxX = (cameraCoords.X - widthFactor) / width;
                float minY = (cameraCoords.Y - heightFactor ) / height;
                float maxY = (cameraCoords.Y + heightFactor) / height;
                

                //Arriba izq
               
                this.vertices[0].Tu = minX;
                this.vertices[0].Tv = minY;

                //Arriba der
                
                this.vertices[1].Tu = maxX;
                this.vertices[1].Tv = minY;

                //Abajo izq
              
                this.vertices[2].Tu = minX;
                this.vertices[2].Tv = maxY;

                //Abajo der
               
                this.vertices[3].Tu = maxX;
                this.vertices[3].Tv = maxY;
            }
            previousCameraPosition = cameraPosition;
        }


        public float Width { get { return this.width; } set { this.width = value; mustUpdate = true; } }

        public float Height { get { return this.height; } set { this.height = value; mustUpdate = true; } }
    }
}
