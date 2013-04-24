using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Input;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle.states
{
    class Selecting : SelectionState
    {
        static readonly int RECT_COLOR = Color.FromArgb(70, Color.LightBlue).ToArgb();

        private Vector2 initMousePos;
        CustomVertex.TransformedColored[] vertices;

        public Selecting(RectangleSelection _selection, Vector2 _initMousePos)
            : base(_selection)
        {
            this.initMousePos = _initMousePos;
            vertices = new CustomVertex.TransformedColored[8];
        }

        public override void update()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            Vector2 lastMousePos = new Vector2(input.Xpos, input.Ypos);

            //Definir recuadro
            Vector2 min = Vector2.Minimize(this.initMousePos, lastMousePos);
            Vector2 max = Vector2.Maximize(this.initMousePos, lastMousePos);
            this.updateRectangle(min, max);
            this.renderRectangle();

            //Solto el mouse
            if(input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                Rectangle rectangle = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

                //Seleccionar solo si el recuadro tiene un tamaño minimo
                if (rectangle.Width > 1 && rectangle.Height > 1)
                {
                    if (!input.keyDown(Key.LeftShift)) this.selection.deselectAllCharacters();
                    this.selection.selectCharactersInRectangle(rectangle);
                }
                else
                {
                    //TODO seleccion simple
                }

                this.selection.setState(new Waiting(this.selection));
                return;
            }
        }

        private void updateRectangle(Vector2 min, Vector2 max)
        {
            //Horizontal arriba
            vertices[0] = new CustomVertex.TransformedColored(min.X, min.Y, 0, 1, RECT_COLOR);
            vertices[1] = new CustomVertex.TransformedColored(max.X, min.Y, 0, 1, RECT_COLOR);

            //Horizontal abajo
            vertices[2] = new CustomVertex.TransformedColored(min.X, max.Y, 0, 1, RECT_COLOR);
            vertices[3] = new CustomVertex.TransformedColored(max.X, max.Y, 0, 1, RECT_COLOR);

            //Vertical izquierda
            vertices[4] = new CustomVertex.TransformedColored(min.X, min.Y, 0, 1, RECT_COLOR);
            vertices[5] = new CustomVertex.TransformedColored(min.X, max.Y, 0, 1, RECT_COLOR);

            //Vertical derecha
            vertices[6] = new CustomVertex.TransformedColored(max.X, min.Y, 0, 1, RECT_COLOR);
            vertices[7] = new CustomVertex.TransformedColored(max.X, max.Y, 0, 1, RECT_COLOR);
        }

        private void renderRectangle()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            bool alphaBlendEnabled = d3dDevice.RenderState.AlphaBlendEnable;
            d3dDevice.RenderState.AlphaBlendEnable = true;

            d3dDevice.VertexFormat = CustomVertex.TransformedColored.Format;
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);

            d3dDevice.RenderState.AlphaBlendEnable = alphaBlendEnabled;
        }
    }
}
