using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle.states
{
    class Waiting : SelectionState
    {
        public Waiting(RectangleSelection _selection)
            : base(_selection)
        {
            //nothing to do
        }

        public override void update()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Definir punto inicial del rectangulo
                Vector2 mousePos = new Vector2(input.Xpos, input.Ypos);
                this.selection.setState(new Selecting(this.selection, mousePos));
            }
        }
    }
}
