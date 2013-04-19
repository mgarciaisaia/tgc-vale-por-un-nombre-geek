using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    class TerrainWithWater : Terrain
    {
        private float waterLevel;
        private TgcBox water;


        public TerrainWithWater(float _waterLevel)
            : base()
        {
            this.setWaterLevel(_waterLevel);
        }

        public TerrainWithWater(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY, float _waterLevel)
            : base(pathHeightmap, pathTextura, scaleXZ, scaleY)
        {
            this.setWaterLevel(_waterLevel);
        }

        private void setWaterLevel(float _waterLevel)
        {
            this.waterLevel = _waterLevel;
            Vector3 waterPosition = new Vector3(this.Position.X, this.Position.Y + this.waterLevel, this.Position.Z);
            this.water = TgcBox.fromSize(waterPosition, new Vector3(this.getWidth() * this.getScaleXZ(), 50 * this.getScaleY(), this.getLength() * this.getScaleXZ()), Color.LightSlateGray);
        }

        public override bool getY(float x, float z, out float y)
        {
            bool ret = base.getY(x, z, out y);
            if (ret == true && y < this.waterLevel) ret = false;
            return ret;
        }

        public new void render()
        {
            water.render();
            base.render();
        }
    }
}
