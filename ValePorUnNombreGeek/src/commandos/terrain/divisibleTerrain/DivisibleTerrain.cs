
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain
{

    public class DivisibleTerrain : IRenderObject
    {


        float scaleXZ;
        float scaleY;
        float halfWidth; //Se usa mas la mitad que el total
        float halfLength;
        string texturePath;
        string heightmapPath;
        protected VertexBuffer vbTerrain;
        Texture terrainTexture;
        int totalVertices;
        int[,] heightmapData;

        #region Getters y Setters

        public float HalfWidth { get { return halfWidth; } }
        public float HalfLength { get { return halfLength; } }
        public float getWidth() { return halfWidth * 2; }
        public float getLength() { return halfLength * 2; }
        public float ScaleXZ { get { return scaleXZ; } }
        public float ScaleY { get { return scaleY; } }
        public string TexturePath { get { return texturePath; } }
        public string HeightmapPath { get { return heightmapPath; } }

        public Vector3 Position
        {
            get { return center; }
        }

        /// <summary>
        /// Valor de Y para cada par (X,Z) del Heightmap
        /// </summary>
        public int[,] HeightmapData
        {
            get { return heightmapData; }
        }


        /// <summary>
        /// Indica si la malla esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled { get; set; }


        private Vector3 center;
        /// <summary>
        /// Centro del terreno
        /// </summary>
        public Vector3 Center
        {
            get { return center; }
        }


        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable { get; set; }



        /// <summary>
        /// Shader del mesh
        /// </summary>
        public Effect Effect { get; set; }



        /// <summary>
        /// Technique que se va a utilizar en el effect.
        /// Cada vez que se llama a render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique { get; set; }


        #endregion

        #region Constructor

        public DivisibleTerrain(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY)
            : base()
        {
            this.loadHeightmap(pathHeightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);
            this.texturePath = pathTextura;
            this.heightmapPath = pathHeightmap;
        }


        public DivisibleTerrain()
        {
            Enabled = true;
            AlphaBlendEnable = false;

            //Shader
            this.Effect = GuiController.Instance.Shaders.VariosShader;
            this.Technique = TgcShaders.T_POSITION_TEXTURED;
        }

        /// <summary>
        /// Crea varios terrainPatch en base a un Heightmap
        /// </summary>
        /// <param name="heightmapPath">Imagen de Heightmap</param>
        /// <param name="scaleXZ">Escala para los ejes X y Z</param>
        /// <param name="scaleY">Escala para el eje Y</param>
        /// <param name="center">Centro de la malla del terreno</param>
        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.center = center;
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;
            
            //cargar heightmap
            heightmapData = loadHeightMap(d3dDevice, heightmapPath);
            float width = (float)heightmapData.GetLength(0);
            float length = (float)heightmapData.GetLength(1);

            halfWidth = width / 2;
            halfLength = length / 2;

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmapData.GetLength(0) - 1) * (heightmapData.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Cargar vertices
            int dataIdx = 0;
            CustomVertex.PositionTextured[] data = new CustomVertex.PositionTextured[totalVertices];

            center.X = center.X * scaleXZ - (width / 2) * scaleXZ;
            center.Y = center.Y * scaleY;
            center.Z = center.Z * scaleXZ - (length / 2) * scaleXZ;

            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < length - 1; j++)
                {
                    //Vertices
                    Vector3 v1 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j] * scaleY, center.Z + j * scaleXZ);
                    Vector3 v2 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);
                    Vector3 v3 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j] * scaleY, center.Z + j * scaleXZ);
                    Vector3 v4 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);

                    //Coordendas de textura
                    Vector2 t1 = new Vector2(i / width, j / length);
                    Vector2 t2 = new Vector2(i / width, (j + 1) / length);
                    Vector2 t3 = new Vector2((i + 1) / width, j / length);
                    Vector2 t4 = new Vector2((i + 1) / width, (j + 1) / length);

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }


            vbTerrain.SetData(data, 0, LockFlags.None);

        }

        /// <summary>
        /// Carga la textura del terreno
        /// </summary>
        public void loadTexture(string path)
        {
            //Dispose textura anterior, si habia
            if (terrainTexture != null && !terrainTexture.Disposed)
            {
                terrainTexture.Dispose();
            }

            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Rotar e invertir textura
            Bitmap b = (Bitmap)Bitmap.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }




        /// <summary>
        /// Carga los valores del Heightmap en una matriz
        /// </summary>
        protected int[,] loadHeightMap(Device d3dDevice, string path)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(path);
            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;
            int[,] heightmap = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //(j, i) invertido para primero barrer filas y despues columnas
                    Color pixel = bitmap.GetPixel(j, i);
                    float intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }

            }

            bitmap.Dispose();
            return heightmap;
        }

        #endregion

        #region Render y Dispose

        /// <summary>
        /// Renderiza el terreno
        /// </summary>
        public virtual void render()
        {
            if (!Enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //Textura
            Effect.SetValue("texDiffuseMap", terrainTexture);
            texturesManager.clear(1);

            GuiController.Instance.Shaders.setShaderMatrix(this.Effect, Matrix.Identity);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            Effect.Technique = this.Technique;
            d3dDevice.SetStreamSource(0, vbTerrain, 0);

            //Render con shader
            Effect.Begin(0);
            Effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
            Effect.EndPass();
            Effect.End();

        }

        /// <summary>
        /// Libera los recursos del Terreno
        /// </summary>
        public void dispose()
        {
            if (vbTerrain != null)
            {
                vbTerrain.Dispose();
            }

            if (terrainTexture != null)
            {
                terrainTexture.Dispose();
            }
        }
        #endregion


        #region Position Transform

        /// <summary>
        /// Transforma coordenadas del mundo en coordenadas relativas del heightmap que no tienen en cuenta la escala.
        /// </summary>
        public bool xzToHeightmapCoords(float x, float z, out Vector2 coords)
        {
            int i, j;



            i = (int)(x / scaleXZ + halfWidth);
            j = (int)(z / scaleXZ + halfLength);


            coords = new Vector2(i, j);

            if (coords.X >= HeightmapData.GetLength(0) || coords.Y >= HeightmapData.GetLength(1) || coords.Y < 0 || coords.X < 0) return false;

            return true;
        }

        /// <summary>
        /// Transforma coordenadas relativas del heightmap en coordenadas del mundo.
        /// </summary>
        public bool heightmapCoordsToXYZ(Vector2 coords, out  Vector3 XYZ)
        {
            int i = (int)coords.X;
            int j = (int)coords.Y;

            XYZ = Vector3.Empty;
            if (coords.X >= HeightmapData.GetLength(0) || coords.Y >= HeightmapData.GetLength(1) || coords.Y < 0 || coords.X < 0) return false;

            XYZ = new Vector3(
                 (int)((i - halfWidth) * scaleXZ),
                 (int)(HeightmapData[i, j] * scaleY),
                 (int)((j - halfLength) * scaleXZ)
                 );
            return true;
        }

        #endregion

        #region Y-Getters

        /// <summary>
        /// Obtiene la altura de un punto, si el punto pertenece al heightmap.
        /// </summary>
        private bool getY(float x, float z, out float y)
        {
            y = 0;
            Vector2 coords;
            if (!this.xzToHeightmapCoords(x, z, out coords)) return false;


            y = HeightmapData[(int)coords.X, (int)coords.Y] * scaleY;

            return true;
        }

        /// <summary>
        /// Obteniendo la altura de un punto, devuelve la posicion como una terna si el punto pertenece al heightmap.
        /// </summary>
        public bool getPosition(float x, float z, out Vector3 ret)
        {
            //devuelve la posicion y true si es parte del heightmap
            float y;


            if (this.getY(x, z, out y))
            {
                ret = new Vector3(x, y, z);
                return true;
            }

            ret = ret = new Vector3(x, 0, z);

            return false;


        }

        /// <summary>
        /// Obteniendo la altura de un punto, devuelve la posicion como una terna, sea o no parte del heightmap.
        /// </summary>
        public Vector3 getPosition(float x, float z)
        {
            //devuelve la posicion, sin fijarse si pertenece al heightmap
            Vector3 ret;
            this.getPosition(x, z, out ret);
            return ret;
        }

        #endregion

        #region Y-Bounds

        /// <summary>
        /// Devuelve el valor de Y mas bajo del mapa.
        /// </summary>
        public float minY { get { return this.Position.Y; } }

        /// <summary>
        /// Devuelve el valor de Y mas alto del mapa.
        /// </summary>
        public float maxY { get { return 255 * this.scaleY; } }

        /// <summary>
        /// Devuelve true si la posicion especificada es valida para que se posicione un personaje.
        /// Se utiliza para saber si una posicion es valida para mover por picking al personaje.
        /// Nota: en un terreno sin agua este metodo siempre devuelve true.
        /// </summary>
        public virtual bool positionAvailableForCharacter(Vector3 coords)
        {
            return true;
        }

        #endregion
      

    }
}