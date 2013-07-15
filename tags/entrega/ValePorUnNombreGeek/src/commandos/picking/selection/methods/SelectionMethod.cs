using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection
{
    interface SelectionMethod
    {
        /// <summary>
        /// Indica si puede iniciar la seleccion (en caso positivo, la inicia)
        /// </summary>
        bool canBeginSelection();

        /// <summary>
        /// Actualiza las cajas o elementos de seleccion
        /// </summary>
        void updateSelection();

        /// <summary>
        /// Renderiza las cajas o elementos de seleccion
        /// </summary>
        void renderSelection();

        /// <summary>
        /// Finaliza la seleccion, devolviendo los personajes seleccionados
        /// </summary>
        List<Character> endAndRetSelection();
    }
}
