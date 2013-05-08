using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.collision;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    interface ILevelObject
    {
        Vector3 Position { get; }
        
        TgcBoundingBox BoundingBox { get; }
        
        Vector3 Center { get; }

        float Radius { get; }

        Effect Effect { get; set;}

        string Technique { get; set; }
      
        void render();

        void dispose();

        bool collidesWith(Cylinder cyl, out Vector3 n);
    }
}
