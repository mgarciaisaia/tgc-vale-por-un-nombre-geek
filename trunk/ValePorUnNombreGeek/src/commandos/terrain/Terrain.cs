using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    class Terrain : TgcSimpleTerrain
    {
        float scaleXZ;
        float scaleY;
        float halfWidth; //Se usa mas la mitad que el total
        float halfLength;

        #region Getters

        public float getHalfWidth() { return halfWidth; }
        public float getHalfLength() { return halfLength; }
        public float getWidth() { return halfWidth*2; }
        public float getLength() { return halfLength*2; }
        public float getScaleXZ() { return scaleXZ; }
        public float getScaleY() { return scaleY; }

        #endregion

        #region Initialize

        public Terrain(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY)
            :base()
        {
            this.loadHeightmap(pathHeightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);
        }

        public Terrain() : base()
        {
            string pathHeightmap;
            string pathTextura;
            
            string mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\" + "Heightmaps\\";
            pathHeightmap = mediaDir  + "heightmap.jpg";
            pathTextura = mediaDir + "TerrainTexture5.jpg";
            //pathHeightmap = mediaDir + "Heightmap3.jpg";
            //pathTextura = mediaDir + "TerrainTexture3.jpg";

            //Cargar heightmap
            this.loadHeightmap(pathHeightmap, 20f, 2f, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);
        }



        private new void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center)
        {
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;
            base.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            halfWidth = (float)HeightmapData.GetLength(0) / 2;
            halfLength = (float)HeightmapData.GetLength(1) / 2;
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

            XYZ= new Vector3(
                 (int)((i-halfWidth)* scaleXZ ),
                 (int)(HeightmapData[i,j] * scaleY),
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
