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
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebaTgcCylinder : TgcExample
    {
        private Cylinder cylinder;
        private string currentTexture;

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

            //cylinder.Transform = Matrix.Scaling(2, 1, 1);
            //cylinder.AutoTransformEnable = false;
            //cylinder.updateValues();

            cylinder.AlphaBlendEnable = true;

            //cylinder.Effect = TgcShaders.loadEffect(GuiController.Instance.ExamplesMediaDir + "Shaders\\Ejemplo1.fx");
            //cylinder.Technique = "RandomColorVS";

            GuiController.Instance.Modifiers.addBoolean("boundingCylinder", "boundingCylinder", false);
            GuiController.Instance.Modifiers.addColor("color", Color.DarkGoldenrod);
            GuiController.Instance.Modifiers.addInt("alpha", 0, 255, 255);
            GuiController.Instance.Modifiers.addTexture("texture", GuiController.Instance.ExamplesMediaDir + "\\Texturas\\madera.jpg");
            GuiController.Instance.Modifiers.addBoolean("useTexture", "useTexture", false);

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
            
            string texturePath = (string)modifiers.getValue("texture");
            if (texturePath != currentTexture)
            {
                currentTexture = texturePath;
                cylinder.setTexture(TgcTexture.createTexture(GuiController.Instance.D3dDevice, currentTexture));
            }

            cylinder.UseTexture = (bool)modifiers.getValue("useTexture");

            cylinder.Position = position;
            cylinder.Rotation = rotation;
            cylinder.Radius = size.X;
            cylinder.Height = size.Y;

            int alpha = (int)modifiers.getValue("alpha");
            Color color = (Color)modifiers.getValue("color");
            cylinder.Color = Color.FromArgb(alpha, color);

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
