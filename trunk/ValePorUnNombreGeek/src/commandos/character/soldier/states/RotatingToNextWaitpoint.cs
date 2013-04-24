using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class RotatingToNextWaitpoint : Rotating
    {
        public RotatingToNextWaitpoint(Soldier _soldier, float _desiredAngle, bool _clockwise, float _timeOnWaitpoint)
            : base(_soldier, _desiredAngle, _clockwise, _timeOnWaitpoint)
        {
            //nothing to do
        }

        protected override void onDesiredAngle()
        {
            this.soldier.setNextPositionTarget();
            this.soldier.setState(new Walking(this.soldier));
        }
    }
}
