using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using System;
using TgcViewer.Utils.TgcSceneLoader;
using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.cone;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.vision
{
    class VisionCone : Cone
    {
        protected ICharacterRepresentation rep;
        protected float[] sqRange;
        protected float cosAngle;
           
        public enum eRange : int
        {            
            SHORT_RANGE = 0,
            LONG_RANGE = 1,
        }
        public eRange current_range;
     
        public VisionCone(ICharacterRepresentation rep, float length, float angle)
            : base(rep.getEyeLevel(), length, angle)
        {
            this.rep = rep;
            this.AutoTransformEnabled = false;
            this.AlphaBlendEnabled = true;
            this.sqRange = new float[2];
            this.sqRange[(int)eRange.SHORT_RANGE] = GeneralMethods.optimizedPow2(length * 1 / 2);
            this.sqRange[(int)eRange.LONG_RANGE] = GeneralMethods.optimizedPow2(length);
            this.cosAngle = FastMath.Cos(angle);
            this.Color1 = System.Drawing.Color.Aquamarine;
            this.Color2 = System.Drawing.Color.Aquamarine;
            
                     
        }

           
        public override void updateValues(){
            base.updateValues();
            this.sqRange[0] = GeneralMethods.optimizedPow2(length * 2 / 3);
            this.sqRange[1] = GeneralMethods.optimizedPow2(length);
            this.cosAngle = FastMath.Cos(angle);
        }


     
        public void updatePosition()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            this.Position = rep.Position + rep.getEyeLevel();
        }

        public override void render()
        {
          
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

        public bool isInsideVisionRange(Character target, ITerrain terrain)
        {

            return isInsideVisionRange(target, terrain, null);
        }

        public bool isInsideVisionRange(Character target, List<ILevelObject> obstacles)
        {
            return isInsideVisionRange(target, null, obstacles);
        }

        public bool isInsideVisionRange(Character target, ITerrain terrain, List<ILevelObject> obstacles)
        {
            if (target.isDead())
            {
                changeColor(false);
                return false;
            }

            Vector3 targetPoint = target.BoundingCylinder.closestCyPointToPoint(this.Position);

            if (target.Representation.isCrouched())
            {
                this.current_range = eRange.SHORT_RANGE;

            } else this.current_range = eRange.LONG_RANGE;

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
            if (positionToTarget.LengthSq() <= this.sqRange[(int)current_range]) {
               
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
         private bool canSeeInTerrain(ITerrain terrain, Vector3 targetPoint)
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


         
         private bool canSeeWithObstacles(Vector3 targetPoint, List<ILevelObject> obstacles)
         {
             Vector3 pt =  targetPoint-this.Position;
             TgcRay ray = new TgcRay(this.Position, pt);
             foreach (ILevelObject o in obstacles)
             {
                 if (objectInsideRadius(pt.Length(), o))
                 {
                     if (o.collidesWith(ray)) return false;
                 }
             }
             return true;
         }

         private bool objectInsideRadius(float radius, ILevelObject o)
         {
           
             //Ver Unidad 6, pagina 13
             Vector3 distance = this.Position - o.Center;

             float radiusSum = radius + o.Radius;

             return distance.LengthSq() <= radiusSum * radiusSum;

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