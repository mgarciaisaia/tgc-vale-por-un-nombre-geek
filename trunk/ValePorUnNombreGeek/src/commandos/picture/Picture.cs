using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class Picture
    {
        protected Texture texDiffuseMap;
        protected Texture g_Mask;
        protected Texture g_Frame;

        protected MyVertex.TransformedDoubleTextured[] vertices;
        public MyVertex.TransformedDoubleTextured[] Vertices { get; set; }
        protected Vector2 position;
        protected float width;
        protected float height;
        protected bool mustUpdate;

        public bool Enable { get; set; }
        public Effect Effect { get; set; }
        public string FrameTechnique { get; set; }
        public string Technique { get; set; }
        public Vector2 Position { get { return this.position; } set { this.position = value; mustUpdate = true; } }
        public float Width { get { return this.width; } set { this.width = value; mustUpdate = true; } }
        public float Height { get { return this.height; } set { this.height = value; mustUpdate = true; } }
        public bool AlphaBlendEnable { get; set; }
        public bool FrameEnable { get; set; }
        public bool MaskEnable { get; set; }

        public Picture(string path)
        {
            ImageInformation information = TextureLoader.ImageInformationFromFile(path);

            this.init(TextureLoader.FromFile(GuiController.Instance.D3dDevice, path), information.Width, information.Height);
        }

        public Picture(string path, float width, float height)
        {
            this.init(TextureLoader.FromFile(GuiController.Instance.D3dDevice, path), width, height);
        }

        public Picture(Texture texture, float width, float height)
        {
            this.init(texture, width, height);
        }

        protected virtual void init(Texture texture, float width, float height)
        {
            this.MaskEnable = false;
            this.FrameEnable = false;
            this.Position = new Vector2(0, 0);
            this.texDiffuseMap = texture;
            this.Technique = "DIFFUSE_MAP";
            this.FrameTechnique = "FRAME";
            this.Width = width;
            this.Height = height;
            this.Effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "picture.fx");
            this.Enable = true;
           
           
            mustUpdate = true;

        }

        public void setMask(Texture mask)
        {
            g_Mask = mask;
            this.MaskEnable = true;
        }

        public void setFrame(Texture frame)
        {
            g_Frame = frame;
            this.FrameEnable = true;
        }

        public virtual void render()
        {
            if (!Enable) return;

            Device device = GuiController.Instance.D3dDevice;
            bool alphaBlendEnable = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = AlphaBlendEnable||MaskEnable;
            
           
            if (mustUpdate) update();

            if (MaskEnable)
            {
                Effect.SetValue("mask_enable", true);
                Effect.SetValue("g_mask", g_Mask);

            } else Effect.SetValue("mask_enable", false);

            Effect.SetValue("texDiffuseMap", texDiffuseMap);
            Effect.Technique = Technique;
            Effect.Begin(0);
            Effect.BeginPass(0);
            device.VertexDeclaration = MyVertex.TransformedDoubleTexturedDeclaration;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
            Effect.EndPass();
            Effect.End();

            device.RenderState.AlphaBlendEnable = alphaBlendEnable;
           
            if (FrameEnable) renderFrame();
        }

        protected virtual void renderFrame()
        {

            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            bool alphaBlendEnable = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = true;

            Effect.Technique = this.FrameTechnique;
            Effect.SetValue("g_frame", g_Frame);
            
            texturesManager.clear(1);

            int passes = Effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                Effect.BeginPass(i);
                device.VertexDeclaration = MyVertex.TransformedDoubleTexturedDeclaration;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                Effect.EndPass();
            }
            Effect.End();

            device.RenderState.AlphaBlendEnable = alphaBlendEnable;
        }

        protected virtual void update()
        {

            vertices = new MyVertex.TransformedDoubleTextured[4];


            //Arriba izq
            this.vertices[0] = new MyVertex.TransformedDoubleTextured(position.X, position.Y, 0, 1, 0, 0, 0, 0);
            //Arriba der
            this.vertices[1] = new MyVertex.TransformedDoubleTextured(position.X + Width, position.Y, 0, 1, 1, 0, 1, 0);
            //Abajo izq
            this.vertices[2] = new MyVertex.TransformedDoubleTextured(position.X, position.Y + Height, 0, 1, 0, 1, 0, 1);
            //Abajo der
            this.vertices[3] = new MyVertex.TransformedDoubleTextured(position.X + Width, position.Y + Height, 0, 1, 1, 1, 1, 1);

            mustUpdate = false;
        }

        public virtual void dispose()
        {
            this.texDiffuseMap.Dispose();
            this.Effect.Dispose();
            if (this.g_Frame != null) g_Frame.Dispose();
            if (this.g_Mask != null) g_Mask.Dispose();
        }

       
    }
}
