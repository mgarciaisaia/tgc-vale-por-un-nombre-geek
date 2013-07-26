using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using System.Drawing;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    class Cylinder : IRenderObject, ITransformObject
    {
        private float halfLength;
        private float radius;
        private int color;
        private BoundingCylinder boundingCylinder;

        private const int END_CAPS_RESOLUTION = 40;
        private CustomVertex.PositionColoredTextured[] sideTrianglesVertices; //triangle strip
        private CustomVertex.PositionColoredTextured[] capsTrianglesVertices; //triangle list

        private bool useTexture;
        private TgcTexture texture;
        private Effect effect;
        private string technique;


        public Cylinder(Vector3 _center, float _radius, float _halfLength)
        {
            this.radius = _radius;
            this.halfLength = _halfLength;
            this.boundingCylinder = new BoundingCylinder(_center, _radius, _halfLength);

            this.color = Color.Red.ToArgb();

            this.initialize();
        }

        private void initialize()
        {
            int capsResolution = END_CAPS_RESOLUTION;

            //cara lateral: un vertice por cada vertice de cada tapa, mas dos para cerrarla
            this.sideTrianglesVertices = new CustomVertex.PositionColoredTextured[2 * capsResolution + 2];

            //tapas: dos vertices por cada vertice de cada tapa, mas uno en el centro
            this.capsTrianglesVertices = new CustomVertex.PositionColoredTextured[capsResolution * 3 * 2];

            this.useColorShader();
            this.updateValues();
        }

        private void useColorShader()
        {
            this.effect = GuiController.Instance.Shaders.VariosShader;
            this.technique = TgcShaders.T_POSITION_COLORED;
            this.useTexture = false;
        }

        private void useTextureShader()
        {
            this.technique = TgcShaders.T_POSITION_COLORED_TEXTURED;
            this.useTexture = true;
        }

        private void updateDraw()
        {
            //vectores utilizados para el dibujado
            Vector3 upVector = new Vector3(0, 1, 0);
            Vector3 n = new Vector3(1, 0, 0);

            int capsResolution = END_CAPS_RESOLUTION;

            //matriz de rotacion del vector de dibujado
            float angleStep = FastMath.TWO_PI / (float)capsResolution;
            Matrix rotationMatrix = Matrix.RotationAxis(-upVector, angleStep);
            float angle = 0;

            //transformacion que se le aplicara a cada vertice
            Matrix transformation = this.Transform;

            //arrays donde guardamos los puntos dibujados
            Vector3[] topCapDraw = new Vector3[capsResolution];
            Vector3[] bottomCapDraw = new Vector3[capsResolution];

            //Vector3 topCapCenter = Vector3.TransformCoordinate(upVector, transformation);
            //Vector3 bottomCapCenter = Vector3.TransformCoordinate(-upVector, transformation);
            Vector3 topCapCenter = upVector;
            Vector3 bottomCapCenter = -upVector;

            for (int i = 0; i < capsResolution; i++)
            {
                //establecemos los vertices de las tapas
                //topCapDraw[i] = Vector3.TransformCoordinate(upVector + n, transformation);
                //bottomCapDraw[i] = Vector3.TransformCoordinate(-upVector + n, transformation);
                topCapDraw[i] = upVector + n;
                bottomCapDraw[i] = -upVector + n;

                float u = angle / FastMath.TWO_PI;

                //triangulos de la cara lateral (strip)
                this.sideTrianglesVertices[2 * i] = new CustomVertex.PositionColoredTextured(topCapDraw[i], color, u, 0);
                this.sideTrianglesVertices[2 * i + 1] = new CustomVertex.PositionColoredTextured(bottomCapDraw[i], color, u, 1);

                //triangulos de la tapa superior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i] = new CustomVertex.PositionColoredTextured(topCapDraw[i - 1], color, FastMath.Cos(angle - angleStep), FastMath.Sin(angle - angleStep));
                this.capsTrianglesVertices[3 * i + 1] = new CustomVertex.PositionColoredTextured(topCapDraw[i], color, FastMath.Cos(angle), FastMath.Sin(angle));
                this.capsTrianglesVertices[3 * i + 2] = new CustomVertex.PositionColoredTextured(topCapCenter, color, 0.5f, 0.5f);

                //triangulos de la tapa inferior (list)
                if (i > 0) this.capsTrianglesVertices[3 * i + 3 * capsResolution] = new CustomVertex.PositionColoredTextured(bottomCapDraw[i - 1], color, FastMath.Cos(angle - angleStep), FastMath.Sin(angle - angleStep));
                this.capsTrianglesVertices[3 * i + 1 + 3 * capsResolution] = new CustomVertex.PositionColoredTextured(bottomCapDraw[i], color, FastMath.Cos(angle), FastMath.Sin(angle)); ;
                this.capsTrianglesVertices[3 * i + 2 + 3 * capsResolution] = new CustomVertex.PositionColoredTextured(bottomCapCenter, color, 0.5f, 0.5f);

                //rotamos el vector de dibujado
                n.TransformNormal(rotationMatrix);
                angle += angleStep;
            }

            //cerramos la cara lateral
            this.sideTrianglesVertices[2 * capsResolution] = new CustomVertex.PositionColoredTextured(topCapDraw[0], color, 1, 0);
            this.sideTrianglesVertices[2 * capsResolution + 1] = new CustomVertex.PositionColoredTextured(bottomCapDraw[0], color, 1, 1);

            //cerramos la tapa superior
            this.capsTrianglesVertices[0] = new CustomVertex.PositionColoredTextured(topCapDraw[capsResolution - 1], color, FastMath.Cos(-angleStep), FastMath.Sin(-angleStep));

            //Cerramos la tapa inferior
            this.capsTrianglesVertices[3 * capsResolution] = new CustomVertex.PositionColoredTextured(bottomCapDraw[capsResolution - 1], color, FastMath.Cos(-angleStep), FastMath.Sin(-angleStep));
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            if (this.AlphaBlendEnable)
            {
                d3dDevice.RenderState.AlphaBlendEnable = true;
                d3dDevice.RenderState.AlphaTestEnable = true;
            }

            if (texture != null)
                texturesManager.shaderSet(effect, "texDiffuseMap", texture);
            else
                texturesManager.clear(0);
            texturesManager.clear(1);

            GuiController.Instance.Shaders.setShaderMatrix(this.effect, this.Transform);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionColoredTextured;
            effect.Technique = this.technique;

            int capsResolution = END_CAPS_RESOLUTION;

            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2 * capsResolution, this.sideTrianglesVertices);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 2 * capsResolution, this.capsTrianglesVertices);
            effect.EndPass();
            effect.End();

            d3dDevice.RenderState.AlphaTestEnable = false;
            d3dDevice.RenderState.AlphaBlendEnable = false;


            //int capsResolution = END_CAPS_RESOLUTION;

            ////FillMode old = d3dDevice.RenderState.FillMode;
            ////d3dDevice.RenderState.FillMode = FillMode.WireFrame;
            //
            //
            ////d3dDevice.RenderState.FillMode = old;

        }

        public void dispose()
        {
            if (this.texture != null) this.texture.dispose();
            this.sideTrianglesVertices = null;
            this.capsTrianglesVertices = null;
            this.boundingCylinder.dispose();
        }

        public Color Color {
            get { return Color.FromArgb(this.color); }
            set { this.color = value.ToArgb(); }
        }

        public bool AlphaBlendEnable { get; set; }

        #region Transformation

        public bool AutoTransformEnable {
            get { return this.boundingCylinder.AutoTransformEnable; }
            set { this.boundingCylinder.AutoTransformEnable = value; }
        }

        public Matrix Transform
        {
            get { return this.boundingCylinder.Transform; }
            set { this.boundingCylinder.Transform = value; }
        }

        public Vector3 Position
        {
            get { return this.boundingCylinder.Position; }
            set { this.boundingCylinder.Position = value; }
        }

        public Vector3 Rotation
        {
            get { return this.boundingCylinder.Rotation; }
            set { this.boundingCylinder.Rotation = value; }
        }

        public Vector3 Scale
        {
            get { return this.boundingCylinder.Scale; }
            set { this.boundingCylinder.Scale = value; }
        }

        public void move(Vector3 v)
        {
            this.boundingCylinder.move(v);
        }

        public void move(float x, float y, float z)
        {
            this.boundingCylinder.move(x, y, z);
        }

        public void moveOrientedY(float movement)
        {
            this.boundingCylinder.moveOrientedY(movement);
        }

        public void getPosition(Vector3 pos)
        {
            this.boundingCylinder.getPosition(pos);
        }

        public void rotateX(float angle)
        {
            this.boundingCylinder.rotateX(angle);
        }

        public void rotateY(float angle)
        {
            this.boundingCylinder.rotateY(angle);
        }

        public void rotateZ(float angle)
        {
            this.boundingCylinder.rotateZ(angle);
        }

        #endregion

        /// <summary>
        /// Shader del mesh
        /// </summary>
        public Effect Effect
        {
            get { return this.effect; }
            set { this.effect = value; }
        }

        /// <summary>
        /// Technique que se va a utilizar en el effect.
        /// Cada vez que se llama a render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return this.technique; }
            set { this.technique = value; }
        }

        /// <summary>
        /// Setea la textura
        /// </summary>
        public void setTexture(TgcTexture _texture)
        {
            if (this.texture != null)
                this.texture.dispose();
            this.texture = _texture;
        }

        /// <summary>
        /// Habilita el dibujado de la textura
        /// </summary>
        public bool UseTexture {
            get { return this.useTexture; }
            set
            {
                if (value)
                    this.useTextureShader();
                else
                    this.useColorShader();
            }
        }

        public BoundingCylinder BoundingCylinder { get { return this.boundingCylinder; } }

        public void updateValues()
        {
            this.boundingCylinder.updateValues();
            this.updateDraw();
        }

        public float Height
        {
            get { return this.boundingCylinder.Height; }
            set { this.boundingCylinder.Height = value; }
        }

        public float Radius
        {
            get { return this.boundingCylinder.Radius; }
            set { this.boundingCylinder.Radius = value; }
        }
    }
}
