using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;

namespace AlumnoEjemplos.MiGrupo.Pruebas
{
    class Terrain:TgcSimpleTerrain
    {
        float scaleXZ;
        float scaleY;
        float halfWidth;
        float halfLength;

        public new void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center)
        {
            this.scaleXZ = scaleXZ;
            this.scaleY = scaleY;
            base.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            halfWidth = (float)HeightmapData.GetLength(0)/2;
            halfLength = (float)HeightmapData.GetLength(1)/2;

        }

      
        public int getHeight(float x, float z)
        {
            int height;
          
            int xInt = (int)x;
            int zInt = (int)z;
            xInt = xInt + (int)(scaleXZ * halfWidth); 
            zInt = zInt + (int)(scaleXZ * halfLength);
           
            height = HeightmapData[(int)(xInt / scaleXZ), (int)(zInt / scaleXZ)];
            height = (int)(height * scaleY);
            return height;
        }

        
    

    }
}
