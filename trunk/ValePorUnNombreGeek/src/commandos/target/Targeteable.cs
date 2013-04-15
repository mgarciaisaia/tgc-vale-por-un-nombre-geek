using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target
{
    interface Targeteable //TODO ponerle un nombre lindo! no sabia como ponerle!
    {
        Vector3 getPosition();

        //esta interfaz solo existe para poder luego implementar persecucion entre personajes.
    }
}
