﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    abstract class SoldierState
    {
        protected Soldier soldier;

        public SoldierState(Soldier _soldier)
        {
            this.soldier = _soldier;
            
        }

        public abstract void update(float elapsedTime);

        public void lookForCommando(){
            Commando c;
            if (soldier.canSeeACommando(out c))
            {
                soldier.setState(new Chasing(soldier, c));
            }
        }
    }
}
