using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class SpriteDrawer
    {
        private Device d3dDevice;
        private Microsoft.DirectX.Direct3D.Sprite dxSprite;
        private List<Sprite> sprites;

        public SpriteDrawer()
        {
            this.d3dDevice = GuiController.Instance.D3dDevice;
            this.dxSprite = new Microsoft.DirectX.Direct3D.Sprite(d3dDevice);
            this.sprites = new List<Sprite>();
        }

        public void addSpriteToDraw(Sprite _sprite)
        {
            this.sprites.Add(_sprite);
        }

        public void drawSprites()
        {
            this.BeginDrawSprite();
            foreach (Sprite sprite in this.sprites)
                this.DrawSprite(sprite);
            this.EndDrawSprite();
        }

        #region DxSpriteDrawing

        /// <summary>
        /// Llamar antes de arrancar a dibujar
        /// </summary>
        private void BeginDrawSprite()
        {
            this.dxSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);
        }

        /// <summary>
        /// Dibuja un sprite
        /// </summary>
        private void DrawSprite(Sprite _sprite)
        {
            this.dxSprite.Transform = _sprite.TransformationMatrix;
            this.dxSprite.Draw(_sprite.Texture, _sprite.RegionRectangle, Vector3.Empty, Vector3.Empty, _sprite.Color);
        }

        /// <summary>
        /// Llamar despues de dibujar
        /// </summary>
        private void EndDrawSprite()
        {
            this.dxSprite.End();
        }

        #endregion
    }
}
