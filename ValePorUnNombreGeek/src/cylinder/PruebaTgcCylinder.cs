using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebaTgcCylinder : TgcExample
    {
        private Cylinder cylinder;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "TgcCylinder";
        }

        public override string getDescription()
        {
            return "Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.";
        }

        public override void init()
        {
            cylinder = new Cylinder(new Vector3(0, 0, 0), 2, 4);
            //cylinder.AutoTransformEnable = false;
            cylinder.rotateZ(12);
            cylinder.Position = new Vector3(0, 3, 0);
            cylinder.updateValues();
            //cylinder = new BoundingCylinder(new Vector3(0, 0, 0), 5, new Vector3(10, 10, 0));
            //cylinder.AlphaBlendEnable = true;

            GuiController.Instance.Modifiers.addBoolean("boundingCylinder", "boundingCylinder", false);
            GuiController.Instance.Modifiers.addColor("color", Color.DarkKhaki);
        }


        public override void render(float elapsedTime)
        {
            //cylinder.Color = (Color)GuiController.Instance.Modifiers.getValue("color");

            if ((bool)GuiController.Instance.Modifiers.getValue("boundingCylinder"))
                cylinder.BoundingCylinder.render();
            else
                cylinder.render();
        }

        public override void close()
        {
            cylinder.dispose();
        }

    }
}
