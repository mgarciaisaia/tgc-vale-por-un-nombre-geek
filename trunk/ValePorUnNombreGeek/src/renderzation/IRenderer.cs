using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    interface IRenderer
    {
        void beginRender();
        void render(ILevelObject o);
        void render(Commando c);
        void render(Enemy e);
        void render(TerrainPatch t);
        void endRender();
    }
}
