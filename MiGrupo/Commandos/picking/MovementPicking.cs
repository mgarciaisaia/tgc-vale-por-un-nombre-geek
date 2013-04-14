using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking
{
    class MovementPicking
    {
        TgcPickingRay pickingRay;
        Terrain terrain;

        public MovementPicking(Terrain _terrain)
        {
            this.terrain = _terrain;
            this.pickingRay = new TgcPickingRay();
        }


        public bool thereIsPicking(out Vector3 p)
        {
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                pickingRay.updateRay();

                Vector3 origin = pickingRay.Ray.Origin;
                Vector3 direction = pickingRay.Ray.Direction;

                float i = 0;

                Vector3 aPoint;

                while (true)
                {
                    aPoint = origin + i * direction;
                    if (GeneralMethods.isCloseTo(aPoint.Y, this.terrain.getHeight(aPoint.X, aPoint.Z)))
                    {
                        //encontramos el punto de interseccion
                        p = aPoint;
                        return true;
                    }
                    if (aPoint.Y < -100){
                        //ya cruzamos hace rato el piso y nos vamos al subsuelo...
                        p = Vector3.Empty;
                        return false;
                    }

                    i++;
                }
            }

            p = Vector3.Empty;
            return false;
        }
    }
}
