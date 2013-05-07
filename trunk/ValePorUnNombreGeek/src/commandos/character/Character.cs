using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Shaders;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable, ILevelObject
    {
        protected ICharacterRepresentation representation;
        protected Level level;
        protected string technique;
        protected TgcBox marcaDePicking;

        /*******************************
         * INICIALIZACION **************
         *******************************/

        public Character(Vector3 _position)
        {
            this.loadCharacterRepresentation(_position);
            this.technique = representation.Technique;
            this.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\shaders.fx");
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

        protected Color selectionColor = Color.Red;
        public Color SelectionColor
        {
            get { return this.selectionColor; }
            set { this.selectionColor = value; }
        }

        public virtual void render()
        {
            string technique = this.technique;

            if (this.Selected)
            {
                technique = selectionAction(technique);
            }
            representation.Technique = representation.Prefix+ "_" + technique;
            representation.render();
        }

        protected virtual string selectionAction(string technique)
        {
            technique = this.technique + "_SELECTED";
            this.representation.Effect.SetValue("selectionColor", ColorValue.FromColor(this.selectionColor));

            if (this.hasTarget())
            {
              
                marcaDePicking.render();
            }
            return technique;
        }

        #region Target

        /*******************************
         * TARGET **********************
         *******************************/

        private ITargeteable target;

       

        internal void goToTarget(float elapsedTime)
        {
            if (!this.hasTarget() || this.Dead) return;

            Vector3 direction = calculateDirectionVector(this.target);
            Vector3 previousPosition = this.Position;

            this.level.moveCharacter(this, direction, this.Speed * elapsedTime);
             
        }

        protected virtual Vector3 calculateDirectionVector(ITargeteable target)
        {
            Vector3 direction = target.Position - this.representation.Position;

            direction.Y = 0;
            direction.Normalize();

           
          
            return direction;
        }


        /// <summary>
        /// Accion a realizar en caso de choque
        /// </summary>
        public virtual bool manageCollision(ILevelObject obj)
        {

            if (obj.Equals(this.target))
            {
                this.collisionedTarget();
                return true;
            }

            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(this.marcaDePicking.BoundingBox, obj.BoundingBox);

            //Si la cosa con la que choqué está sobre mi objetivo.
            if (result != TgcCollisionUtils.BoxBoxResult.Afuera)
            {
                this.setNoTarget();
                this.representation.standBy();
                return true;
            }

            return false;
          
        }

        public virtual bool manageSteepTarrain()
        {
            this.setNoTarget();
            this.representation.standBy();
            return true;
        }

        protected virtual void collisionedTarget(){

        }
      

        public void move(Vector3 movement, float speed)
        {
            this.representation.walk();
            
            this.representation.move(movement*speed);
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
            //marcamos hacia donde vamos
            marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
            marcaDePicking.Position = this.target.Position;
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

        public Effect Effect
        {
            get { return representation.Effect; }
            set { representation.Effect = value; }
        }

        public string Technique
        {
            get { return this.technique; }
            set { this.technique = value; }
        }


      
    }
}
