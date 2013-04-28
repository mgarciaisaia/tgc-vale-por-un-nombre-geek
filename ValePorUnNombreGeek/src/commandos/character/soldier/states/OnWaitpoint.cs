using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    abstract class OnWaitpoint : SoldierState
    {
        protected float timeOnWaitpoint;
        private const float MAX_TIME_ON_WAITPOINT = 10;

        public OnWaitpoint(Soldier _soldier, float _timeOnWaitpoint)
            : base(_soldier)
        {
            this.timeOnWaitpoint = _timeOnWaitpoint;
        }

        /// <summary>
        /// Ejecuta la logica y se fija si es momento de avanzar al proximo waitpoint.
        /// </summary>
        public override void update(float elapsedTime)
        {
            this.timeOnWaitpoint += elapsedTime;

            if (this.timeOnWaitpoint > MAX_TIME_ON_WAITPOINT)
            {
                Vector3 angleZeroVector = this.soldier.Representation.getAngleZeroVector();
                Vector3 nextWaitpointDirection = this.soldier.getNextPositionTarget() - this.soldier.Position;
                nextWaitpointDirection.Y = 0;
                nextWaitpointDirection.Normalize();
                float desiredAngle = GeneralMethods.angleBetweenVersors(angleZeroVector, nextWaitpointDirection);

                bool clockwise = this.shallRotateClockwise(desiredAngle);

                this.soldier.setState(new RotatingToNextWaitpoint(this.soldier, desiredAngle, clockwise, 0));
                return;
            }

            this.onWaitpointUpdate(elapsedTime);
        }

        /// <summary>
        /// Indica para que lado girar desde el angulo actual hacia el deseado.
        /// </summary>
        protected bool shallRotateClockwise(float desiredAngle)
        {
            float actualAngle = this.soldier.Representation.FacingAngle;

            bool clockwise;
            float a = actualAngle;
            float b = desiredAngle;

            if (a < b)
                clockwise = (b - a < FastMath.PI);
            else
                clockwise = (a - b > FastMath.PI);

            return clockwise;
        }

        /// <summary>
        /// Ejecuta la logica de mirar aleatoriamente mientras esta detenido en el waitpoint.
        /// </summary>
        public abstract void onWaitpointUpdate(float elapsedTime);
    }
}
