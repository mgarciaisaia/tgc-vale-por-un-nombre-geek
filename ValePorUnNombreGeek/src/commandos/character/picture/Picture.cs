using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.picture
{
    class Picture
    {
        protected Texture texture;
        private Texture g_Mask;
        public bool MaskEnable { get; set; }

        protected MyVertex.TransformedDoubleTextured[] vertices;
        public MyVertex.TransformedDoubleTextured[] Vertices { get; set; }
        protected Vector2 position;
        protected float width;
        protected float height;
        protected bool mustUpdate;
        private Texture g_Frame;

        public Picture(string path)
        {
            this.information = TextureLoader.ImageInformationFromFile(path);

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

        protected void init(Texture texture, float width, float height)
        {
            this.MaskEnable = false;
            this.FrameEnable = false;
            this.texture = texture;
            this.Technique = "DIFFUSE_MAP";
            this.Width = width;
            this.Height = height;
            this.Effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "picture.fx");
           
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
            Device device = GuiController.Instance.D3dDevice;
            bool alphaBlendEnable = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = AlphaBlendEnable||MaskEnable;
            
           
            if (mustUpdate) update();

            if (MaskEnable)
            {
                Effect.SetValue("mask_enable", true);
                Effect.SetValue("g_mask", g_Mask);
            }

            Effect.SetValue("texDiffuseMap", texture);
            Effect.Technique = Technique;
            Effect.Begin(0);
            Effect.BeginPass(0);
            device.VertexDeclaration = MyVertex.TransformedDoubleTexturedDeclaration;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
            Effect.EndPass();
            Effect.End();

            device.RenderState.AlphaBlendEnable = alphaBlendEnable;
            if (MaskEnable) Effect.SetValue("mask_enable", false);
            if (FrameEnable) renderFrame();
        }

        private void renderFrame()
        {

            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            bool alphaBlendEnable = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = true;

            Effect.Technique = "FRAME";
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

        private void update()
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

        public void dispose()
        {
            this.texture.Dispose();
        }

        private ImageInformation information { get; set; }
        public Effect Effect{ get; set; }
        public string Technique { get; set; }
        public Vector2 Position { get { return this.position; } set { this.position = value; mustUpdate = true; } }
        public float Width { get { return this.width; } set { this.width = value; mustUpdate = true; } }
        public float Height { get { return this.height; } set { this.height= value; mustUpdate = true; } }

        public bool AlphaBlendEnable { get; set; }

        public bool FrameEnable { get; set; }
    }
}
