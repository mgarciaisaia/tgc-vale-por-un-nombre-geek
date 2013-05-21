using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    class TerrainWithWater : Terrain
    {
        private TgcBox water;

        #region Initialize

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
            Vector3 waterPosition = new Vector3(this.Position.X, this.Position.Y + _waterLevel, this.Position.Z);
            this.water = TgcBox.fromSize(waterPosition, new Vector3(this.getWidth() * this.ScaleXZ, 50 * this.ScaleY, this.getLength() * this.ScaleXZ), Color.LightSlateGray);
            this.water.AlphaBlendEnable = true;
            this.water.Color = Color.FromArgb(150, Color.Aqua);
        }

        #endregion

        #region Getters

        public float waterLevel
        {
            get
            {
                return this.water.Position.Y;
            }
        }

        #endregion

        public new void render()
        {
            water.render();
            base.render();
        }

        public override bool positionAvailableForCharacter(Vector3 coords)
        {
            if (coords.Y >= this.waterLevel) return true; else return false;
        }
    }
}
