using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states
{
    abstract class SelectionState
    {
        protected MultipleSelection selection;
        protected Terrain terrain;

        public SelectionState(MultipleSelection _selection, Terrain _terrain)
        {
            this.selection = _selection;
            this.terrain = _terrain;
        }

        abstract public void update();
    }
}
