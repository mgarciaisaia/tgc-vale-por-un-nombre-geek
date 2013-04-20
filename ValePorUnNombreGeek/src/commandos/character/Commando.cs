using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Commando : Character
    {
        public Commando(Vector3 _position, Terrain _terrain)
            : base(_position, _terrain)
        {
            //nothing to do
        }

        public Commando(Vector3 _position)
            : base(_position)
        {
            //nothing to do
        }

        protected override void update(float elapsedTime)
        {
            if (!this.hasTarget()) return;

            this.goToTarget(elapsedTime);

            if (this.isOnTarget()) this.setNoTarget();
        }
    }
}
