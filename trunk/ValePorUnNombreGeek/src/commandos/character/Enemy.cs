using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cone;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using System.Collections;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    abstract class Enemy : Character
    {
        protected WideVisionCone vision;
           
        private const float DEFAULT_VISION_RADIUS = 400;
        private const float DEFAULT_VISION_ANGLE = 30;
 



        /// <summary>
        /// Angulo en radianes.
        /// </summary>
        public float VisionAngle { get { return vision.Angle; } set { vision.Angle = value; } }

        /// <summary>
        /// Maxima distancia de vision.
        /// </summary>
        public float VisionRadius { get { return vision.Length; } set { vision.Length = value; } }


        /// <summary>
        /// Maxima altura de vision.
        /// </summary>
        public float VisionMaxHeight { get { return vision.MaxHeight; } set { vision.MaxHeight = value; } }


        /// <summary>
        /// Renderizado de cono
        /// </summary>
        public bool ConeEnabled { get { return vision.Enabled; } set { vision.Enabled = value; } }


        /// <summary>
        /// Renderizado de direccion del cono
        /// </summary>
        public bool ShowConeDirection { get { return vision.ShowDirection; } set { vision.ShowDirection = value; } }


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
            vision = new WideVisionCone(this.representation, radius, angle, 50);
        }

       

        public bool canSee(Character target)
        {

            return vision.isInsideVisionRange(target, this.terrain);
        }


        protected bool watch()
        {
            //Si ve a un character lo setea como objetivo y devuelve true
            return false;
        }


        public override void render(float elapsedTime) 
        {
            base.render(elapsedTime);
            vision.render();
        }

        public override void dispose()
        {
            base.dispose();
            this.vision.dispose();
        }

        public override bool userCanMove()
        {
            return false;
        }
    }
}