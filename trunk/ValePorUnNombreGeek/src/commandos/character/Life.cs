using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Life
    {
        protected float points;
        protected float maxPoints;
        protected bool vertical;
        protected Vector2 traslation;
        protected Character character;
        protected Effect effect;
        protected Vector2 size;
        protected Color color;
        protected CustomVertex.TransformedColoredTextured[] vertices;
        protected Vector2 position;
        protected bool mustUpdate;
        protected string technique;
        protected CharacterPicture picture;
        public CharacterPicture Picture{get{return picture;}
            set{
                this.picture = value;
                this.picture.Position = position;
                this.position = traslation + new Vector2(this.Picture.Width, 0);
                this.picture.Width = size.Y / this.picture.Height * this.picture.Width;
                this.picture.Height = size.Y;
            }}

        public Color Color
        {
            get { return color; }
            set { this.color = value; mustUpdate = true;}
        }

        public Vector2 Position
        {
            get {if(this.Picture!=null) return this.Picture.Position; else return this.position; }
            set
            {
                this.traslation = value;
                if (this.Picture != null)
                {
                    this.Picture.Position = value; this.position = this.traslation + new Vector2(this.Picture.Width, 0);

                }
                else this.position = value;
                mustUpdate = true;
            }
        }



        public Vector2 Size
        {
            get { return size; }
            set { this.size = value; mustUpdate = true; }
        }

        public float Width
        {
            get { return this.size.X + ((this.Picture!=null)?this.Picture.Width:0); }
            
        }

        public float Height
        {
            get { return this.size.Y; }

        }



        public Effect Effect
        {
            get { return effect; }
            set { this.effect = value; }
        }

        public string Technique
        {
            get { return technique; }
            set { this.technique = value; }
        }

        public float MaxPoints
        {
            get { return maxPoints; }
            set { this.maxPoints = value; }
        }


        public Life(Character character, float maxPoints, Vector2 size, Color color, Vector2 position)
        {
            this.maxPoints = maxPoints;
            this.points = maxPoints;
            this.character = character;
            this.traslation = position;
           
            this.size = size;
            this.color = color;
            this.mustUpdate = true;
            this.effect = TgcShaders.loadEffect(CommandosUI.Instance.ShadersDir + "life.fx");
            this.technique = "COLOR";
            
        }

        protected void createBar(){
            
            this.vertical = this.size.Y > this.size.X;
            vertices = new CustomVertex.TransformedColoredTextured[4];
            int color = this.color.ToArgb();
          
            //Arriba izq
            this.vertices[0] = new CustomVertex.TransformedColoredTextured(position.X, position.Y, 0, 1, color, 0, 0);
            //Arriba der
            this.vertices[1] = new CustomVertex.TransformedColoredTextured(position.X, position.Y+size.Y, 0, 1,color, 0, 1);
            //Abajo izq
            this.vertices[2] = new CustomVertex.TransformedColoredTextured(position.X+ size.X, position.Y, 0, 1, color, 1, 0);
            //Abajo der
            this.vertices[3] = new CustomVertex.TransformedColoredTextured(position.X+size.X, position.Y+size.Y, 0, 1, color, 1, 1);

            this.mustUpdate = false;
        }

        public void render()
        {

            Microsoft.DirectX.Direct3D.Device device = CommandosUI.Instance.d3dDevice;
            string technique;
            if (mustUpdate) createBar(); 

            if (vertical)
            {
                effect.SetValue("pointsY", 1-points/maxPoints);
                effect.SetValue("pointsX", 1);
                technique = this.technique + "_VERTICAL";
            }
            else
            {
                effect.SetValue("pointsX", points / maxPoints);
                effect.SetValue("pointsY", 0);
                technique = this.technique + "_HORIZONTAL";
            }

            effect.Technique = technique;


            int passes = effect.Begin(0);
            for (int i = 0; i < passes; i++)
            {
                effect.BeginPass(i);
                device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);
                effect.EndPass();
            }
            effect.End();

            if(this.Picture!=null)this.Picture.render();
        }

        public void decrement(float points)
        {
            if (this.points > 0)
            {
                this.points -= points;
                if (this.points <= 0)
                {
                    this.points = 0;
                    this.character.die();
                }
            }
        }

        public void dispose()
        {
            effect.Dispose();
            if(this.Picture!=null)Picture.dispose();
           
        }

        public void add()
        {
            this.points += points;
            if (this.points > maxPoints)
            {
                this.points = maxPoints;
               
            }

        }
    }
}
