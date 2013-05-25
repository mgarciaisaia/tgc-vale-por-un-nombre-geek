using System;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    interface IQuadTree
    {
        void add(ILevelObject obstacle);
        void add(Commando commando);
        void add(Enemy enemy);
        void render(TgcFrustum frustum);
        IRenderer Renderer { get; set; }
        void dispose();
    }
}
