using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.Pruebas;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class Terreno
    {
        Terrain terrain;

        public Terreno()
        {
            string pathHeightmap;
            string pathTextura;
            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;
            pathHeightmap = mediaDir + "Heightmaps\\" + "heightmap.jpg";
            pathTextura = mediaDir + "Heightmaps\\" + "TerrainTexture5.jpg";

            //Cargar heightmap
            this.terrain = new Terrain();
            this.terrain.loadHeightmap(pathHeightmap, 20f, 2f, new Vector3(0, 0, 0));
            this.terrain.loadTexture(pathTextura);

            GuiController.Instance.Modifiers.addBoolean("Terrain", "wireframe", false);
        }

        public void render()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("Terrain"))
            {
                this.terrain.renderWireframe();
            }
            else this.terrain.render();
        }

        public void dispose()
        {
            this.terrain.dispose();
        }

        public float getHeight(float x, float z)
        {
            return this.terrain.getHeight(x, z);
        }

        public Vector3 getPosition(float x, float z)
        {
            return new Vector3(x, this.getHeight(x, z), z);
        }
    }
}
