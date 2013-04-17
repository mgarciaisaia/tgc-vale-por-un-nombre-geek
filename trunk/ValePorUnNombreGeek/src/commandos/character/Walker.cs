using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Walker : Character
    {
        private Targeteable target;
        private Terrain terrain;


        /*****************************************
         * INICIALIZACION
         * ***************************************/

        public Walker(Vector3 _position, Terrain _terrain)
            : base(getMesh(), getAnimations(), _position)
        {
            this.terrain = _terrain;
        }

        public Walker(Vector3 _position)
            : base(getMesh(), getAnimations(), _position)
        {
           
        }


        private static string[] getAnimations()
        {
            String myMediaDir = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\SkeletalAnimations\\BasicHuman\\Animations\\";
            String exMediaDir = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\";
            return new string[] { 
                    exMediaDir + "Walk-TgcSkeletalAnim.xml",
                    exMediaDir + "StandBy-TgcSkeletalAnim.xml",
                    exMediaDir + "Jump-TgcSkeletalAnim.xml",
                    myMediaDir + "Die-TgcSkeletalAnim.xml"
                };
        }

        protected static string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml";
        }

        /*****************************************
         * UPDATE & RENDER
         * ***************************************/

        protected override void update()
        {
            if (this.hasTarget()) this.goToTarget();
            if (this.selected && this.hasTarget())
            {
                //marcamos hacia donde vamos
                TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                marcaDePicking.Position = this.target.getPosition();
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
            Vector3 direccion = this.target.getPosition() - this.personaje.Position;
            direccion = direccion * (1 / direccion.Length());

            this.personaje.playAnimation("Walk", true);
            this.personaje.move(direccion);
            if(this.terrain!=null)this.personaje.Position = this.terrain.getPosition(this.personaje.Position.X, this.personaje.Position.Z);

            //nos fijamos si ya estamos en la posicion (o lo suficientemente cerca)
            if (GeneralMethods.isCloseTo(personaje.Position, this.target.getPosition()))
            {
                this.personaje.playAnimation("StandBy", true);
                this.target = null;
            }
        }

        private void setTarget(Targeteable _target)
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
