using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable
    {
       
        
        private bool dead;
        protected ICharacterRepresentation representation;
        public ICharacterRepresentation Representation
        {
            get { return this.representation; }
        }
        private ITargeteable target;
        protected Nivel nivel;
        protected float speed = 150;
        private Vector3 _position;
        

        public Nivel Nivel
        {
            get { return this.nivel; }
            set { this.nivel = value; }
        }
        public Vector3 Position
        {

            get { return this.representation.Position; }
            set { this.representation.Position = value; }
        }
        
        public bool Enabled //Solo se renderiza si esta en true. Sirve para las optimizaciones
        {

            get { return this.representation.Enabled; }
            set { this.representation.Enabled = value; }
        }

        public TgcBoundingBox BoundingBox()
        {
            return this.representation.BoundingBox;
        }

        public bool Selected
        {

            get { return representation.Selected; }
            set { if(!dead) this.representation.Selected = value; }
        }


        
    
        public Character(Vector3 _position)
        {
            this.loadCharacterRepresentation(_position);
            this.Selected = false;
            this.dead = false;
        }

      
        //Sobreescribible para que los hijos puedan usar otra representacion
        protected virtual void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new SkeletalRepresentation(position);
        }



      
        public void die()
        {
            this.dead = true;
            this.Selected = false;
            this.representation.die();
        }

        public virtual void dispose()
        {
            representation.dispose();
        }




        /*****************************************
         * UPDATE & RENDER
         * ***************************************/

        protected abstract void update(float elapsedTime);
        
        public virtual void render(float elapsedTime)
        {
            update(elapsedTime);

            if (this.Selected && this.hasTarget())
            {
                //marcamos hacia donde vamos
                TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                marcaDePicking.Position = this.target.Position;
                marcaDePicking.render();
            }

            representation.render();
        }


        /*****************************************
         * TARGET
         * ***************************************/

        protected bool hasTarget()
        {
            return this.target != null;
        }

        internal void goToTarget(float elapsedTime)
        {

            if (!this.hasTarget()) return;

            Vector3 direccion = this.target.Position - this.representation.Position;
            direccion.Y = 0;
            direccion.Normalize();

            float currentVelocity = speed * elapsedTime;
            direccion.Multiply(currentVelocity);

            this.representation.walk();
            this.representation.move(direccion);
            if (this.nivel == null) return;
            this.representation.Position = this.nivel.getPosition(this.representation.Position.X, this.representation.Position.Z);
        }

        internal bool isOnTarget()
        {
            if (this.target == null) return true;

            return GeneralMethods.isCloseTo(this.representation.Position, this.target.Position, 1);
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

        public abstract bool userCanMove();
    }
}
