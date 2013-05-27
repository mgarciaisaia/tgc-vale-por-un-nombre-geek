using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    abstract class IRenderer
    {
        public List<ILevelObject> objects { get; set; }
        public List<Character> characters { get; set; }
        public List<TerrainPatch> patches { get; set; }

        //void beginRender();
        //void render(ILevelObject o);
        //void render(Commando c);
        //void render(Enemy e);
        //void render(TerrainPatch t);
        public abstract void render();
        public abstract void dispose();
    }
}
