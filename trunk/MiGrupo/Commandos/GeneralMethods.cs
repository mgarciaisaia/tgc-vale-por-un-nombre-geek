using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class GeneralMethods //TODO darle un mejor nombre
    {
        private const float TOLERANCIA = 1;

        public static bool isCloseTo(float a, float b)
        {
            return Math.Abs(a - b) < TOLERANCIA;
        }

        public static bool isCloseTo(Vector3 a, Vector3 b)
        {

           return Math.Abs(a.X - b.X) < TOLERANCIA && 
                  Math.Abs(a.Y-b.Y)  < TOLERANCIA && 
                  Math.Abs (a.Z - b.Z) < TOLERANCIA; 
        }

      
        public static Vector3 intersectionPoint(Vector3 origin, Vector3 direction, Terrain terrain)
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
        }
    }
}
