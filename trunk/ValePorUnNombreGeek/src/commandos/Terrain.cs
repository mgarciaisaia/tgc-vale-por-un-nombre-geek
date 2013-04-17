using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class Terrain : TgcSimpleTerrain
    {
        float scaleXZ;
        float scaleY;
        float halfWidth;//Se usa mas la mitad que el total
        float halfLength;

        public float getHalfWidth() { return halfWidth; }
        public float getHalfLength() { return halfLength; }
        public float getWidth() { return halfWidth*2; }
        public float getLength() { return halfLength*2; }
        public float getScaleXZ() { return scaleXZ; }
        public float getScaleY() { return scaleY; }


        public Terrain(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY):base()
        {
            
            this.loadHeightmap(pathHeightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);
            
            GuiController.Instance.Modifiers.addBoolean("Terrain", "wireframe", false);

        }

        public Terrain() : base()
        {
            string pathHeightmap;
            string pathTextura;
            string mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\"; ;
            pathHeightmap = mediaDir + "Heightmaps\\" + "heightmap.jpg";
            pathTextura = mediaDir + "Heightmaps\\" + "TerrainTexture5.jpg";



            //Cargar heightmap
            this.loadHeightmap(pathHeightmap, 20f, 1f, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);

            GuiController.Instance.Modifiers.addBoolean("TerrainWireframe", "Visible", false);
        }



        public new void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center)
        {
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;
            base.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            halfWidth = (float)HeightmapData.GetLength(0) / 2;
            halfLength = (float)HeightmapData.GetLength(1) / 2;


        }

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
            //float x,z;

            /*         i = (int)(x / scaleXZ + halfWidth);
                         j = (int)(z / scaleXZ + halfLength);
*/
            XYZ = Vector3.Empty;
            if (coords.X >= HeightmapData.GetLength(0) || coords.Y >= HeightmapData.GetLength(1) || coords.Y < 0 || coords.X < 0) return false;

            XYZ= new Vector3(
                 (int)((i-halfWidth)* scaleXZ ),
                 (int)(HeightmapData[i,j] * scaleY),
                 (int)((j - halfLength) * scaleXZ)
                 );
            return true;
        }


        /// <summary>
        /// Obtiene la altura de un punto.
        /// </summary>
        public bool getY(float x, float z, out float y)
        {
            y = 0;
            Vector2 coords;
            if (!this.xzToHeightmapCoords(x, z, out coords)) return false;

            
            y = HeightmapData[(int)coords.X, (int)coords.Y] * scaleY;

            return true;
        }

        public bool getPosition(float x, float z, out Vector3 ret)
        {
            //devuelve la posicion y true si es parte del heightmap
            float y;
            ret = Vector3.Empty;

            if (!this.getY(x, z, out y)) return false;

            ret = new Vector3(x, y, z);
            return true;
        }

        public Vector3 getPosition(float x, float z)
        {
            //devuelve la posicion, sin fijarse si pertenece al heightmap
            Vector3 ret;
            this.getPosition(x, z, out ret);
            return ret;
        }




        public new void render()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("TerrainWireframe"))
            {
                this.renderWireframe();
            }
            else base.render();
        }

        public void renderWireframe()
        {
            Device device = GuiController.Instance.D3dDevice;

            //Cambiamos a modo WireFrame
            device.RenderState.FillMode = FillMode.WireFrame;

            //Llamamos al metodo original del padre
            base.render();

            //Restrablecemos modo solido
            device.RenderState.FillMode = FillMode.Solid;
        }
    }
}
