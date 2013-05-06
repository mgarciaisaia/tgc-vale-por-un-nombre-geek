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
        Cylinder myCylinder;
        Cylinder cylinder;
        TgcBoundingSphere sphere;
        TgcBoundingBox boundingBox;
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

            this.myCylinder = new Cylinder(this.lastCylinderPos, 20, 10);

            this.cylinder = new Cylinder(new Vector3(-30, 0, 0), 40, 15);
            this.sphere = new TgcBoundingSphere(new Vector3(80, 0, 0), 45);
            this.boundingBox = new TgcBoundingBox(new Vector3(0, 0, -120), new Vector3(40, 40, -80));
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (this.thereIsCollision()) this.myCylinder.setColor(Color.Blue);
            else this.myCylinder.setColor(Color.Red);

            Vector3 newCylinderPos = (Vector3)GuiController.Instance.Modifiers.getValue("posicion");
            if(this.lastCylinderPos != newCylinderPos)
            {
                this.lastCylinderPos = newCylinderPos;
                this.myCylinder.Position = newCylinderPos;
            }

            this.myCylinder.render();
            this.cylinder.render();
            this.sphere.render();
            this.boundingBox.render();
        }

        public override void close()
        {
            this.myCylinder.dispose();
            this.cylinder.dispose();
            this.sphere.dispose();
            this.boundingBox.dispose();
        }

        private bool thereIsCollision()
        {
            if (myCylinder.thereIsCollisionCySp(this.sphere)) return true;
            if (myCylinder.thereIsCollisionCyCy(this.cylinder)) return true;
            if (myCylinder.thereIsCollisionCyBB(this.boundingBox)) return true;
            return false;
        }

    }
}
