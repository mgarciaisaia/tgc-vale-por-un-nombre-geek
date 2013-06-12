﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.methods
{
    class ScreenProjection : SelectionMethod
    {
        protected List<Character> selectableCharacters;

        public ScreenProjection(List<Character> _selectableCharacters)
        {
            this.selectableCharacters = _selectableCharacters;
            this.vertices = new CustomVertex.TransformedColored[4];
        }

        private List<Character> getCharactersInRectangle(Rectangle rectangle)
        {
            List<Character> ret = new List<Character>();

            foreach (Character ch in this.selectableCharacters)
            {
                Rectangle characterRectangle = ch.BoundingCylinder.projectToScreen();
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
            Microsoft.DirectX.Direct3D.Device d3dDevice = CommandosUI.Instance.d3dDevice;

            bool alphaBlendEnabled = d3dDevice.RenderState.AlphaBlendEnable;
            d3dDevice.RenderState.AlphaBlendEnable = true;

            d3dDevice.VertexFormat = CustomVertex.TransformedColored.Format;
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vertices);

            d3dDevice.RenderState.AlphaBlendEnable = alphaBlendEnabled;
        }

        #endregion

        #region Update

        private Vector2 initMousePos;
        public virtual bool canBeginSelection()
        {
            this.initMousePos = CommandosUI.Instance.MousePosition;
            return true;
        }


        protected Vector2 min, max;
        public virtual void updateSelection()
        {
            Vector2 lastMousePos = CommandosUI.Instance.MousePosition;

            min = Vector2.Minimize(this.initMousePos, lastMousePos);
            max = Vector2.Maximize(this.initMousePos, lastMousePos);
            this.updateRectangle(min, max);
        }

        public void renderSelection()
        {
            this.renderRectangle();
        }


        public virtual List<Character> endAndRetSelection()
        {
            Rectangle rectangle = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            return this.getCharactersInRectangle(rectangle);
        }

        #endregion
    }
}
