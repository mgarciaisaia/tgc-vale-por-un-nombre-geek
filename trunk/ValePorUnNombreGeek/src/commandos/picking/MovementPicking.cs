using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils._2D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
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
                if(TerrainPickingRaySingleton.Instance.terrainIntersection(this.terrain, out p)) return true;

                else return false;
            }

            p = Vector3.Empty;
            return false;
        }
    }
}
