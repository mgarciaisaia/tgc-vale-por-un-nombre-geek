using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class Character : Targeteable
    {
        TgcSkeletalMesh personaje;

        private Targeteable target;
        public bool drawBoundingBox = false;

        public Character(Vector3 _position)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml",
                new string[] { 
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Walk-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "StandBy-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Jump-TgcSkeletalAnim.xml"
                });
            personaje.playAnimation("StandBy", true);
            personaje.Position = _position;
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

            if (this.target != null)
            {
                //primero nos movemos
                Vector3 direccion = this.target.getPosition() - this.personaje.Position;
                direccion = direccion * (1 / direccion.Length());

                personaje.playAnimation("Walk", true);
                personaje.move(direccion);

                //marcamos hacia donde vamos
                TgcBox marcaDePicking = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
                marcaDePicking.Position = this.target.getPosition();
                marcaDePicking.render();

                //nos fijamos si ya estamos en la posicion (o lo suficientemente cerca)
                if (GeneralMethods.isCloseTo(personaje.Position, this.target.getPosition()))
                {
                    personaje.playAnimation("StandBy", true);
                    this.target = null;
                }
            }


            personaje.render();
            if (this.drawBoundingBox) personaje.BoundingBox.render();
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
