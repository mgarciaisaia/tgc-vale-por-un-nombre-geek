using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;
using System.Collections;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cone
{
    class VisionCone : Cone
    {
        protected ICharacterRepresentation rep;
        protected float sqLength;
        protected float cosAngle;
       

     
        public VisionCone(ICharacterRepresentation rep, float length, float angle)
            : base(rep.getEyeLevel(), length, angle)
        {
            this.rep = rep;
            this.AutoTransformEnable = false;
            this.AlphaBlendEnabled = true;
            
            this.sqLength = FastMath.Pow2(length);
            this.cosAngle = FastMath.Cos(angle);
            this.Color1 = System.Drawing.Color.Aquamarine;
            this.Color2 = System.Drawing.Color.Aquamarine;
            
                     
        }

           
        public override void updateValues(){
            base.updateValues();
            this.sqLength = FastMath.Pow2(length);
            this.cosAngle = FastMath.Cos(angle);
        }


     
        private void updatePosition()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            this.Position = rep.Position + rep.getEyeLevel();
        }

        public override void render()
        {
           updatePosition();
           base.render();
        }

        

        /// <summary>
        /// Retorna true si el target esta dentro del cono.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>

        public bool isInsideVisionRange(Character target)
        {
            Vector3 point = getClosestPointToVertex(target);
          
           if (isPointInsideCone(point))
           {
                this.Color1 = System.Drawing.Color.Red;
                this.Color2 = System.Drawing.Color.Red;
                 
                return true;
         
            }
            this.Color1 = System.Drawing.Color.Aquamarine;
            this.Color2 = System.Drawing.Color.Aquamarine;
            
            return false;
        }

      
        /// <summary>
        /// Calcula punto del eje Y del target que esta mas cerca del vertice del cono
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Punto del eje Y del personaje que esta mas cerca del vertice del cono</returns>
        protected Vector3 getClosestPointToVertex(Character target)
        {
           
            Vector3 pMin = target.BoundingBox().PMin;
            Vector3 pMax = target.BoundingBox().PMax;
            Vector3 center = target.BoundingBox().calculateBoxCenter();


            if (pMin.Y < this.Position.Y && pMax.Y > this.Position.Y)
            {
                return new Vector3(center.X, this.Position.Y, center.Z);
            }
            else if (pMin.Y > this.Position.Y)
            {
                return new Vector3(center.X, pMin.Y, center.Z);
            }
            else return new Vector3(center.X, pMax.Y, center.Z);
           
           
            
        }
       

        protected virtual bool isPointInsideCone(Vector3 point)
        {
            //Vector que va desde el vertice del cono hasta el punto
            Vector3 positionToTarget = point - this.Position;

            //Primero me fijo si cae dentro de la esfera que contiene al cono
            //Comparo los cuadrados de las distancias porque hacer raiz cuadrada es costoso.
            if (positionToTarget.LengthSq() <= this.sqLength) {
               
                //Despues comparo angulos para ver si cae dentro del cono


                // A . B = |A||B| cos o  ^  |A|=|B| =1  = > A . B = cos o
                float cos = Vector3.Dot(Vector3.Normalize(positionToTarget), Vector3.Normalize(this.Direccion)); 
                
                //Comparo cosenos para no tener que hacer Acos. Es equivalente a hacer anguloVerticePunto < anguloCono
                if (cos > this.cosAngle) return true; 
            }

            return false;
        }

         public bool isInsideVisionRange(Character target, Terrain terrain)
        {
             
            if (isInsideVisionRange(target))
            {
                //if(no hay nada tapandome la vista)
                return true;
            }
            else return false;
        }





    }

}