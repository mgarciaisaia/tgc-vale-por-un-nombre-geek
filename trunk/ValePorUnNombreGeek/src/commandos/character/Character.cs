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

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : ITargeteable
    {
       
        
        private bool dead;
        protected ICharacterRepresentation representation;

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



        public Character(Vector3 position)
        {
            this.loadCharacterRepresentation(position);
            this.Selected = false;
            this.dead = false;
        }

        //Sobreescribible para que los hijos puedan usar otra representacion
        protected virtual void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new SkeletalRepresentation(position);
        }

        public virtual void render(float elapsedTime)
        {
            this.update();

            representation.render();
        }

        protected virtual void update()
        {
        }

        public void die()
        {
            this.dead = true;
            this.Selected = false;
            this.representation.die();
        }

        public void dispose()
        {
            representation.dispose();
        }

        
    }
}
