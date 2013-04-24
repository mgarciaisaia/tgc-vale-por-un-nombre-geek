using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    interface Selection
    {
        void update();
        List<Character> getSelectedCharacters();
        void deselectAllCharacters();
    }
}
