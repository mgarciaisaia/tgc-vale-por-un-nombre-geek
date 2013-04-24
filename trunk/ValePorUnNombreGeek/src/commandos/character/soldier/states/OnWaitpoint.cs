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

        public override void update(float elapsedTime)
        {
            this.timeOnWaitpoint += elapsedTime;

            if (this.timeOnWaitpoint > MAX_TIME_ON_WAITPOINT)
            {
                Vector3 angleZeroVector = this.soldier.Representation.getAngleZeroVector();
                Vector3 nextWaitpointDirection = this.soldier.getNextPositionTarget() - this.soldier.Position;
                nextWaitpointDirection.Y = 0;
                nextWaitpointDirection.Normalize();

                float desiredAngle = FastMath.Acos(Vector3.Dot(angleZeroVector, nextWaitpointDirection));

                Vector3 rotationAxis = Vector3.Cross(angleZeroVector, nextWaitpointDirection);
                bool clockwise;
                if (rotationAxis.Y > 0) clockwise = false; else clockwise = true;


                try
                {
                    GuiController.Instance.UserVars.setValue("initialAngle", this.soldier.Representation.FacingAngle / FastMath.PI);
                    GuiController.Instance.UserVars.setValue("desiredAngle", desiredAngle / FastMath.PI);
                    GuiController.Instance.UserVars.setValue("clockwise", clockwise);
                }
                catch (Exception e)
                {
                    GuiController.Instance.UserVars.addVar("initialAngle");
                    GuiController.Instance.UserVars.addVar("desiredAngle");
                    GuiController.Instance.UserVars.addVar("clockwise");
                    GuiController.Instance.UserVars.setValue("initialAngle", this.soldier.Representation.FacingAngle / FastMath.PI);
                    GuiController.Instance.UserVars.setValue("desiredAngle", desiredAngle / FastMath.PI);
                    GuiController.Instance.UserVars.setValue("clockwise", clockwise);
                }


                this.soldier.setState(new RotatingToNextWaitpoint(this.soldier, desiredAngle, clockwise, 0));
                return;
            }

            this.onWaitpointUpdate(elapsedTime);
        }

        public abstract void onWaitpointUpdate(float elapsedTime);
    }
}
