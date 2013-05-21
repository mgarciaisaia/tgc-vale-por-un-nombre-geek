using System;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    interface ITerrain : IRenderObject
    {

        void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center, Vector2 FORMAT);
        void loadTexture(string path);
        Texture Texture { get; }
        
        
        Vector3 Center { get; }
        Vector3 Position { get; }

        float HalfLength { get; }
        float HalfWidth { get; }
        float getLength();
        float getWidth();
        float ScaleXZ { get; }
        float ScaleY { get; }

        
        float maxY { get; }
        float minY { get; }
    


        int[,] HeightmapData { get; }
        bool heightmapCoordsToXYZ(Vector2 coords, out Vector3 XYZ);
        bool xzToHeightmapCoords(float x, float z, out Vector2 coords);
        Vector3 getPosition(float x, float z);
        bool getPosition(float x, float z, out Vector3 ret);
        
       
        bool positionAllowed(Vector3 coords);
         

        Effect Effect { get; set; }
        bool Enabled { get; set; }
        string Technique { get; set; }
      
    }
}
