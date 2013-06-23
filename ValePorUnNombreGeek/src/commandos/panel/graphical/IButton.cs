using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    interface IButton
    {
        Vector2 Position { get; set; }
        float Height { get; }
        float Width { get; }
        void click();
        void render();

        void dispose();
    }
}
