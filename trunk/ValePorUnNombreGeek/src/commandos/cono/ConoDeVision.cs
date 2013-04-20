using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    class ConoDeVision : Cono
    {
        protected ICharacterRepresentation rep;
        private float sqLength;
        private float cosAngle;
        
        public Vector3 Direction
        {
            get {
                //Centro de la circunferencia del final del cono
                Vector3 centroCircunferencia = new Vector3(0, 0, -length);
                
                //Aplico las transformaciones que sufrio el cono
                Vector3.TransformCoordinate(centroCircunferencia, this.Transform * Matrix.Translation(rep.getEyeLevel())); 
                
                //Obtengo el vector que va desde el vertice del cono al centro de su circunferencia
                Vector3 vectorDireccion = centroCircunferencia - this.Position;

                //Retorno el vector normalizado
                return Vector3.Normalize(vectorDireccion);
            }
           
           
        }

     
        public ConoDeVision(ICharacterRepresentation rep, float length, float angle)
            : base(rep.getEyeLevel(), length, angle)
        {
            this.rep = rep;
            this.AutoTransformEnable = false;

            this.sqLength = FastMath.Pow2(length);
            this.cosAngle = FastMath.Cos(angle);
                     
        }

        public override void render()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            base.render();
           
        }

        
        public override void updateValues(){
            base.updateValues();
            this.sqLength = FastMath.Pow2(length);
            this.cosAngle = FastMath.Cos(angle);
        }


     
        public override void renderWireframe()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            base.renderWireframe();
                     
           
        }

        public override void renderTransparent()
        {
            this.Transform = rep.Transform * Matrix.Translation(rep.getEyeLevel());
            base.renderTransparent();
        }



        public bool isInsideVisionRange(Character target)
        {
            Vector3[] points = getBoundingBoxPoints(target);

            foreach (Vector3 point in points)
            {

                if (isPointInsideCone(point))
                    return true;
            }

            return false;
        }

        private static Vector3[] getBoundingBoxPoints(Character target)
        {
            Vector3[] points = new Vector3[3];
            Vector3 pMin = target.BoundingBox().PMin;
            Vector3 pMax = target.BoundingBox().PMax;

            points[0] = target.BoundingBox().calculateBoxCenter();
            points[1] = new Vector3(points[0].X, points[0].Y + pMin.Y, points[0].Z);
            points[2] = new Vector3(points[0].X, points[0].Y + pMax.Y, points[0].Z);

           /*Extremos del bounding box
            points[3] = new Vector3(pMin.X, pMin.Y, pMin.Z);
            points[4] = new Vector3(pMin.X, pMin.Y, pMax.Z);
            points[5] = new Vector3(pMin.X, pMax.Y, pMin.Z);
            points[6] = new Vector3(pMin.X, pMax.Y, pMax.Z);
            points[7] = new Vector3(pMax.X, pMin.Y, pMin.Z);
            points[8] = new Vector3(pMax.X, pMin.Y, pMax.Z);
            points[9] = new Vector3(pMax.X, pMax.Y, pMin.Z);
            points[10] = new Vector3(pMax.X, pMax.Y, pMax.Z);
            * */

            return points;
        }
       

        private bool isPointInsideCone(Vector3 point)
        {
            //Vector que va desde el vertice del cono hasta el punto
            Vector3 positionToTarget = point - this.Position;

            //Comparo los cuadrados de las distancias porque hacer raiz cuadrada es costoso.
            if (positionToTarget.LengthSq() <= this.sqLength) {
               
                // A . B = |A||B| cos o  ^  |A|=|B| =1  = > A . B = cos o
                float cos = Vector3.Dot(Vector3.Normalize(positionToTarget), this.Direction); 
                
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