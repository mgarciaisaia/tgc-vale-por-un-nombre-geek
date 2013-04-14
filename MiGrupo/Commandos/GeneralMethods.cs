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
            if (a < b + TOLERANCIA && a > b - TOLERANCIA) return true;
            return false;
        }

        public static bool isCloseTo(Vector3 a, Vector3 b)
        {
            if (a.X < b.X + TOLERANCIA && a.X > b.X - TOLERANCIA &&
                a.Y < b.Y + TOLERANCIA && a.Y > b.Y - TOLERANCIA &&
                a.Z < b.Z + TOLERANCIA && a.Z > b.Z - TOLERANCIA) return true;
            return false;
        }

        public static float max(float a, float b){
            if (a > b) return a; else return b;
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
                if (aPoint.Y <= 0){
                    //ya nos estamos llendo al subsuelo...
                    return aPoint;
                }
                i++;
            }
        }
    }
}
