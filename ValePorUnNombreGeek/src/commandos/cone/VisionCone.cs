using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;
using System.Collections;
using System;
using TgcViewer.Utils.TgcSceneLoader;
using System.Collections.Generic;

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
            return isInsideVisionRange(target, null, null);
        }

        public bool isInsideVisionRange(Character target, Terrain terrain)
        {

            return isInsideVisionRange(target, terrain, null);
        }

        public bool isInsideVisionRange(Character target, List<ITransformObject> obstacles)
        {
            return isInsideVisionRange(target, null, obstacles);
        }

        public bool isInsideVisionRange(Character target, Terrain terrain, List<ITransformObject> obstacles)
        {

            Vector3 targetPoint = getClosestPointToVertex(target);
            if (isPointInsideCone(targetPoint))
            {
                if (terrain == null || canSeeInTerrain(terrain, targetPoint))
                {
                    if (obstacles.Count == 0 || canSeeWithObstacles(targetPoint, obstacles))
                    {
                        changeColor(true);
                        return true;
                    }
                }
            }

            changeColor(false);

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
       
        /// <summary>
        /// Calcula si el punto cae dentro del cono.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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

       /// <summary>
       /// Se fija si el terreno tapa la vista a un punto.
       /// </summary>
       /// <param name="terrain"></param>
       /// <param name="targetPoint"></param>
       /// <returns></returns>
         private bool canSeeInTerrain(Terrain terrain, Vector3 targetPoint)
         {
             Vector3 origin = this.Position;
             Vector3 direction = targetPoint - this.Position;
             float t;
             for (t = 0; t < 1; t += 0.05f)
             {
                 Vector3 aPoint = origin + t * direction;
                 Vector3 terrainPoint = terrain.getPosition(aPoint.X, aPoint.Z);

                 if (aPoint.Y < terrainPoint.Y)
                             return false;
                               
             }

             return true;
         }


         
         private bool canSeeWithObstacles(Vector3 targetPoint, List<ITransformObject> obstacles)
         {
             throw new NotImplementedException();
         }

         private void changeColor(bool canSee)
         {
             if (canSee)
             {
                 this.Color1 = System.Drawing.Color.Red;
                 this.Color2 = System.Drawing.Color.Red;
             }
             else
             {
                 this.Color1 = System.Drawing.Color.Aquamarine;
                 this.Color2 = System.Drawing.Color.Aquamarine;
             }
         }




    }

}