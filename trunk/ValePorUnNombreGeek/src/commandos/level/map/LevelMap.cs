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
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map
{
    class LevelMap
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



        //Para calculos de proporciones de cosas..

        float terrainWidth;
        float terrainHeight;
        float realZoom;
        float widthFactor;
        float heightFactor;

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
        private bool mustRecalculate = true;

        public float Zoom
        {
            get { return this.zoom; }
            set { this.realZoom = value; mustRecalculate = true; }
        }
        
        public Vector2 Position{

            get{ return this.position;}
            
        }
        
        public LevelMap(Level level, float width, float height, float zoom)
        {
            this.level = level;
            this.zoom = zoom;
            this.width= width;
            this.height= height;
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
            mustRecalculate = true;
        }

        public void render()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            if (!enabled) return;
            if (mustUpdate) this.crearRectangulo();
            if (mustRecalculate) this.updatePropotions();
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

        private void updatePropotions()
        {

            terrainWidth = level.Terrain.getWidth();
            terrainHeight = level.Terrain.getLength();
            realZoom = FastMath.Max(this.width / terrainWidth, this.height / terrainHeight) * this.zoom;
            widthFactor = terrainWidth / 2 / realZoom * this.width / terrainWidth;
            heightFactor = terrainHeight / 2 / realZoom * this.height / terrainHeight;
            mustRecalculate = false;
        }



        private void actualizarVista()
        {
            Vector3 cameraPosition = GuiController.Instance.CurrentCamera.getPosition();
            if (previousCameraPosition != null && previousCameraPosition.Equals(cameraPosition)) return;
           
            Vector2 cameraCoords;
            if (this.level.Terrain.xzToHeightmapCoords(cameraPosition.X, cameraPosition.Z, out cameraCoords))
            {
           

                
                float minX = (cameraCoords.X + widthFactor) / terrainWidth;
                float maxX = (cameraCoords.X - widthFactor) / terrainWidth;
                float minY = (cameraCoords.Y - heightFactor ) / terrainHeight;
                float maxY = (cameraCoords.Y + heightFactor) / terrainHeight;
                

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

        public void setPosition(Vector2 vector2)
        {
            this.position = vector2; 
            mustUpdate = true;
        }
    }
}
