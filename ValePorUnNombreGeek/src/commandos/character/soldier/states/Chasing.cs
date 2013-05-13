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
        TgcStaticSound talk;
        public Chasing(Soldier _soldier, Commando commando):base(_soldier){
            this.commando = commando;
            talk = new TgcStaticSound();
            talk.loadSound(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek//" + "Sounds//TALK.WAV");

        }
        public override void update(float elapsedTime)
        {
            if (this.soldier.canSee(commando))
            {
                this.soldier.setPositionTarget(commando.Position); //pablo

                if(GeneralMethods.isCloseTo(this.soldier.Position, commando.Position, 100)) commando.Life.decrement(elapsedTime*10);
                if (GeneralMethods.isCloseTo(this.soldier.Position, commando.Position, this.soldier.Radius))
                {
                    this.soldier.Representation.talk();
                    
                    talk.play();
                  
                }
                else
                {
                    //this.soldier.setCharacterTarget(commando);
                    this.soldier.goToTarget(elapsedTime);
                    
                }

            }
            else
            {
                //if (soldier.canSeeACommando(out commando)) return;
                if (soldier.hasTarget()) //pablo
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
