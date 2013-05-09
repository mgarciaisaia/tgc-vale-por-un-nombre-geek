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

        TgcArrow normal;

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
            GuiController.Instance.Modifiers.addVertex3f("posicion", new Vector3(-200, -50, -200), new Vector3(200, 50, 200), this.lastCylinderPos);

            this.myCylinder = new Cylinder(this.lastCylinderPos, 20, 10, Color.Yellow);

            this.cylinder = new Cylinder(new Vector3(-30, 0, 0), 40, 15, Color.Yellow);
            this.sphere = new TgcBoundingSphere(new Vector3(80, 0, 0), 45);
            this.boundingBox = new TgcBoundingBox(new Vector3(0, 0, -120), new Vector3(80, 40, -80));

            this.normal = new TgcArrow();
            this.normal.Thickness = 2f;
            this.normal.HeadSize = new Vector2(4f, 4f);
            this.normal.Enabled = true;
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            if (this.thereIsCollision()) this.myCylinder.Color = Color.DarkOliveGreen;
            else this.myCylinder.Color = Color.Yellow;

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
            this.normal.dispose();
        }

        private bool thereIsCollision()
        {
            Vector3 n;
            if (myCylinder.thereIsCollisionCyCy(this.cylinder, out n))
            {
                this.normal.PStart = this.myCylinder.Position;
                this.normal.PEnd = n * 50 + this.myCylinder.Position;
                this.normal.updateValues();
                this.normal.render();
                return true;
            }
            if (myCylinder.thereIsCollisionCyBB(this.boundingBox, out n))
            {
                this.normal.PStart = this.myCylinder.Position;
                this.normal.PEnd = n * 50 + this.myCylinder.Position;
                this.normal.updateValues();
                this.normal.render();
                return true;
            }
            return false;
        }

    }
}
