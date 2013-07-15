using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target
{
    interface ITargeteable //TODO ponerle un nombre lindo! no sabia como ponerle!
    {
         Vector3 Position
        { get; }
        //esta interfaz solo existe para poder luego implementar persecucion entre personajes.
    }
}
