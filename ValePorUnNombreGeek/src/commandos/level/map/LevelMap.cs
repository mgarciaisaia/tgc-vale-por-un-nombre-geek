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
        private Texture texDiffuseMap;
        private Texture textHeightmap;
        private Texture g_Posiciones;
        private Surface g_pDepthStencil;
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

        int terrainWidth;
        int terrainHeight;
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
            this.updateProportions();
            Device d3dDevice = GuiController.Instance.D3dDevice;
            g_Posiciones = new Texture(GuiController.Instance.D3dDevice, terrainWidth,
                                                                         terrainHeight,
                                    1, Usage.RenderTarget, Format.X8R8G8B8,
                                    Pool.Default);
            g_pDepthStencil = GuiController.Instance.D3dDevice.CreateDepthStencilSurface(terrainWidth,
                                                                          terrainHeight,
                                                                          DepthFormat.D24S8,
                                                                          MultiSampleType.None,
                                                                          0,
                                                                          true);

            
           
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
        int a = 0;
        public void render()
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            if (!enabled) return;
            if (mustUpdate) this.crearRectangulo();
            if (mustRecalculate) this.updateProportions();
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            actualizarVista();

         
           //Renderizo las posiciones de los pj en una textura.
            renderCharacterPositions();
                            

            //Renderizo el mapa
            effect.Technique = technique;
            effect.SetValue("texDiffuseMap", texDiffuseMap);
            effect.SetValue("texHeightMap", textHeightmap);
            texturesManager.clear(1);
            int passes = effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                effect.BeginPass(i);
                device.VertexFormat = CustomVertex.TransformedTextured.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                effect.EndPass();
            }
            effect.End();


            //Renderizo las posiciones
            bool alphaBlendEnabled = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = true;
      
            effect.Technique = "MAPA";
            effect.SetValue("texDiffuseMap", g_Posiciones);
            texturesManager.clear(1);
            passes = effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                effect.BeginPass(i);
                device.VertexFormat = CustomVertex.TransformedTextured.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                effect.EndPass();
            }
            effect.End();

            device.RenderState.AlphaBlendEnable = alphaBlendEnabled;
        }

        private void renderCharacterPositions()
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            Surface pOldRT;
            Surface pOldDS;
            //Renderizo posiciones de personajes

            device.EndScene();


            //Guardo el render target actual
            pOldRT = device.GetRenderTarget(0);

            //Obtengo la superficie para renderizar
            Surface pSurf = g_Posiciones.GetSurfaceLevel(0);

            pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;

            device.SetRenderTarget(0, pSurf);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            device.BeginScene();

            foreach (CustomVertex.TransformedColored[] characterRectangle in this.getCharacterRectangles())
            {

                device.VertexFormat = CustomVertex.TransformedColored.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, characterRectangle);
            }



            if (a == 0)
            {
                TextureLoader.Save("test.bmp", ImageFileFormat.Bmp, g_Posiciones);
                a++;
                GuiController.Instance.Logger.log("listo");

            }


            device.EndScene();
            device.BeginScene();


            device.DepthStencilSurface = pOldDS;
            device.SetRenderTarget(0, pOldRT);
                
        }

        private IEnumerable<CustomVertex.TransformedColored[]> getCharacterRectangles()
        {
            List<CustomVertex.TransformedColored[]> rectangles = new List<CustomVertex.TransformedColored[]>();
            int width = 2;
            int height = 2;
            int color = Color.Green.ToArgb();
            CustomVertex.TransformedColored[] vertices;

            foreach(Commando c in level.Commandos){
                Vector2 position;
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

        private void updateProportions()
        {

            terrainWidth = (int)level.Terrain.getWidth();
            terrainHeight = (int)level.Terrain.getLength();
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
