using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.vision
{
    class WideVisionCone:VisionCone
    {
        protected float maxHeight;

        public float MaxHeight { 
            get { return maxHeight; } 
            set { 
                maxHeight = value;
                mustUpdate = true;
            }
        }
       
        public WideVisionCone(ICharacterRepresentation rep, float length, float horizontalAngle, float maxHeight)
            : base(rep, length, horizontalAngle)
        {

            this.MaxHeight = maxHeight;
                     
        }

        public WideVisionCone(ICharacterRepresentation rep, float length, float angle)
            : base(rep, length, angle)
        {

            this.maxHeight = this.length * FastMath.Tan(angle);

        }


        //Crea una circunferencia aplanada en Y. ("Corta", no convierte en elipse)
        protected override void crearCircunferencia(float radiusA, int cantPuntos)
        {
            float theta;
            float dtheta = 2 * FastMath.PI / cantPuntos;
            int i;
            Vector3 point;
            

            circunferencia = new Vector3[cantPuntos];

            for (i = 0, theta = 0; i < cantPuntos; i++, theta += dtheta)
            {
                point =  new Vector3(
                          radiusA * FastMath.Cos(theta),
                          radiusA * FastMath.Sin(theta),
                          -length
                     );

                //Si el punto se pasa de la altura limite, bajarlo.
                if (FastMath.Abs(point.Y) > this.MaxHeight)
                {
                    point.Y = this.MaxHeight * point.Y/FastMath.Abs(point.Y);
                }

                circunferencia[i] = point;
            }
        }

        protected override bool isPointInsideCone(Vector3 point)
        {
            if (!base.isPointInsideCone(point)) return false;

            if (FastMath.Abs(point.Y - this.Position.Y) > this.maxHeight)
            {
                this.Color1 = Color.Red;
                this.Color2 = Color.Red;
                return false;
            }
            return true;
            

        }

    }
}
