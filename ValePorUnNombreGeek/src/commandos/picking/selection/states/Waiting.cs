﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.states
{
    class Waiting : SelectionState
    {
        public Waiting(MultipleSelection _selection, Terrain _terrain)
            : base(_selection, _terrain)
        {
        }

        public override void update()
        {
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                Vector3 initSelectionPoint;
                if (TerrainPickingRaySingleton.Instance.terrainIntersection(this.terrain, out initSelectionPoint))
                    this.selection.setState(new Selecting(this.selection, this.terrain, initSelectionPoint));
            }
        }
    }
}
