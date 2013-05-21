using System;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    interface ITerrain
    {
        bool AlphaBlendEnable { get; set; }
        Microsoft.DirectX.Vector3 Center { get; }
        void dispose();
        Microsoft.DirectX.Direct3D.Effect Effect { get; set; }
        bool Enabled { get; set; }
        float getLength();
        Microsoft.DirectX.Vector3 getPosition(float x, float z);
        bool getPosition(float x, float z, out Microsoft.DirectX.Vector3 ret);
        float getWidth();
        float HalfLength { get; }
        float HalfWidth { get; }
        bool heightmapCoordsToXYZ(Microsoft.DirectX.Vector2 coords, out Microsoft.DirectX.Vector3 XYZ);
        int[,] HeightmapData { get; }
        string HeightmapPath { get; }
        void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Microsoft.DirectX.Vector3 center);
        void loadTexture(string path);
        float maxY { get; }
        float minY { get; }
        Microsoft.DirectX.Vector3 Position { get; }
        bool positionAvailableForCharacter(Microsoft.DirectX.Vector3 coords);
        void render();
        float ScaleXZ { get; }
        float ScaleY { get; }
        string Technique { get; set; }
        Microsoft.DirectX.Direct3D.Texture TerrainTexture { get; }
        string TexturePath { get; }
        bool xzToHeightmapCoords(float x, float z, out Microsoft.DirectX.Vector2 coords);
    }
}
