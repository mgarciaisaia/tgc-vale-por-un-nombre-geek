﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    abstract class LevelObject
    {
        public abstract Vector3 Position { get; set; }
        
        //TgcBoundingBox BoundingBox { get; set; }
        public abstract TgcBoundingBox BoundingBox { get; }

        public abstract Vector3 Center { get; }

        public abstract float Radius { get; }

        public abstract Effect Effect { get; set; }

        public abstract string Technique { get; set; }

        public abstract void render();

        public abstract void dispose();
    }
}
