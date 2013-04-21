using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cone
{
    class VisionCone : Cone
    {
        protected ICharacterRepresentation rep;
        protected float sqLength;
        protected float cosAngle;
        protected bool showDirection;

        public bool ShowDirection { get { return this.showDirection; } set { this.showDirection = value; } }

        public Vector3 Direccion
        {
            get {
                //Centro de la circunferencia del final del cono
                Vector3 centroCircunferencia = new Vector3(0, 0, -length) + rep.getEyeLevel();
                
                //Aplico las transformaciones que sufrio el cono
                Vector3 vectorDireccion = Vector3.TransformCoordinate(centroCircunferencia, this.Transform); 
                
               
                return vectorDireccion-this.Position;
            }
           
           
        }

     
        public VisionCone(ICharacterRepresentation rep, float length, float angle)
            : base(rep.getEyeLevel(), length, angle)
        {
            this.rep = rep;
            this.AutoTransformEnable = false;
            this.AlphaBlendEnabled = true;
            this.showDirection = false;
            this.sqLength = FastMath.Pow2(length);
            this.cosAngle = FastMath.Cos(angle);
                     
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

            renderDirection();

           base.render();
        }

        protected void renderDirection()
        {
            if (!this.showDirection) return;
            TgcArrow arrow = new TgcArrow();
            arrow.PStart = this.Position;
            arrow.PEnd = this.Position + this.Direccion;
            arrow.Thickness = 2f;
            arrow.HeadSize = new Vector2(0.5f, 0.5f);
            arrow.updateValues();
            arrow.render();
        }



        public bool isInsideVisionRange(Character target)
        {
            Vector3[] points = getBoundingBoxPoints(target);

            foreach (Vector3 point in points)
            {

                if (isPointInsideCone(point))
                {
                    this.Color = System.Drawing.Color.Red;
                    return true;
                }
            }
            this.Color = System.Drawing.Color.Aquamarine;
            return false;
        }


        protected static Vector3[] getBoundingBoxPoints(Character target)
        {
            Vector3[] points = new Vector3[3];
            Vector3 pMin = target.BoundingBox().PMin;
            Vector3 pMax = target.BoundingBox().PMax;

            //3 puntos del eje central
            points[0] = target.BoundingBox().calculateBoxCenter();
            points[1] = new Vector3(points[0].X, pMin.Y, points[0].Z);
            points[2] = new Vector3(points[0].X, pMax.Y, points[0].Z);

            /*
            //Base bounding box
            points[3] = new Vector3(pMin.X, pMin.Y, pMin.Z);
            points[4] = new Vector3(pMin.X, pMin.Y, pMax.Z);
            points[5] = new Vector3(pMax.X, pMin.Y, pMin.Z);
            points[6] = new Vector3(pMax.X, pMin.Y, pMax.Z);
            points[7] = new Vector3(pMin.X, pMin.Y, pMin.Z);
            points[8] = new Vector3(pMin.X, pMin.Y, pMax.Z);
            
           //Tapa bounding box
            points[9] = new Vector3(pMin.X, pMax.Y, pMin.Z);
            points[10] = new Vector3(pMin.X, pMax.Y, pMax.Z);
            points[11] = new Vector3(pMax.X, pMax.Y, pMin.Z);
            points[12] = new Vector3(pMax.X, pMax.Y, pMax.Z);

             **/
           
            return points;
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