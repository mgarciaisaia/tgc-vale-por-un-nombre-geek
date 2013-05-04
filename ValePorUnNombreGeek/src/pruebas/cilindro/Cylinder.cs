using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.cilindro
{
    class Cylinder
    {
        private Vector3 center;
        private float radius;
        private float halfLength;
        private int renderColor;

        private const int END_CAPS_RESOLUTION = 10;
        private CustomVertex.PositionColored[] vertices;

        public Cylinder(Vector3 _center, float _halfLength, float _radius)
        {
            this.center = _center;
            this.radius = _radius;
            this.halfLength = _halfLength;
            this.renderColor = Color.Yellow.ToArgb();
            this.updateValues();
        }

        private void updateValues()
        {
            if (vertices == null)
            {
                int verticesCount = (END_CAPS_RESOLUTION * 2 + 2) * 3;
                this.vertices = new CustomVertex.PositionColored[verticesCount];
            }

            int index = 0;

            float step = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;

            // Plano XZ
            for (float a = 0f; a <= FastMath.TWO_PI; a += step)
            {
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a) * this.radius, this.halfLength, FastMath.Sin(a) * this.radius) + this.center, renderColor);
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a + step) * this.radius, -this.halfLength, FastMath.Sin(a + step) * this.radius) + this.center, renderColor);
            }
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionColored;
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
        }

        public void dispose()
        {
            this.vertices = null;
        }
    }
}
