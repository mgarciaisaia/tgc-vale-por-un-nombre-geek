using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    abstract class Culling
    {
        protected List<ILevelObject> objects;
        protected List<Character> characters;
        protected List<TerrainPatch> patches;

        public List<ILevelObject> objectsIn { set { this.objects = value; } }
        public List<Character> charactersIn { set { this.characters = value; } }
        public List<TerrainPatch> patchesIn { set { this.patches = value; } }

        public void filter()
        {
            this.filteredObjects.Clear();
            this.filteredCharacters.Clear();
            this.filteredPatches.Clear();
            this.filterAlgorithm();
        }

        protected abstract void filterAlgorithm();

        protected List<ILevelObject> filteredObjects;
        protected List<Character> filteredCharacters;
        protected List<TerrainPatch> filteredPatches;

        public List<ILevelObject> objectsOut { get { return this.filteredObjects; } }
        public List<Character> charactersOut { get { return this.filteredCharacters; } }
        public List<TerrainPatch> patchesOut { get { return this.filteredPatches; } }

        public Culling()
        {
            this.filteredObjects = new List<ILevelObject>();
            this.filteredCharacters = new List<Character>();
            this.filteredPatches = new List<TerrainPatch>();
        }
    }
}
