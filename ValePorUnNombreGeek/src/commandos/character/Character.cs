using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable
    {
        protected ICharacterRepresentation representation;
        protected Level level;

        /*********************************************
         * INICIALIZACION ****************************
         *********************************************/

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

       
        public ICharacterRepresentation Representation
        {
            get { return this.representation; }
        }

        public abstract float Speed
        {
            get;
        }

        public Level Nivel
        {
            get { return this.level; }
            set { this.level = value; }
        }

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

        protected bool dead;
        private bool Dead
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




        /*****************************************
         * UPDATE & RENDER
         * ***************************************/

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
        /*****************************************
         * TARGET
         * ***************************************/

        private ITargeteable target;

        internal void goToTarget(float elapsedTime)
        {
            if (!this.hasTarget() || this.Dead) return;

            /*foreach (Character obstaculo in this.level.getCharactersExcept(this))
            {
                TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(this.BoundingBox, obstaculo.BoundingBox);
                if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    return;
                }
            }*/

            Vector3 direccion = this.target.Position - this.representation.Position;
            direccion.Y = 0;
            direccion.Normalize();

            float currentVelocity = this.Speed * elapsedTime;
            direccion.Multiply(currentVelocity);

            this.representation.walk();
            this.representation.move(direccion);
            if (this.level == null) return;
            this.representation.Position = this.level.getPosition(this.representation.Position.X, this.representation.Position.Z);
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

        public abstract bool userCanMove { get; }
    }
}
