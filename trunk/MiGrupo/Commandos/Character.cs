using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos
{
    class Character
    {
        TgcSkeletalMesh personaje;

        Vector3? objetivo;


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

            if (this.objetivo != null)
            {
                Vector3 direccion = (Vector3)this.objetivo - this.personaje.Position;
                direccion = direccion * (1 / direccion.Length());

                personaje.playAnimation("Walk", true);
                personaje.move(direccion);

                if (GeneralMethods.isCloseTo(personaje.Position, (Vector3)this.objetivo))
                {
                    personaje.playAnimation("StandBy", true);
                    objetivo = null;
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

        public void setTarget(Vector3 _target)
        {
            this.objetivo = _target;
        }
    }
}
