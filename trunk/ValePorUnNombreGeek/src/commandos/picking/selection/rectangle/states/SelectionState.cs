using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle.states
{
    abstract class SelectionState
    {
        protected RectangleSelection selection;

        public SelectionState(RectangleSelection _selection)
        {
            this.selection = _selection;
        }

        public abstract void update();
    }
}
