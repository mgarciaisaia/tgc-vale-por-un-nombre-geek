using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class GeneralMethods //TODO darle un mejor nombre
    {

        public static bool isCloseTo(float a, float b, float delta)
        {
            return Math.Abs(a - b) < delta;
        }

        public static bool isCloseTo(Vector3 a, Vector3 b, float delta)
        {

            return Math.Abs(a.X - b.X) < delta &&
                  Math.Abs(a.Y - b.Y) < delta &&
                  Math.Abs(a.Z - b.Z) < delta; 
        }

        public static float checkAngle(float angle)
        {
            float ret = angle;
            float limit = 2 * FastMath.PI;
            while (ret >= limit) ret -= limit;
            while (ret < 0) ret += limit;
            return ret;
        }

        /*public static float SignedAcos(float p)
        {
            float angle = (float)Math.Acos(p)*Math.Sign(p);
            return angle;
        }*/
    }
}
