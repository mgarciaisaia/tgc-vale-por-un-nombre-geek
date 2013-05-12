using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class Sprite
    {
        public Sprite(string _filePath)
        {
            this.texture = TextureLoader.FromFile(GuiController.Instance.D3dDevice, _filePath, 0, 0, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default, Filter.Linear, Filter.Linear, Color.Magenta.ToArgb(), ref this.imageInformation);
            
            this.transformationMatrix = Matrix.Identity;

            //Set an empty rectangle to indicate the entire bitmap.
            this.regionRectangle = Rectangle.Empty;

            this.position = new Vector2(0, 0);
            //this.scale = new Vector2(0.90f, 0.9f);
            this.scale = new Vector2(1f, 1f);
            this.scalingCenter = new Vector2(0, 0);
            this.rotation = 0;
            this.rotationCenter = new Vector2(0, 0);
            this.color = Color.White;

            this.updateTransformationMatrix();
        }

        public void dispose()
        {
            this.texture.Dispose();
        }

        #region GettersAndSetters

        private ImageInformation imageInformation;
        public ImageInformation ImageInformation
        {
            get { return this.imageInformation; }
        }

        private Texture texture;
        public Texture Texture
        {
            get { return this.texture; }
        }

        private Matrix transformationMatrix;
        public Matrix TransformationMatrix
        {
            get { return this.transformationMatrix; }
        }

        private Rectangle regionRectangle;
        /// <summary>
        /// Region del bitmap original a dibujar
        /// </summary>
        public Rectangle RegionRectangle
        {
            get { return this.regionRectangle; }
            set { this.regionRectangle = value; }
        }

        private Color color; //sera necesario????
        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        private Vector2 position;
        public Vector2 Position
        {
            get { return this.position; }
            set {
                this.position = value;
                this.updateTransformationMatrix();
            }
        }

        private float rotation;
        public float Rotation
        {
            get { return this.rotation; }
            set {
                this.rotation = value;
                this.updateTransformationMatrix();
            }
        }

        private Vector2 rotationCenter;
        /// <summary>
        /// Centro de rotacion
        /// </summary>
        public Vector2 RotationCenter
        {
            get { return this.rotationCenter; }
            set
            {
                this.rotationCenter = value;
                this.updateTransformationMatrix();
            }
        }

        private Vector2 scalingCenter;
        /// <summary>
        /// Centro de referencia para la escala
        /// </summary>
        public Vector2 ScalingCenter
        {
            get { return this.scalingCenter; }
            set { this.scalingCenter = value; this.updateTransformationMatrix(); }
        }

        private Vector2 scale;
        public Vector2 Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                this.updateTransformationMatrix();
            }
        }

        public float Width
        {
            get { return (int)(this.ImageInformation.Width * this.Scale.X); }
            set
            {
                this.scale.X = value / this.ImageInformation.Width;
                this.updateTransformationMatrix();
            }
        }

        public float Height
        {
            get { return (int)(this.ImageInformation.Height * this.Scale.Y); }
            set { 
                    this.scale.Y = value / this.ImageInformation.Height; 
                    this.updateTransformationMatrix(); 
            }
        }

        #endregion

        private void updateTransformationMatrix()
        {
            this.transformationMatrix = Matrix.Transformation2D(
                this.ScalingCenter,
                0,
                this.Scale,
                this.RotationCenter,
                this.Rotation,
                this.Position);
        }
    }
}
