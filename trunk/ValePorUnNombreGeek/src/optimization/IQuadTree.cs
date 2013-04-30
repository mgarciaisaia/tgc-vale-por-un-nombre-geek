using System;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.levelObject;
using System.Collections.Generic;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    interface IQuadTree
    {
        void add(ILevelObject obstacle);
        void render(TgcFrustum frustum, List<Commando> commandos, List<Enemy> enemies);
    }
}
