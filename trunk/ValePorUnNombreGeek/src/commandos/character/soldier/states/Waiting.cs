using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Waiting : SoldierState
    {
        private float waitingTime;
        private float maxWaitingTime;

        private float boringSeenAngle;
        private float boringSeenAngleLimit;
        private float boringSeenAngleDelta;

        private Vector3 boringWatching;
        private Random rnd = new Random();

        public Waiting(Soldier _soldier)
            : base(_soldier)
        {
            this.waitingTime = 0;
            this.maxWaitingTime = 5;


            Vector3 facing = this.soldier.Representation.Facing;
            this.boringSeenAngle = (float)Math.Acos(facing.X);

            this.boringSeenAngleLimit = this.boringSeenAngle + 2 * (float)rnd.NextDouble();
            this.boringSeenAngleDelta = 0.5f;
        }

        public override void update(float elapsedTime)
        {
            //Si espero suficiente tiempo, fijar el proximo waitpoint como objetivo
            this.waitingTime += elapsedTime;

            if (this.waitingTime > this.maxWaitingTime)
            {
                this.soldier.setNextPositionTarget();
                this.soldier.setState(new Walking(this.soldier));
                return;
            }

            this.boringSeenAngle += this.boringSeenAngleDelta * elapsedTime;

            this.boringWatching.X = (float)Math.Cos(this.boringSeenAngle);
            this.boringWatching.Z = (float)Math.Sin(this.boringSeenAngle);
            this.soldier.Representation.faceTo(this.boringWatching);

            if (this.boringSeenAngleDelta > 0 && (this.boringSeenAngle > this.boringSeenAngleLimit))
            {
                this.boringSeenAngleDelta = -this.boringSeenAngleDelta;
                this.boringSeenAngleLimit = -2 * (float) rnd.NextDouble();
            }
            if (this.boringSeenAngleDelta < 0 && (this.boringSeenAngle < this.boringSeenAngleLimit))
            {
                this.boringSeenAngleDelta = -this.boringSeenAngleDelta;
                this.boringSeenAngleLimit = 2 * (float)rnd.NextDouble();
            }

        }
    }
}
