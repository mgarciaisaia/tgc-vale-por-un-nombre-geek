using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cone;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Character
    {
        protected VisionCone vision;
        
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


       /* /// <summary>
        /// Maxima altura de vision.
        /// </summary>
        public float VisionMaxHeight { get { return vision.MaxHeight; } set { vision.MaxHeight = value; } }
*/

        /// <summary>
        /// Renderizado de cono
        /// </summary>
        public bool ConeEnabled { get { return vision.Enabled; } set { vision.Enabled = value; } }


        /// <summary>
        /// Renderizado de direccion del cono
        /// </summary>
        public bool ShowConeDirection { get { return vision.ShowDirection; } set { vision.ShowDirection = value; } }


      

        public Enemy(Vector3 _position)
            : base(_position)
        {

            this.createVisionCone(DEFAULT_VISION_RADIUS, FastMath.ToRad(DEFAULT_VISION_ANGLE));


        }


        

        protected override void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new EnemyRepresentation(position);
        }


        private void createVisionCone(float radius, float angle )
        {
           // this.vision = new WideVisionCone(this.representation, radius, angle, 50);
            this.vision = new WideVisionCone(this.representation, radius, angle);
        }

       

        public bool canSee(Character target)
        {
            return vision.isInsideVisionRange(target, this.level.Terrain, this.level.Objects);
        }


        /*public bool canSeeACommando(out Commando commando)
        {
            Commando closest = null;
           foreach(Commando c in this.level.Commandos){
               if (this.canSee(c))
               {
                   commando = c;
                   
                   return true;  
               }
           }
           commando = closest;
           return false;
        }
        */
        public bool canSeeACommando(out Commando commando)
        {
            Commando closest = null;
            foreach (Commando c in this.level.Commandos)
            {
                if (this.canSee(c))
                {
                    if (closest == null || Vector3.LengthSq(this.Position - c.Position) < Vector3.LengthSq(this.Position - closest.Position))
                        closest = c;


                }
            }
            commando = closest;
            return commando != null;
        }


        protected override Vector3 calculateDirectionVector(ITargeteable target)
        {
            Vector3 movementVector = target.Position - this.representation.Position;
           
            
            movementVector.Y = 0;
            movementVector.Normalize();

            
            return movementVector;
        }


        public override void render() 
        {
            base.render();
            if(!this.Dead && this.Selected)
                vision.render();
        }

        public override void dispose()
        {
            base.dispose();
            this.vision.dispose();
        }

        public override bool OwnedByUser
        {
            get { return false; }
        }

        public VisionCone VisionCone { get { return this.vision; } }

        public override void update(float elapsedTime) { this.VisionCone.updatePosition(); }
    }
}