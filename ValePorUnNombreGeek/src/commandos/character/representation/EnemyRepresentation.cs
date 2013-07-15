using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.representation.sound;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    class EnemyRepresentation:SkeletalRepresentation
    {

        public EnemyRepresentation(Vector3 position)
            : base(position)
        {
            this.sounds = CommandoSound.soldier();
        }

        protected override string getMesh()
        {
            return CommandosUI.Instance.MediaDir + "SkeletalAnimations\\BasicHuman\\" + "CS_Arctic-TgcSkeletalMesh.xml";
      
        }
    }
}
