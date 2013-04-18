using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states
{
    class Waiting : SelectionState
    {
        public Waiting(MultipleSelection _selection, Terrain _terrain)
            : base(_selection, _terrain)
        {
        }

        public override SelectionState update()
        {
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                Vector3 initTerrainPoint;
                if (TerrainPickingRaySingleton.Instance.terrainIntersection(this.terrain, out initTerrainPoint))
                {
                    Vector3 initGroundPoint = TerrainPickingRaySingleton.Instance.getRayGroundIntersection(this.terrain);
                    return new Selecting(this.selection, this.terrain, initTerrainPoint, initGroundPoint);
                }
            }
            return this;
        }
    }
}
