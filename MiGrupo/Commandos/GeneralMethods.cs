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
    }
}
