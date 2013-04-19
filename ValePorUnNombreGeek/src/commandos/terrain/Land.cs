using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain
{
    class Land : Terrain
    {
        #region Initilize

        public Land(string pathHeightmap, string pathTextura, float scaleXZ, float scaleY)
            : base(pathHeightmap, pathTextura, scaleXZ,scaleY)
        {
            //nothing to do
        }

        public Land()
            : base()
        {
            //nothing to do
        }

        #endregion

        public override bool positionAvailableForCharacter(Vector3 coords)
        {
            //en un mapa sin agua toda posicion es valida para el personaje
            return true;
        }
    }
}
