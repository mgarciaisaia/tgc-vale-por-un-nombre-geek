using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;

namespace AlumnoEjemplos.ValePorUnNombreGeek.pruebas
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebasRendimiento : TgcExample
    {
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "PruebasRendimiento";
        }

        public override string getDescription()
        {
            return "Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            GuiController.Instance.Modifiers.addBoolean("customPow2", "customPow2", false);
        }


        public override void render(float elapsedTime)
        {
            float x;
            if ((bool)GuiController.Instance.Modifiers.getValue("customPow2"))
                for (int i = 0; i < 1000; i++)
                    x = GeneralMethods.optimizedPow2(12345);
            else
                for (int i = 0; i < 1000; i++)
                    x = FastMath.Pow2(12345);
        }

        public override void close()
        {

        }

    }
}
