using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision:Cono
    {
        public ConoDeVision(Vector3 vertex, float radius, float angle):base(vertex, radius,  angle)
        {
            GuiController.Instance.Modifiers.addBoolean("Cono", "Visible", false);
            
        }

        public override void render()
        {
            this.Enabled = (bool)GuiController.Instance.Modifiers.getValue("Cono");
            base.render();
        }
        public bool colisionaCon(TgcBox target)
        {
            return false;
        }
    }
}
