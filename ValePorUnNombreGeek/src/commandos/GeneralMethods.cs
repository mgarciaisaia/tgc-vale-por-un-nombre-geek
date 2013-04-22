using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

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

      
        /*public static Vector3 intersectionPoint(Vector3 origin, Vector3 direction, Terrain terrain)
        {
            Vector3 aPoint;
            float i = 0;

            while (true)
            {
                aPoint = origin + i * direction;
                if (GeneralMethods.isCloseTo(aPoint.Y, terrain.getHeight(aPoint.X, aPoint.Z)))
                {
                    //encontramos el punto de interseccion
                    return aPoint;
                }
                if (aPoint.Y <= terrain.Position.Y){
                    //ya nos estamos llendo al subsuelo...
                    return aPoint;
                }
                i++;
            }
        }*/

        public static float SignedAcos(float p)
        {
            float angle = (float)Math.Acos(p)*Math.Sign(p);
           // if (p < 0) angle = (float)Math.PI - angle;
            return angle;
        }
    }
}
