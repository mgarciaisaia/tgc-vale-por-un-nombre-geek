using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision:Cono
    {
        public ConoDeVision(Vector3 vertex, float radius, float angle):base(vertex, radius,  angle)
        {

        }

        public bool colisionaCon(TgcBox target)
        {
            return false;
        }
    }
}
