using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    abstract class OnWaitpoint : SoldierState
    {
        protected float timeOnWaitpoint;
        private const float MAX_TIME_ON_WAITPOINT = 30;

        public OnWaitpoint(Soldier _soldier, float _timeOnWaitpoint)
            : base(_soldier)
        {
            this.timeOnWaitpoint = _timeOnWaitpoint;
        }

        public override void update(float elapsedTime)
        {
            this.timeOnWaitpoint += elapsedTime;

            if (this.timeOnWaitpoint > MAX_TIME_ON_WAITPOINT)
            {
                this.soldier.setNextPositionTarget();
                this.soldier.setState(new Walking(this.soldier));
                return;
            }

            this.onWaitpointUpdate(elapsedTime);
        }

        public abstract void onWaitpointUpdate(float elapsedTime);
    }
}
