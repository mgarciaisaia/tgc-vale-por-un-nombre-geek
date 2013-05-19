using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects
{
    interface ILevelObject
    {
        Vector3 Position { get; }
        
        Vector3 Center { get; }

        float Radius { get; }

        Effect Effect { get; set;}

        string Technique { get; set; }
      
        void render();

        void dispose();

        bool collidesWith(Character ch, out Vector3 n);
        bool collidesWith(TgcRay ray);

        bool isOver(Vector3 _position);
    }
}
