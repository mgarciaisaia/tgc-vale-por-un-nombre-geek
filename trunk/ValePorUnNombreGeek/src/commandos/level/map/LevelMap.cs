using System.Collections.Generic;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map
{
    class LevelMap:Picture
    {
        private Level level;

   
        private Texture g_Posiciones;
        private Surface g_pDepthStencil;

       

        private bool mustUpdateTextureCoords = true;
        private bool mustUpdateValues = true;

        private float zoom;
       
      
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
                mustUpdateValues = true;
                mustUpdateTextureCoords = true;
            }
        }

       
        public Vector3 ViewCenter
        {
            get { return this.viewCenter; }
            set
            {
                this.viewCenter = value;
                mustUpdateTextureCoords = true;
            }
        }
 

        public bool ShowCharacters { get; set; }
        public bool FollowCamera { get; set; }
       
        public LevelMap(Level level, float width, float height, float zoom):base(level.Terrain.Texture, width, height)
        {
            this.level = level;
            this.zoom = zoom;
              
            terrainWidth = (int)level.Terrain.getWidth();
            terrainHeight = (int)level.Terrain.getLength();

            createPositionsTexture(level);

            this.position = new Vector2(CommandosUI.Instance.ScreenWidth - this.width - 10, 10);
            this.Effect = TgcShaders.loadEffect(CommandosUI.Instance.ShadersDir + "mapa.fx");
            this.Technique = "MAPA";
            this.ShowCharacters = true;
            this.FollowCamera = true;
            
        
        }


        #region update
        /// <summary>
        /// Actualiza la posicion de los vertices del rectangulo.
        /// </summary>
        protected override void update()
        {
            if (!mustUpdate) return;
            base.update();
            mustUpdateValues = true;
        
        }

        /// <summary>
        /// Calcula valores que luego van a usarse en updateView para calcular las coordenadas de textura.
        /// </summary>
        private void updateValues()
        {
            if (!mustUpdateValues) return;
            realZoom = FastMath.Max(this.width / terrainWidth, this.height / terrainHeight) * this.zoom;
            widthFactor = terrainWidth / 2 / realZoom * this.width / terrainWidth;
            heightFactor = terrainHeight / 2 / realZoom * this.height / terrainHeight;
            mustUpdateValues = false;
            mustUpdateTextureCoords = true;

        }

        /// <summary>
        /// Calcula las coordenadas de textura que debe tener cada vertice para que en el centro del rectangulo
        /// se vea la posicion que se pasa como center (por default, la posicion de la camara), y con el zoom
        /// correspondiente.
        /// </summary>
        /// <param name="center"></param>
        private void updateTextureCoords()
        {
            if (FollowCamera) viewCenter = CommandosUI.Instance.Camera.getLookAt();
            
            if(!mustUpdateTextureCoords && viewCenter.Equals(previousViewCenter)) return;

            Vector2 centerCoords;
            if (this.level.Terrain.xzToHeightmapCoords(viewCenter.X, viewCenter.Z, out centerCoords))
            {

                Vector2 min = new Vector2((centerCoords.X + widthFactor) / terrainWidth, (centerCoords.Y - heightFactor) / terrainHeight);
                Vector2 max = new Vector2((centerCoords.X - widthFactor) / terrainWidth, (centerCoords.Y + heightFactor) / terrainHeight);


                //Arriba izq
                this.vertices[0].Tu1 = min.X;
                this.vertices[0].Tv1 = min.Y;

                //Arriba der
                this.vertices[1].Tu1 = max.X;
                this.vertices[1].Tv1 = min.Y;

                //Abajo izq
                this.vertices[2].Tu1 = min.X;
                this.vertices[2].Tv1 = max.Y;

                //Abajo der
                this.vertices[3].Tu1 = max.X;
                this.vertices[3].Tv1 = max.Y;
            }

            previousViewCenter = viewCenter;
            mustUpdateTextureCoords = false;
        }

        #endregion


        public override void render()
        {                 
            if (!Enable) return;          
                        
            this.update();
            
            this.updateValues();

            this.updateTextureCoords();         
                     
            renderCharacterPositions();
            
            Effect.SetValue("show_characters", ShowCharacters);
            Effect.SetValue("g_Posiciones", g_Posiciones);

            base.render();
         
        }

             

        public override void dispose()
        {
            base.dispose();
            g_pDepthStencil.Dispose();
            g_Posiciones.Dispose();
        }
      

        #region Characters Positions

        private void createPositionsTexture(Level level)
        {
            Device d3dDevice = CommandosUI.Instance.d3dDevice;

            //Textura auxiliar para renderizar las posiciones de los personajes
            g_Posiciones = new Texture(d3dDevice, terrainWidth, terrainHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
            //Z-Buffer
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(terrainWidth,
                                                                          terrainHeight,
                                                                          DepthFormat.D24S8,
                                                                          MultiSampleType.None,
                                                                          0,
                                                                          true);
        }

        

        private IEnumerable<CustomVertex.TransformedColored[]> getCharacterRectangles()
        {
            List<CustomVertex.TransformedColored[]> rectangles = new List<CustomVertex.TransformedColored[]>();
            float width = zoom;
            float height= width;
            int color;
            CustomVertex.TransformedColored[] vertices;

            foreach (Character c in level.Characters)
            {

                Vector2 position;

                if (c.OwnedByUser) color = Color.Green.ToArgb(); else color = Color.Red.ToArgb();
                level.Terrain.xzToHeightmapCoords(c.Position.X, c.Position.Z, out position);


                vertices = new CustomVertex.TransformedColored[4];


                //Arriba izq
                vertices[0] = new CustomVertex.TransformedColored(position.X, position.Y, 0, 1, color);
                //Arriba der
                vertices[1] = new CustomVertex.TransformedColored(position.X + width, position.Y, 0, 1, color);
                //Abajo izq
                vertices[2] = new CustomVertex.TransformedColored(position.X, position.Y + height, 0, 1, color);
                //Abajo der
                vertices[3] = new CustomVertex.TransformedColored(position.X + width, position.Y + height, 0, 1, color);

                rectangles.Add(vertices);

            }

            return rectangles;
        }

        private void renderCharacterPositions()
        {
            if (!ShowCharacters) return;

            Microsoft.DirectX.Direct3D.Device device = CommandosUI.Instance.d3dDevice;
           
                      
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

       #endregion 
       
        
    
    
    }
}
