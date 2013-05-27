using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTreeDummie : ICulling
    {
        protected override void filterAlgorithm(TgcFrustum frustum)
        {
            this.filteredObjects.AddRange(this.objects);
            this.filteredCharacters.AddRange(this.characters);
            this.filteredPatches.AddRange(this.patches);
        }
    }
}
