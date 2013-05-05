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

        private const int END_CAPS_RESOLUTION = 50;
        private CustomVertex.PositionColored[] vertices;
        private CustomVertex.PositionColored[] bordes;

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
                verticesCount = verticesCount * 2; //por las dos tapas
                this.vertices = new CustomVertex.PositionColored[verticesCount];
                this.bordes = new CustomVertex.PositionColored[4]; //bordes laterales
            }

            float step = FastMath.TWO_PI / (float)END_CAPS_RESOLUTION;
            int index = 0;

            for (float a = 0f; a <= FastMath.TWO_PI; a += step)
            {
                //tapa superior
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a) * this.radius, this.halfLength, FastMath.Sin(a) * this.radius) + this.center, this.renderColor);
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a + step) * this.radius, this.halfLength, FastMath.Sin(a + step) * this.radius) + this.center, this.renderColor);

                //tapa inferior
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a) * this.radius, -this.halfLength, FastMath.Sin(a) * this.radius) + this.center, this.renderColor);
                vertices[index++] = new CustomVertex.PositionColored(new Vector3(FastMath.Cos(a + step) * this.radius, -this.halfLength, FastMath.Sin(a + step) * this.radius) + this.center, this.renderColor);
            }

            this.updateBorders();
        }

        private void updateBorders()
        {
            Vector3 cameraPos = GuiController.Instance.CurrentCamera.getPosition();
            Vector3 cameraSeen = cameraPos - this.center;
            Vector3 vup = new Vector3(0, this.halfLength, 0);
            Vector3 transversalALaCamara = Vector3.Cross(cameraSeen, vup);
            transversalALaCamara.Normalize();
            transversalALaCamara *= this.radius;

            Vector3 puntoA = this.center + vup + transversalALaCamara;
            Vector3 puntoB = this.center - vup + transversalALaCamara;

            bordes[0] = new CustomVertex.PositionColored(puntoA, this.renderColor);
            bordes[1] = new CustomVertex.PositionColored(puntoB, this.renderColor);

            puntoA = this.center + vup - transversalALaCamara;
            puntoB = this.center - vup - transversalALaCamara;

            bordes[2] = new CustomVertex.PositionColored(puntoA, this.renderColor);
            bordes[3] = new CustomVertex.PositionColored(puntoB, this.renderColor);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.updateBorders();
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
            d3dDevice.DrawUserPrimitives(PrimitiveType.LineList, bordes.Length / 2, bordes);
        }

        public void dispose()
        {
            this.vertices = null;
        }
    }
}
