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
            : base(rep.Position+rep.getEyeLevel(), length, angle)
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
            Vector3[] points = new Vector3[3];
            points[0] = target.BoundingBox().calculateBoxCenter();
            points[1] = target.BoundingBox().PMin;
            points[2] = target.BoundingBox().PMax;

            for (int i = 0; i < 3; i++)
            {

                Vector3 positionToTarget = points[i] - this.Position; 
                float dot = Vector3.Dot(Vector3.Normalize(positionToTarget), this.Direction);
                float angle = FastMath.Acos(dot);

                if (angle <= this.angle) return true;
            }

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