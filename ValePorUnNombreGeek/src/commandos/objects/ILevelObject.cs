using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    interface ILevelObject
    {
        //Vector3 Position { get; set; }
        Vector3 Position { get; }
        
        //TgcBoundingBox BoundingBox { get; set; }
        TgcBoundingBox BoundingBox { get; }
        
        Vector3 Center { get; }

        float Radius { get; }
        
        void render();

        void dispose();

        
    }
}
