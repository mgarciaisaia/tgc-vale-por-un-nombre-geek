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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.collision;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable, ILevelObject
    {
        protected ICharacterRepresentation representation;
        protected Level level;
        protected string technique;
        protected TgcBox marcaDePicking;
        private Cylinder boundingCylinder;
        public static bool RenderCylinder = false;
        protected Life life;


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
            this.life = new Life(this, 100, new Vector2(20, 60), Color.Red, new Vector2(60, 10));
            //this.life = new Life(this, 100, new Vector2(60, 20), Color.Red, new Vector2(60, 10));
            Vector3 boundingSize = this.BoundingBox.calculateSize() * 0.5f;
            this.boundingCylinder = new Cylinder(this.Center, boundingSize.Y, boundingSize.X);
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


        public Life Life
        {
            get { return this.life; }
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

        public void die()
        {
            this.Dead = true;
        }

        public bool Enabled //Solo se renderiza si esta en true. Sirve para las optimizaciones
        {
            get { return this.representation.Enabled; }
            set { this.representation.Enabled = value; }
        }

        #endregion


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

            if(Character.RenderCylinder) this.boundingCylinder.render();
        }

        public virtual void dispose()
        {
            representation.dispose();
            life.dispose();
            this.boundingCylinder.dispose();
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
        public ITargeteable Target
        {
            get { return this.target; }
            set { this.target = value; }
        }
       

        internal void goToTarget(float elapsedTime)
        {
            if (!this.hasTarget() || this.Dead) return;
            if (GeneralMethods.isCloseTo(this.Position, target.Position, 1)) //pablo
            {
                this.setNoTarget();
                return;
            }

            Vector3 direction = calculateDirectionVector(this.target);
            Vector3 previousPosition = this.Position;

            this.move(direction, this.Speed * elapsedTime);
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

        public virtual bool manageSteepTerrain()
        {
            this.setNoTarget();
            this.representation.standBy();
            return true;
        }

        protected virtual void collisionedTarget(){

        }

        internal bool isOnTarget()
        {
            if (this.target == null) return true;

            return GeneralMethods.isCloseTo(this.representation.Position, this.target.Position, 1);
        }

        public bool hasTarget()
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


        #region Collision

        public bool collidesWith(Character ch, out Vector3 n)
        {
            return ch.collidesWith(this.boundingCylinder, out n);
        }

        public bool collidesWith(Cylinder cyl, out Vector3 n)
        {
            return this.boundingCylinder.thereIsCollisionCyCy(cyl, out n);
        }

        public bool collidesWith(TgcBoundingBox aabb, out Vector3 n)
        {
            return this.boundingCylinder.thereIsCollisionCyBB(aabb, out n);
        }

        #endregion








        public void doMovement(Vector3 movement, float speed)
        {
            this.representation.walk();
            this.representation.move(movement * speed);
            this.Position = this.level.Terrain.getPosition(this.Position.X, this.Position.Z);
            this.boundingCylinder.Center = this.Center;
        }


        public void move(Vector3 direction, float speed)
        {
            Vector3 previousPosition = this.Position;
            Vector3 realMovement = direction;

            //Cuando se pueda hacer que no se traben, se quita character.OwnedByUser
            if (this.OwnedByUser && this.terrenoMuyEmpinado(previousPosition, direction * speed))
            {
                /*//Busco movimientos alternativos
                foreach (Vector3 alt in getAlternativeMovements(direction))
                {
                    if (!terrenoMuyEmpinado(previousPosition, alt*speed))
                    {
                        realMovement = alt;
                        break;
                    }
                }
               */
                if (realMovement == direction)
                {
                    this.manageSteepTerrain();
                    return;
                }
            }

            //Muevo el personaje
            this.doMovement(realMovement, speed);

            ILevelObject obj;
            Vector3 n;
            Vector3 centrifugal = Vector3.Empty;

            int intentos;
            int maxIntentos = 50;
            for (intentos = 0; this.level.thereIsCollision(this, out obj, out n); intentos++)
            {
                //Cancelo el movimiento
                this.Position = previousPosition;
                if (intentos == maxIntentos) break;

                //Si el pj ya arregló el problema, parar.
                if (this.manageCollision(obj)) break;

                //Calculo un vec que se aleja del centro del objeto para que el pj gire alrededor.
                centrifugal = (this.Center - obj.Center);

                centrifugal.Y = 0;
                centrifugal.Normalize();

                realMovement = centrifugal + realMovement;  //Voy haciendo que la direccion tienda mas hacia la centrifuga.
                realMovement.Normalize();

                this.doMovement(realMovement, speed);

                if (Character.RenderCylinder)
                {
                    GeneralMethods.renderVector(this.Center, n, Color.LightPink);
                    //GeneralMethods.renderVector(this.Center, direction, Color.Yellow);
                    //GeneralMethods.renderVector(this.Center, realMovement, Color.Green);
                }
            }
        }



        const float MAX_DELTA_Y = 20f;

        private bool terrenoMuyEmpinado(Vector3 origin, Vector3 direction)
        {
            Vector3 deltaXZ = direction * 5;

            Vector3 target = new Vector3(origin.X, 0, origin.Z);
            target.Add(deltaXZ);
            float targetDeltaY = this.level.Terrain.getPosition(target.X, target.Z).Y - origin.Y;

            //if(targetDeltaY > MAX_DELTA_Y)GuiController.Instance.Logger.log("Pendiente: " + origin.Y + " -> " + (origin.Y + targetDeltaY) + " = " + targetDeltaY );

            return targetDeltaY > MAX_DELTA_Y;
        }

        public bool isDead(){
            return this.dead;
        }
    }
}
