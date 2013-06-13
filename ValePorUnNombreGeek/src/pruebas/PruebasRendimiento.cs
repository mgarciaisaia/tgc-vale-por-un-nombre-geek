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

namespace AlumnoEjemplos.ValePorUnNombreGeek.pruebas
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PruebasRendimiento : TgcExample
    {
        TgcBoundingBox box;

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
            GuiController.Instance.Modifiers.addBoolean("tecnica", "tecnica", false);
            GuiController.Instance.UserVars.addVar("colision");

            new StandardCamera();

            box = new TgcBoundingBox(new Vector3(0, 0, -120), new Vector3(80, 40, -80));
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            box.render();

            TgcFrustum frustum = GuiController.Instance.Frustum;

            if ((bool)GuiController.Instance.Modifiers.getValue("tecnica"))
            {
                for (int i = 0; i < 1000; i++)
                {
                    TgcCollisionUtils.FrustumResult result = TgcCollisionUtils.classifyFrustumAABB(frustum, box);
                    GuiController.Instance.UserVars.setValue("colision", result != TgcCollisionUtils.FrustumResult.OUTSIDE);
                }
            }
            else
            {
                for (int i = 0; i < 1000; i++)
                {
                    bool colision = true;
                    foreach (Plane plane in frustum.FrustumPlanes)
                    {
                        Vector3 boxPos = box.Position;
                        if (!pointIsInFrontOfPlane(boxPos, frustum.BottomPlane)) goto nohaycolision;
                        if (!pointIsInFrontOfPlane(boxPos, frustum.TopPlane)) goto nohaycolision;
                        if (!pointIsInFrontOfPlane(boxPos, frustum.LeftPlane)) goto nohaycolision;
                        if (!pointIsInFrontOfPlane(boxPos, frustum.RightPlane)) goto nohaycolision;
                        break;
                        //if (plane.A * boxPos.X + plane.B * boxPos.Y + plane.C * boxPos.Z > plane.D)
                        //{
                        //    colision = false;
                        //    break;
                        //}
                    nohaycolision:
                        colision = false;
                        break;
                    }
                    GuiController.Instance.UserVars.setValue("colision", colision);
                }
            }
        }

        private bool pointIsInFrontOfPlane(Vector3 point, Plane plane)
        {
            return plane.A * point.X + plane.B * point.Y + plane.C * point.Z < plane.D;
        }

        public override void close()
        {

        }

    }
}
