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
        Terrain terrain;

        public MovementPicking(Terrain _terrain)
        {
            this.terrain = _terrain;
        }

        public bool thereIsPicking(out Vector3 p)
        {
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                PickingRayHome.getInstance().updateRay();
                p = PickingRayHome.getInstance().getRayIntersection(this.terrain);
                return true;
            }

            p = Vector3.Empty;
            return false;
        }
    }
}
