using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision : Cono
    {
        float radiusB;
        public float RadiusB { get{return radiusB;} set{radiusB=value;}}
        public ConoDeVision(Vector3 vertex, float radius, float angle)
            : base(vertex, radius, angle)
        {
            GuiController.Instance.Modifiers.addBoolean("Cono", "Visible", false);

        }

        public override void render()
        {
            this.Enabled = (bool)GuiController.Instance.Modifiers.getValue("Cono");
            base.render();
        }

        public override void renderWireframe()
        {
            this.Enabled = (bool)GuiController.Instance.Modifiers.getValue("Cono");
            base.renderWireframe();
        }


        public bool colisionaCon(TgcBox target)
        {
            return false;
        }

        protected override void crearCircunferencia(float radiusA, int cantPuntos)
        {
            float theta;
            float dtheta = 2 * FastMath.PI / cantPuntos;
            int i;
            
            if(radiusB==0) radiusB= (float)0.5*radiusA;

            circunferencia = new Vector3[cantPuntos];

            for (i = 0, theta = 0; i < cantPuntos; i++, theta += dtheta)
            {

                circunferencia[i] = new Vector3(
                          radiusA * FastMath.Cos(theta),
                          (float)(radiusB * FastMath.Sin(theta)),
                          -radius
                     );
            }
        }
       
    }

}