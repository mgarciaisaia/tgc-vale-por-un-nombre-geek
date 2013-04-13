﻿using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Pruebas
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



        public float getHeight(Vector3 p)
        {
            return getHeight(p.X, p.Z);
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