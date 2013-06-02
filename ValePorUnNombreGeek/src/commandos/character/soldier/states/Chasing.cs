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
        TgcStaticSound shoot;
        public Chasing(Soldier _soldier, Commando commando):base(_soldier){
            this.commando = commando;
            shoot = new TgcStaticSound();
            //shoot.loadSound(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek//" + "Sounds//SHOT.WAV");

        }
        public override void update(float elapsedTime)
        {   
            if (this.soldier.canSee(commando))
            {
                this.soldier.setPositionTarget(commando.Position); //pablo
                commando.Life.decrement(elapsedTime * 40);
                //shoot.play();
                if (GeneralMethods.isCloseTo(this.soldier.Position, commando.Position, this.soldier.Radius * 2.5f))
                {
                    commando.Representation.standBy();
                    
                  
                }
                else
                {
                    this.soldier.goToTarget(elapsedTime);
                    
                }

            }
            else
            {
                //if (soldier.canSeeACommando(out commando)) return;
                if (soldier.hasTarget() && !commando.isDead()) //pablo
                {
                    this.soldier.goToTarget(elapsedTime);
                    return;
                }
                this.soldier.setPreviousPositionTarget();
                shoot.dispose();
                this.soldier.setState(new Waiting(soldier, 0, true));
            }
        }
    }
}
