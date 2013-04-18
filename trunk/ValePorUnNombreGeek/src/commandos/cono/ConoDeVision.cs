using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision : Cono
    {
        float radiusB;
        public float RadiusB { get{return radiusB;} set{radiusB=value;}}
        ICharacterRepresentation rep;

        public ConoDeVision(ICharacterRepresentation rep, float radius, float angle)
            : base(rep.Position+rep.getEyeLevel(), radius, angle)
        {
            this.rep = rep;
            this.AutoTransformEnable = false;
        }

        public override void render()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            base.render();
        }

        public override void renderWireframe()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            base.renderWireframe();
        }


        public bool isInsideVisionRange(TgcBox target)
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