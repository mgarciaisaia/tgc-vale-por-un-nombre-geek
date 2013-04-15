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

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    public class Character : Targeteable
    {
        TgcSkeletalMesh personaje;

        private Targeteable target;
        public bool selected = false;

        public Character(Vector3 _position)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                getMesh(),
                getAnimations());
            personaje.playAnimation("StandBy", true);
            personaje.Position = _position;
        }

        protected virtual string[] getAnimations()
        {
            return new string[] { 
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Walk-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "StandBy-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Jump-TgcSkeletalAnim.xml"
                };
        }

       protected virtual string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml";
        }

        public void render(float elapsedTime)
        {
            //personaje.rotateY(rotAngle);
            //personaje.playAnimation("Walk", true);
            //personaje.playAnimation("StandBy", true);

            /*
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Sin(personaje.Rotation.Y) * moveForward,
                    0,
                    FastMath.Cos(personaje.Rotation.Y) * moveForward
                    );
            }
            */

            if (this.hasTarget())
            {
                this.goToTarget();
            }

            personaje.updateAnimation();
            personaje.render();
            if (this.selected)
            {
                personaje.BoundingBox.render();

                if (this.hasTarget())
                {
                    //marcamos hacia donde vamos
                    TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                    marcaDePicking.Position = this.target.getPosition();
                    marcaDePicking.render();
                }
            }
        }

        protected virtual void goToTarget()
        {
            //primero nos movemos
            Vector3 direccion = this.target.getPosition() - this.personaje.Position;
            direccion = direccion * (1 / direccion.Length());

            personaje.playAnimation("Walk", true);
            personaje.move(direccion);

            //nos fijamos si ya estamos en la posicion (o lo suficientemente cerca)
            if (GeneralMethods.isCloseTo(personaje.Position, this.target.getPosition()))
            {
                personaje.playAnimation("StandBy", true);
                this.target = null;
            }
        }

        private bool hasTarget()
        {
            return this.target != null;
        }

        public void dispose()
        {
            personaje.dispose();
        }

        public Vector3 getPosition()
        {
            return personaje.Position;
        }

        public TgcBoundingBox BoundingBox()
        {
            return this.personaje.BoundingBox;
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
