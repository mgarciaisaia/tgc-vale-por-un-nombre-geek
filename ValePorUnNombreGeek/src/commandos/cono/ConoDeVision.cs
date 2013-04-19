﻿using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision : Cono
    {
        protected ICharacterRepresentation rep;
        public Vector3 Direction
        {
            get{ return Vector3.Normalize( Vector3.TransformCoordinate( new Vector3(0, 1, 0), this.Transform));}
           
        }
       

        public ConoDeVision(ICharacterRepresentation rep, float lenght, float angle)
            : base(rep.Position+rep.getEyeLevel(), lenght, angle)
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



        public bool isInsideVisionRange(Character target)
        {
            return false;
        }

         public bool isInsideVisionRange(Character target, Terrain terrain)
        {
            return false;
        }
        /* Para crear elipse en vez de circunferencia. Se ve mejor pero es mas dificil calcular colision.
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
        }*/
       
    }

}