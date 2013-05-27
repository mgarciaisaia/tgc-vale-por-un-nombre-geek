using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    abstract class Renderer
    {
        public List<ILevelObject> objects { get; set; }
        public List<Character> characters { get; set; }
        public List<TerrainPatch> patches { get; set; }

        public abstract void render();
        public abstract void dispose();
    }
}
