using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Walker : Character
    {
        private ITargeteable target;
        private Terrain terrain;


        /*****************************************
         * INICIALIZACION
         * ***************************************/

        public Walker(Vector3 _position, Terrain _terrain)
            : base(_position)
        {
            this.terrain = _terrain;
        }

        public Walker(Vector3 _position)
            : base(_position)
        {
           
        }


     

        /*****************************************
         * UPDATE & RENDER
         * ***************************************/

        protected override void update()
        {
            if (this.hasTarget()) this.goToTarget();
            if (this.Selected && this.hasTarget())
            {
                //marcamos hacia donde vamos
                TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                marcaDePicking.Position = this.target.Position;
                marcaDePicking.render();
            }
        }


        /*****************************************
         * TARGET
         * ***************************************/

        private bool hasTarget()
        {
            return this.target != null;
        }

        private void goToTarget()
        {
            //primero nos movemos
            Vector3 direccion = this.target.Position - this.representation.Position;
            direccion = direccion * (1 / direccion.Length());

            this.representation.walk();
            this.representation.move(direccion);
            if(this.terrain!=null)this.representation.Position = this.terrain.getPosition(this.representation.Position.X, this.representation.Position.Z);

            //nos fijamos si ya estamos en la posicion (o lo suficientemente cerca)
            if (GeneralMethods.isCloseTo(representation.Position, this.target.Position))
            {
                this.representation.standBy();
                this.target = null;
            }
        }

        private void setTarget(ITargeteable _target)
        {
            this.target = _target;
        }

        public void setPositionTarget(Vector3 pos)
        {
            this.setTarget(new TargeteablePosition(pos));
        }

        public void setCharacterTarget(Character ch)
        {
            this.setTarget(ch);
        }
    }
}
