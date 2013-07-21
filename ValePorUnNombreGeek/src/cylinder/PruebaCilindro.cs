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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cylinder
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebaCilindro : TgcExample
    {
        CommandosCylinder userCylinder;
        //Vector3 lastPos;

        TgcBoundingSphere staticSphere;
        CommandosCylinder staticCylinder;
        TgcBoundingBox staticAABB;

        TgcArrow colisionNormal;


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "VPUNG:Prueba colision cilindros";
        }

        public override string getDescription()
        {
            return "ValePorUnNombreGeek. Prueba de colisión cilindro-esfera, cilindro-cilindro y cilindro-AABB. Usar las flechitas del teclado para moverse.";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            CommandosUI.Instance.Camera = new TgcCameraAdapter(new StandardCamera());

            //this.lastPos = new Vector3(0, 0, 0);
            //GuiController.Instance.Modifiers.addVertex3f("posicion", new Vector3(-200, 0, -200), new Vector3(200, 0, 200), this.lastPos);

            this.userCylinder = new CommandosCylinder(CommandosUI.Instance.Camera.getLookAt(), 40, 20, Color.Yellow);

            this.staticSphere = new TgcBoundingSphere(new Vector3(200, 0, -200), 40);
            this.staticCylinder = new CommandosCylinder(new Vector3(-100, 0, 0), 40, 40, Color.Yellow);
            this.staticAABB = new TgcBoundingBox(new Vector3(0, -40, -200), new Vector3(80, 40, -120));

            //GuiController.Instance.Modifiers.addBoolean("closestPoint", "closestPoint", false);

            this.colisionNormal = new TgcArrow();
            this.colisionNormal.Thickness = 2f;
            this.colisionNormal.HeadSize = new Vector2(4f, 4f);
            this.colisionNormal.Enabled = true;
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = CommandosUI.Instance.d3dDevice;

            Vector3 cameraPos = CommandosUI.Instance.Camera.getLookAt();
            this.userCylinder.Position = new Vector3(cameraPos.X, -40, cameraPos.Z);

            if (this.thereIsCollision()) this.userCylinder.Color = Color.DarkOliveGreen;
            else this.userCylinder.Color = Color.Yellow;

            //Vector3 newCylinderPos = (Vector3)GuiController.Instance.Modifiers.getValue("posicion");
            //if(this.lastPos != newCylinderPos)
            //{
            //    this.lastPos = newCylinderPos;
            //    this.userCylinder.Position = newCylinderPos;
            //}

            this.userCylinder.render();

            this.staticSphere.render();
            this.staticCylinder.render();
            this.staticAABB.render();
        }

        public override void close()
        {
            this.userCylinder.dispose();
            this.staticSphere.dispose();
            this.staticCylinder.dispose();
            this.staticAABB.dispose();
            this.colisionNormal.dispose();
        }

        private bool thereIsCollision()
        {
            //if ((bool)GuiController.Instance.Modifiers.getValue("closestPoint"))
            //{
            //    this.colisionNormal.PEnd = userCylinder.closestCyPointToPoint(new Vector3(0, 0, 0));
            //    this.colisionNormal.PStart = new Vector3(0, 0, 0);
            //    this.colisionNormal.updateValues();
            //    this.colisionNormal.render();
            //    return false;
            //}

            Vector3 n;
            if (userCylinder.thereIsCollisionCySp(this.staticSphere, out n) ||
                userCylinder.thereIsCollisionCyCy(this.staticCylinder, out n) ||
                userCylinder.thereIsCollisionCyBB(this.staticAABB, out n))
            {
                this.colisionNormal.PStart = this.userCylinder.Position;
                this.colisionNormal.PEnd = n * 50 + this.userCylinder.Position;
                this.colisionNormal.updateValues();
                this.colisionNormal.render();
                return true;
            }
            return false;
        }

    }
}
