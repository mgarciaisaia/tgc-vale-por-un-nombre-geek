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

                p = GeneralMethods.intersectionPoint(pickingRay.Ray.Origin, pickingRay.Ray.Direction, this.terrain);
                return true;
            }

            p = Vector3.Empty;
            return false;
        }
    }
}
