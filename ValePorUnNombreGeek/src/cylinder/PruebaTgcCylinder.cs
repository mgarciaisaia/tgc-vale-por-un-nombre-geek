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
            cylinder.rotateZ(12);
            cylinder.Position = new Vector3(0, 3, 0);
            cylinder.updateValues();

            //cylinder.Transform = Matrix.Scaling(2, 1, 1);
            //cylinder.AutoTransformEnable = false;
            //cylinder.updateValues();

            GuiController.Instance.Modifiers.addBoolean("boundingCylinder", "boundingCylinder", false);
            GuiController.Instance.Modifiers.addColor("color", Color.DarkGoldenrod);

            GuiController.Instance.Modifiers.addVertex2f("size", new Vector2(1, 1), new Vector2(5, 10), new Vector2(2, 5));
            GuiController.Instance.Modifiers.addVertex3f("position", new Vector3(-20, -20, -20), new Vector3(20, 20, 20), new Vector3(0, 0, 0));
            float angle = FastMath.TWO_PI;
            GuiController.Instance.Modifiers.addVertex3f("rotation", new Vector3(-angle, -angle, -angle), new Vector3(angle, angle, angle), new Vector3(0, 0, 0));
        }


        public override void render(float elapsedTime)
        {
            TgcModifiers modifiers = GuiController.Instance.Modifiers;
            Vector2 size = (Vector2)modifiers.getValue("size");
            Vector3 position = (Vector3)modifiers.getValue("position");
            Vector3 rotation = (Vector3)modifiers.getValue("rotation");

            cylinder.Position = position;
            cylinder.Rotation = rotation;
            cylinder.Radius = size.X;
            cylinder.Height = size.Y;

            cylinder.Color = (Color)modifiers.getValue("color");

            cylinder.updateValues();

            if ((bool)modifiers.getValue("boundingCylinder"))
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
