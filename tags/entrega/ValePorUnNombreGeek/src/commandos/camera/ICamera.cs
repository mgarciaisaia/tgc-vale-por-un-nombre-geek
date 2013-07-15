using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    interface ICamera : TgcCamera
    {
        /// <summary>
        /// Distancia del centro a la posicion de la camara
        /// </summary>
        float Distance { get; }

        /// <summary>
        /// Direccion en la que mira la camara
        /// </summary>
        Vector3 Direction { get; }
    }
}
