using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using System;

namespace AlumnoEjemplos.MiGrupo.Pruebas
{
    class Terrain : TgcSimpleTerrain
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
            halfWidth = (float)HeightmapData.GetLength(0) / 2;
            halfLength = (float)HeightmapData.GetLength(1) / 2;


        }


        public int getHeight(float x, float z)
        {
            int height;
            int i, j;

            i = (int)(x / scaleXZ + halfWidth);
            j = (int)(z / scaleXZ + halfLength);

            if (i >= HeightmapData.GetLength(0) || j >= HeightmapData.GetLength(1) || j < 0 || i < 0) return 0;

            height = (int)(HeightmapData[i, j] * scaleY);
           
            return height;
        }


    }
}
