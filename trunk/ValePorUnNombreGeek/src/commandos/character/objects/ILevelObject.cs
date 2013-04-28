using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.objetos
{
    interface ILevelObject
    {
        Vector3 Position { get; set; }
        TgcBoundingBox BoundingBox { get; set; }
        void render();
    }
}
