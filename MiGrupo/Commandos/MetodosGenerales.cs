﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class MetodosGenerales //TODO darle un mejor nombre
    {
        public static bool estaCerca(float a, float b)
        {
            if (a < b + 1 && a > b - 1) return true;
            return false;
        }

        public static bool estaCerca(Vector3 a, Vector3 b)
        {
            if (a.X < b.X + 1 && a.X > b.X - 1 &&
                a.Y < b.Y + 1 && a.Y > b.Y - 1 &&
                a.Z < b.Z + 1 && a.Z > b.Z - 1) return true;
            return false;
        }
    }
}
