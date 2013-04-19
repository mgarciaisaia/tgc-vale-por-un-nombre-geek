using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Character
    {
        private ConoDeVision vision;

     
        private const float DEFAULT_VISION_RADIUS = 400;
        private const float DEFAULT_VISION_ANGLE = 60;

        /// <summary>
        /// Angulo en radianes.
        /// </summary>
        public float VisionAngle { get { return vision.Angle; } set { vision.Angle = value; } }

        /// <summary>
        /// Maxima distancia de vision.
        /// </summary>
        public float VisionRadius { get { return vision.Length; } set { vision.Length = value; } }

        /// <summary>
        /// Renderizado de cono
        /// </summary>
        public bool ConeEnabled { get { return vision.Enabled; } set { vision.Enabled = value; } }

        public Enemy(Vector3 _position, Terrain _terrain)
            : base(_position, _terrain)
        {
            inicializar();
            
        }

  

        public Enemy(Vector3 _position)
            : base(_position)
        {

              inicializar();


        }

        private void inicializar()
        {

            crearConoDeVision(DEFAULT_VISION_RADIUS, FastMath.ToRad(DEFAULT_VISION_ANGLE));
        }

        protected override void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new EnemyRepresentation(position);
        }


        private void crearConoDeVision(float radius, float angle )
        {
            vision = new ConoDeVision(this.representation, radius, angle);
        }

       

        public bool canSee(Character target)
        {
            
            return vision.isInsideVisionRange(target);
        }


        protected bool watch()
        {
            //Si ve a un character lo setea como objetivo y devuelve true
            return false;
        }


        public override void render(float elapsedTime) 
        {
            base.render(elapsedTime);
            vision.renderWireframe();
        }

       

        }



}