using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos.target;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class Character : Targeteable
    {
        TgcSkeletalMesh personaje;

        Targeteable target;


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
                Vector3 direccion = this.target.getPosition() - this.personaje.Position;
                direccion = direccion * (1 / direccion.Length());

                personaje.playAnimation("Walk", true);
                personaje.move(direccion);

                if (GeneralMethods.isCloseTo(personaje.Position, this.target.getPosition()))
                {
                    personaje.playAnimation("StandBy", true);
                    this.target = null;
                }
            }


            personaje.render();
        }

        public void dispose()
        {
            personaje.dispose();
        }

        public Vector3 getPosition()
        {
            return personaje.Position;
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
