using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable, ILevelObject
    {
        protected ICharacterRepresentation representation;
        protected Level level;

        /*******************************
         * INICIALIZACION **************
         *******************************/

        public Character(Vector3 _position)
        {
            this.loadCharacterRepresentation(_position);
            this.Selected = false;
            this.Dead = false;
        }

        //Sobreescribible para que los hijos puedan usar otra representacion
        protected virtual void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new SkeletalRepresentation(position);
        }

        public void setLevel(Level _level)
        {
            this.level = _level;
        }



        /*******************************
         * SETTERS & GETTERS ***********
         *******************************/

        public ICharacterRepresentation Representation
        {
            get { return this.representation; }
        }

        /*public Level Level
        {
            get { return this.level; }
            set { this.level = value; }
        }*/

        public Vector3 Position
        {
            get { return this.representation.Position; }
            set { this.representation.Position = value; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.representation.BoundingBox; }
        }

        #region Status

        private bool selected;
        public bool Selected
        {
            get { return this.selected; }
            set { if (!this.Dead) this.selected = value; }
        }

        private bool dead;
        protected bool Dead
        {
            get { return this.dead; }
            set {
                if (value == true)
                {
                    this.Selected = false;
                    this.representation.die();
                }
                this.dead = value;
            }
        }

        public bool Enabled //Solo se renderiza si esta en true. Sirve para las optimizaciones
        {
            get { return this.representation.Enabled; }
            set { this.representation.Enabled = value; }
        }

        #endregion

        public void die()
        {
            this.Dead = true;
        }

      
  
        public virtual void dispose()
        {
            representation.dispose();
        }




        /*******************************
         * UPDATE & RENDER *************
         *******************************/

        public abstract void update(float elapsedTime);
        
        public virtual void render()
        {
            if (this.Selected)
            {
                this.BoundingBox.render();
                if (this.hasTarget())
                {
                    //marcamos hacia donde vamos
                    TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                    marcaDePicking.Position = this.target.Position;
                    marcaDePicking.render();
                }
            }
            representation.render();
        }

        #region Target

        /*******************************
         * TARGET **********************
         *******************************/

        private ITargeteable target;

        internal void goToTarget(float elapsedTime)
        {
            if (!this.hasTarget() || this.Dead) return;

            Vector3 movementVector = this.target.Position - this.representation.Position;
            movementVector.Y = 0;
            movementVector.Normalize();

            float currentVelocity = this.Speed * elapsedTime;
            movementVector.Multiply(currentVelocity);

            if (!this.level.moveCharacter(this, movementVector))
            {
                this.manageCollision(movementVector);
            }

                       
        }


        /// <summary>
        /// Accion a realizar en caso de choque
        /// </summary>
        protected virtual void manageCollision(Vector3 movementVector)
        {
            this.Representation.standBy();
        }

        public void move(Vector3 movement)
        {
            this.representation.walk();
            this.representation.move(movement);
        }

        internal bool isOnTarget()
        {
            if (this.target == null) return true;

            return GeneralMethods.isCloseTo(this.representation.Position, this.target.Position, 1);
        }

        protected bool hasTarget()
        {
            return this.target != null;
        }

        private void setTarget(ITargeteable _target)
        {
            this.target = _target;
        }

        public void setNoTarget()
        {
            this.representation.standBy();
            this.target = null;
        }

        public void setPositionTarget(Vector3 pos)
        {
            this.setTarget(new TargeteablePosition(pos));
        }

        public void setCharacterTarget(Character ch)
        {
            this.setTarget(ch);
        }

        #endregion

        public abstract bool OwnedByUser { get; }

        public abstract float Speed {  get; }

        public float Radius { get { return this.representation.Radius; } }
        public Vector3 Center { get { return this.representation.Center; } }

        public Effect effect
        {
            get { return representation.Effect; }
            set { representation.Effect = value; }
        }

        public string Technique
        {
            get { return representation.Technique; }
            set { representation.Technique = value; }
        }
      
    }
}
