using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Waiting : SoldierState
    {
        private float waitingTime;
        private float maxWaitingTime;

        public Waiting(Soldier _soldier)
            : base(_soldier)
        {
            this.waitingTime = 0;
            this.maxWaitingTime = 5;
        }

        public override void update(float elapsedTime)
        {
            //Si espero suficiente tiempo, fijar el proximo waitpoint como objetivo
            this.waitingTime += elapsedTime;
            if (this.waitingTime > this.maxWaitingTime)
            {
                this.soldier.setNextPositionTarget();
                this.soldier.setState(new Walking(this.soldier));
            }
        }
    }
}
