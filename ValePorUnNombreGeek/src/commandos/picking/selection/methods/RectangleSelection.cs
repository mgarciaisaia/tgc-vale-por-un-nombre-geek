using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle
{
    class RectangleSelection : SelectionMethod
    {
        private List<Character> selectableCharacters;

        public RectangleSelection(List<Character> _selectableCharacters)
        {
            this.selectableCharacters = _selectableCharacters;
            this.vertices = new CustomVertex.TransformedColored[4];
        }



        private List<Character> getCharactersInRectangle(Rectangle rectangle)
        {
            List<Character> ret = new List<Character>();

            foreach (Character ch in this.selectableCharacters)
            {
                Rectangle characterRectangle = ch.BoundingBox().projectToScreen();
                if (rectangle.IntersectsWith(characterRectangle))
                    ret.Add(ch);
            }

            return ret;
        }



        #region RectangleRendering

        CustomVertex.TransformedColored[] vertices;
        static readonly int RECT_COLOR = Color.FromArgb(70, Color.LightBlue).ToArgb();

        private void updateRectangle(Vector2 min, Vector2 max)
        {
            //Arriba izq
            this.vertices[0] = new CustomVertex.TransformedColored(min.X, min.Y, 0, 1, RECT_COLOR);
            //Arriba der
            this.vertices[1] = new CustomVertex.TransformedColored(max.X, min.Y, 0, 1, RECT_COLOR);
            //Abajo izq
            this.vertices[2] = new CustomVertex.TransformedColored(min.X, max.Y, 0, 1, RECT_COLOR);
            //Abajo der
            this.vertices[3] = new CustomVertex.TransformedColored(max.X, max.Y, 0, 1, RECT_COLOR);
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

        #endregion



        private Vector2 initMousePos;
        public bool canBeginSelection()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            this.initMousePos = new Vector2(input.Xpos, input.Ypos);

            return true;
        }


        private Vector2 min, max;
        public void renderSelection()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            Vector2 lastMousePos = new Vector2(input.Xpos, input.Ypos);

            min = Vector2.Minimize(this.initMousePos, lastMousePos);
            max = Vector2.Maximize(this.initMousePos, lastMousePos);
            this.updateRectangle(min, max);
            this.renderRectangle();
        }


        public List<Character> endAndRetSelection()
        {
            Rectangle rectangle = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            return this.getCharactersInRectangle(rectangle);
        }
    }
}
