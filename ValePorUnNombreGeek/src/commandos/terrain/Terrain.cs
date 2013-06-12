using TgcViewer.Utils.Terrain;
using Microsoft.DirectX;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    public class Terrain : DivisibleTerrain
    {
       

        public Terrain(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY)
            :base(pathHeightmap, pathTextura, scaleXZ, scaleY, new Vector2(1,1))
        {
        }

        public Terrain() : base()
        {
            string pathHeightmap;
            string pathTextura;

            string mediaDir = CommandosUI.Instance.MediaDir + "Heightmaps\\";
            pathHeightmap = mediaDir  + "heightmap.jpg";
            pathTextura = mediaDir + "TerrainTexture5.jpg";
          
            
            //Cargar heightmap
            this.loadHeightmap(pathHeightmap, 20f, 2f, new Vector3(0, 0, 0));
            this.loadTexture(pathTextura);
            
        }

        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, Vector3 center)
        {
            loadHeightmap(heightmapPath, scaleXZ, scaleY, center, new Vector2(1, 1));
        }

    }
}
