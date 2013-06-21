using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Chasing:SoldierState
    {
        Commando commando;
        
        public Chasing(Soldier _soldier, Commando commando):base(_soldier){
            this.commando = commando;
        }
        public override void update(float elapsedTime)
        {   
            if (this.soldier.canSee(commando))
            {
                this.soldier.setPositionTarget(commando.Position); //pablo
                commando.getShot(elapsedTime * 50);
                if (GeneralMethods.isCloseTo(this.soldier.Position, commando.Position, this.soldier.Radius * 2.5f))
                    this.soldier.standBy();
                else
                    this.soldier.goToTarget(elapsedTime);

            }
            else
            {
                if (soldier.hasTarget() && !commando.isDead()) //pablo
                {
                    this.soldier.goToTarget(elapsedTime);
                    return;
                }
                this.soldier.setPreviousPositionTarget();
                this.soldier.setState(new Waiting(soldier, 0, true));
            }
        }
    }
}
