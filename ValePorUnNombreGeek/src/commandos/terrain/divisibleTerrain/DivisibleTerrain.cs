
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

    public class DivisibleTerrain : ITerrain
    {


        protected float scaleXZ;
        protected float scaleY;
        protected float halfWidth; 
        protected float halfLength;
    

        protected Texture texture;
        protected int totalVertices;
        protected int[,] heightmapData;
        protected List<TerrainPatch> patches;

        #region Getters y Setters

        public float HalfWidth { get { return halfWidth; } }
        public float HalfLength { get { return halfLength; } }
        public float getWidth() { return halfWidth * 2; }
        public float getLength() { return halfLength * 2; }
        public float ScaleXZ { get { return scaleXZ; } }
        public float ScaleY { get { return scaleY; } }
        public Texture Texture { get { return texture; } }
        public List<TerrainPatch> Patches { get { return patches; } }

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


        protected Vector3 center;
        protected Vector2 FORMAT;
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

        public DivisibleTerrain(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY, Vector2 FORMAT)
            :this()
        {
            this.loadHeightmap(pathHeightmap, scaleXZ, scaleY, new Vector3(0, 0, 0), FORMAT);
            this.loadTexture(pathTextura);
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
        /// <param name="format">Cantidad de filas y columnas en que se divide el terreno</param>

        
        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center, Vector2 FORMAT)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.center = center;
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;

            this.patches = new List<TerrainPatch>();


            //cargar heightmap
            heightmapData = loadHeightMap(d3dDevice, heightmapPath);
            float width = (float)heightmapData.GetLength(0);
            float length = (float)heightmapData.GetLength(1);

            halfWidth = width / 2;
            halfLength = length / 2;

            
            totalVertices = 2 * 3 * (heightmapData.GetLength(0) - 1) * (heightmapData.GetLength(1) - 1);
       
          
           

            center.X = center.X * scaleXZ - (width / 2) * scaleXZ;
            center.Y = center.Y * scaleY;
            center.Z = center.Z * scaleXZ - (length / 2) * scaleXZ;

           
            
            int patchWidth = (int)Math.Floor(width / FORMAT.Y);
            int patchLength = (int)Math.Floor(length / FORMAT.X);
            int totalPatchVertices = 2 * 3 * patchWidth * patchLength;

            for (int h = 0; h < FORMAT.Y; h++)
            {
                for (int v = 0; v < FORMAT.X; v++)
                {
                    int dataIdx = 0;
                    CustomVertex.PositionTextured[] data = new CustomVertex.PositionTextured[totalPatchVertices];
                    float maxY = 0;
                    for (int i = h * patchWidth; i < (h+1)*patchWidth && i < width -1 ; i++)
                    {
                        for (int j = v * patchLength; j < (v+1)*patchLength && j < length-1 ; j++)
                        {
                            //Vertices
                            Vector3 v1 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j] * scaleY, center.Z + j * scaleXZ);
                            Vector3 v2 = new Vector3(center.X + i * scaleXZ, center.Y + heightmapData[i, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);
                            Vector3 v3 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j] * scaleY, center.Z + j * scaleXZ);
                            Vector3 v4 = new Vector3(center.X + (i + 1) * scaleXZ, center.Y + heightmapData[i + 1, j + 1] * scaleY, center.Z + (j + 1) * scaleXZ);

                            if (center.Y + heightmapData[i, j] * scaleY > maxY) maxY = center.Y + heightmapData[i, j] * scaleY;
                            
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
                    Vector3 pMin = new Vector3(center.X + h*patchWidth*scaleXZ, center.Y, center.Z + v*patchLength*ScaleXZ);
                    Vector3 pMax = new Vector3(center.X + (h + 1) * patchWidth * scaleXZ, maxY, center.Z + (v + 1) * patchLength * scaleXZ);
                    
                    this.patches.Add(new TerrainPatch(this, data, new TgcBoundingBox(pMin, pMax)));
                    GuiController.Instance.Modifiers.addBoolean("Parche de terreno " + (patches.Count - 1).ToString(), "Mostrar", true);
                }
            }
        }

        /// <summary>
        /// Carga la textura del terreno
        /// </summary>
        public void loadTexture(string path)
        {
            //Dispose textura anterior, si habia
            if (texture != null && !texture.Disposed)
            {
                texture.Dispose();
            }

            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Rotar e invertir textura
            Bitmap b = (Bitmap)Bitmap.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            texture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
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
            for (int i=0; i<patches.Count; i++)
            {
                
                bool mostrar = (bool)GuiController.Instance.Modifiers.getValue("Parche de terreno " + i.ToString());
                if (mostrar)
                {
                    patches[i].render();
                   
                }
            }
            
        }

        /// <summary>
        /// Libera los recursos del Terreno
        /// </summary>
        public void dispose()
        {

            foreach (TerrainPatch patch in patches) patch.dispose();

            if (texture != null) 
            {
                texture.Dispose();
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
        public virtual bool positionAllowed(Vector3 coords)
        {
            return true;
        }

        #endregion



    }
}