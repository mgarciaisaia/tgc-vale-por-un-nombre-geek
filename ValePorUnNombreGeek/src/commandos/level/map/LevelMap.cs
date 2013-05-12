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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map
{
    class LevelMap
    {
        private Level level;

        private Texture texDiffuseMap;
        private Texture textHeightmap;

        private Texture g_Posiciones;
        private Surface g_pDepthStencil;

        private MyVertex.TransformedTextured[] vertices;

  
      
        private float width;
        private float height;
        private Vector2 position;

        private bool mustUpdateRectangle = true;
        private bool mustUpdateView = true;
        private bool mustUpdateProportions = true;

        private float zoom;
       

        private bool followCamera;        
        private Vector3 previousViewCenter;
        private Vector3 viewCenter;


        private int terrainWidth;
        private int terrainHeight;
        private float realZoom;
        private float widthFactor;
        private float heightFactor;




     
      
       

        public float Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value; 
                mustUpdateProportions = true;
                mustUpdateView = true;
            }
        }

       
        public Vector3 ViewCenter
        {
            get { return this.viewCenter; }
            set
            {
                this.viewCenter = value;
                mustUpdateView = true;
            }
        }
        public Vector2 Position{

            get{ return this.position;}
            set { this.position = value; mustUpdateRectangle = true; }
            
        }

        public bool ShowCharacters { get; set; }

        public Effect Effect { get; set; }

        public string Technique { get; set; }

        public bool Enabled { get; set; }
       
        public float Width { get { return this.width; } set { this.width = value; mustUpdateRectangle = true; } }

        public float Height { get { return this.height; } set { this.height = value; mustUpdateRectangle = true; } }

       

        public bool FollowCamera
        {
            get { return this.followCamera; }
            set { this.followCamera = value;}
        }
 
        public LevelMap(Level level, float width, float height, float zoom)
        {
            this.level = level;
            this.zoom = zoom;
            this.width= width;
            this.height= height;

            terrainWidth = (int)level.Terrain.getWidth();
            terrainHeight = (int)level.Terrain.getLength();

            createTextures(level);

            this.position = new Vector2(GuiController.Instance.Panel3d.Width-this.width-10,10);
            this.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\mapa.fx");
            this.Technique = "MAPA";
            this.Enabled = true;
            this.ShowCharacters = true;
            this.followCamera = true;
           
        }

        private void createTextures(Level level)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            Bitmap bitmap;
            
            //Textura del terreno
            bitmap = (Bitmap)Bitmap.FromFile(level.Terrain.TexturePath);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            texDiffuseMap = Texture.FromBitmap(d3dDevice, bitmap, Usage.None, Pool.Managed);

            //Heightmap por si se quiere dar un efecto segun la altura
            bitmap = (Bitmap)Bitmap.FromFile(level.Terrain.HeightmapPath);
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            textHeightmap = Texture.FromBitmap(d3dDevice, bitmap, Usage.None, Pool.Managed);

            //Textura auxiliar para renderizar las posiciones de los personajes
            g_Posiciones = new Texture(d3dDevice, terrainWidth, terrainHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(terrainWidth,
                                                                          terrainHeight,
                                                                          DepthFormat.D24S8,
                                                                          MultiSampleType.None,
                                                                          0,
                                                                          true);
        }

        
        

        public void render()
        {                 
            if (!Enabled) return;          

            if (mustUpdateRectangle) this.updateRectangle();
            if (mustUpdateProportions) this.updateProportions();

            if (followCamera)
            {
                viewCenter = GuiController.Instance.CurrentCamera.getPosition();
            }

            if (!viewCenter.Equals(previousViewCenter)) mustUpdateView = true;
           
            
            if(mustUpdateView)updateView(viewCenter);         

         
           //Renderizo las posiciones de los pj en una textura.
            if(ShowCharacters)renderCharacterPositions();
            
            //Renderizo el mapa
            renderMap();

         
        }

       
        private void renderCharacterPositions()
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
           
                      
            //Renderizo posiciones de personajes sobre una textura

            Surface pOldRT;
            Surface pOldDS;

            device.EndScene();


           
           
            //Obtengo la superficie para renderizar
            Surface pSurf = g_Posiciones.GetSurfaceLevel(0);

            //Cambio la superficie en la que se hace el render por la superficie de posiciones.
            pOldRT = device.GetRenderTarget(0);
            pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;
            device.SetRenderTarget(0, pSurf);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            
            //Renderizo los rectangulitos.
            device.BeginScene();

            foreach (CustomVertex.TransformedColored[] characterRectangle in this.getCharacterRectangles())
            {

                device.VertexFormat = CustomVertex.TransformedColored.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, characterRectangle);
            }

            device.EndScene();

            //Dejo el device como estaba antes.
            device.BeginScene();
            device.DepthStencilSurface = pOldDS;
            device.SetRenderTarget(0, pOldRT);


           
        }

        private void renderMap()
        {
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            Effect.Technique = Technique;
            Effect.SetValue("texDiffuseMap", texDiffuseMap);
            Effect.SetValue("texHeightMap", textHeightmap);

            if (ShowCharacters)
            {
                Effect.SetValue("g_Posiciones", g_Posiciones);
                Effect.Technique = Technique + "_POSICIONES";
            }


            texturesManager.clear(1);

            int passes = Effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                Effect.BeginPass(i);
                device.VertexDeclaration = MyVertex.TransformedTexturedDeclaration;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                Effect.EndPass();
            }
            Effect.End();
        }


        private IEnumerable<CustomVertex.TransformedColored[]> getCharacterRectangles()
        {
            List<CustomVertex.TransformedColored[]> rectangles = new List<CustomVertex.TransformedColored[]>();
            int width = 2;
            int height = 2;
            int color;
            CustomVertex.TransformedColored[] vertices;

            foreach(Character c in level.Characters){
                
                Vector2 position;

                if (c.OwnedByUser) color = Color.Green.ToArgb(); else color = Color.Red.ToArgb();
                level.Terrain.xzToHeightmapCoords(c.Position.X, c.Position.Z, out position);


                vertices =  new CustomVertex.TransformedColored[4];
          
                      
                 //Arriba izq
                 vertices[0] = new CustomVertex.TransformedColored(position.X, position.Y, 0, 1, color);
                //Arriba der
                 vertices[1] = new CustomVertex.TransformedColored(position.X + width, position.Y, 0, 1, color);
                //Abajo izq
                 vertices[2] = new CustomVertex.TransformedColored(position.X, position.Y + height,0, 1, color);
                //Abajo der
                 vertices[3] = new CustomVertex.TransformedColored(position.X + width, position.Y + height, 0, 1,color);

                 rectangles.Add(vertices);

            }

            return rectangles;
        }


        private void updateRectangle()
        {


            vertices = new MyVertex.TransformedTextured[4];


            //Arriba izq
            this.vertices[0] = new MyVertex.TransformedTextured(position.X, position.Y, 0, 1, 0, 0);
            //Arriba der
            this.vertices[1] = new MyVertex.TransformedTextured(position.X + Width, position.Y, 0, 1, 0, 0);
            //Abajo izq
            this.vertices[2] = new MyVertex.TransformedTextured(position.X, position.Y + Height, 0, 1, 0, 0);
            //Abajo der
            this.vertices[3] = new MyVertex.TransformedTextured(position.X + Width, position.Y + Height, 0, 1, 0, 0);

            mustUpdateRectangle = false;
            mustUpdateProportions = true;
            mustUpdateView = true;
        }
        

        private void updateProportions()
        {

            realZoom = FastMath.Max(this.width / terrainWidth, this.height / terrainHeight) * this.zoom;
            widthFactor = terrainWidth / 2 / realZoom * this.width / terrainWidth;
            heightFactor = terrainHeight / 2 / realZoom * this.height / terrainHeight;
            mustUpdateProportions = false;
            mustUpdateView = true;

        }
        

        private void updateView(Vector3 center)
        {
          
           
            Vector2 cameraCoords;
            if (this.level.Terrain.xzToHeightmapCoords(center.X, center.Z, out cameraCoords))
            {                
                float minX = (cameraCoords.X + widthFactor) / terrainWidth;
                float maxX = (cameraCoords.X - widthFactor) / terrainWidth;
                float minY = (cameraCoords.Y - heightFactor ) / terrainHeight;
                float maxY = (cameraCoords.Y + heightFactor) / terrainHeight;
               
                //Arriba izq
                this.vertices[0].Tu1 = minX;
                this.vertices[0].Tv1 = minY;

                //Arriba der
                this.vertices[1].Tu1 = maxX;
                this.vertices[1].Tv1 = minY;

                //Abajo izq
                this.vertices[2].Tu1 = minX;
                this.vertices[2].Tv1 = maxY;

                //Abajo der
                this.vertices[3].Tu1 = maxX;
                this.vertices[3].Tv1 = maxY;
            }

            previousViewCenter = center;
            mustUpdateView = false;
        }


       public void dispose(){
           Effect.Dispose();
           textHeightmap.Dispose();
           texDiffuseMap.Dispose();
           g_pDepthStencil.Dispose();
           g_Posiciones.Dispose();
       }

  
    }
}
