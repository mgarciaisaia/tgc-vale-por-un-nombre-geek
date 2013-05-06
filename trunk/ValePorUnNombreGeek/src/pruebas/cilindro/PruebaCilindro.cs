using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.cilindro
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebaCilindro : TgcExample
    {
        Cylinder cylinder;
        TgcBoundingSphere sphere;
        Vector3 lastCylinderPos;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Prueba colision cilindros";
        }

        public override string getDescription()
        {
            return "Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            FreeCamera camera = new FreeCamera();
            camera.Enable = true;

            this.lastCylinderPos = new Vector3(0, 0, 0);
            GuiController.Instance.Modifiers.addVertex3f("posicion", new Vector3(-100, -100, -100), new Vector3(100, 100, 100), this.lastCylinderPos);

            this.cylinder = new Cylinder(this.lastCylinderPos, 20, 10);

            this.sphere = new TgcBoundingSphere(new Vector3(30, 0, 0), 10);
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (cylinder.thereIsCollisionCySp(this.sphere)) this.cylinder.setColor(Color.Blue);
            else this.cylinder.setColor(Color.Red);

            Vector3 newCylinderPos = (Vector3)GuiController.Instance.Modifiers.getValue("posicion");
            if(this.lastCylinderPos != newCylinderPos)
            {
                this.lastCylinderPos = newCylinderPos;
                this.cylinder.Position = newCylinderPos;
            }

            this.cylinder.render();
            this.sphere.render();
        }

        public override void close()
        {
            this.cylinder.dispose();
            this.sphere.dispose();
        }

    }
}
