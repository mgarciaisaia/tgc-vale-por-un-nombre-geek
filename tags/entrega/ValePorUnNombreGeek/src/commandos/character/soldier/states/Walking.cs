using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Walking : SoldierState
    {
        public Walking(Soldier _soldier)
            : base(_soldier)
        {
            //nothing to do
        }

        public override void update(float elapsedTime)
        {
            this.lookForCommando();
            this.soldier.goToTarget(elapsedTime);
            if (this.soldier.isOnTarget())
            {
                this.soldier.setNoTarget();
                this.soldier.setState(new Waiting(this.soldier, 0));
            }
        }
    }
}
