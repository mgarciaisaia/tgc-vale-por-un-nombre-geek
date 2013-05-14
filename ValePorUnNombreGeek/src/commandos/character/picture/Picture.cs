using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.picture
{
    class Picture
    {
        protected Texture texture;
        protected CustomVertex.TransformedTextured[] vertices;
        protected Vector2 position;
        protected float width;
        protected float height;
        protected bool mustUpdate;

        public Picture(string path)
        {
            this.texture = TextureLoader.FromFile(GuiController.Instance.D3dDevice, path);
            this.Technique = "DIFFUSE_MAP";
            this.information = TextureLoader.ImageInformationFromFile(path);
            this.Width = information.Width;
            this.Height = information.Height;
            this.Effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "picture.fx");
            vertices = new CustomVertex.TransformedTextured[4];
            mustUpdate = true;
         
        }

        public virtual void render()
        {
            Device device = GuiController.Instance.D3dDevice;
            if (mustUpdate) update();
            Effect.SetValue("texDiffuseMap", texture);
            Effect.Technique = Technique;
            Effect.Begin(0);
            Effect.BeginPass(0);
            device.VertexFormat = CustomVertex.TransformedTextured.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
            Effect.EndPass();
            Effect.End();

        }
        private void update()
        {

            //Arriba izq
            vertices[0] = new CustomVertex.TransformedTextured(Position.X, Position.Y, 0, 1, 0, 0);
            //Arriba der
            vertices[1] = new CustomVertex.TransformedTextured(Position.X + Width, Position.Y, 0, 1, 1, 0);
            //Abajo izq
            vertices[2] = new CustomVertex.TransformedTextured(Position.X, Position.Y + Height, 0, 1, 0, 1);
            //Abajo der
            vertices[3] = new CustomVertex.TransformedTextured(Position.X + Width, Position.Y + Height, 0, 1, 1, 1);

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
    }
}
