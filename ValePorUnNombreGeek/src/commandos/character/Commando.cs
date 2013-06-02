﻿using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Commando : Character
    {
        private const float WALK_SPEED = 150;
        private float speed;

        public Commando(Vector3 _position)
            : base(_position)
        {
            this.speed = WALK_SPEED;
        }


        public override float Speed
        {
            get { return this.speed; }
        }

        public override void update(float elapsedTime)
        {
            this.killNearEnemies();

            if (!this.hasTarget()) return;

            this.goToTarget(elapsedTime);

            

            if (this.isOnTarget()) this.setNoTarget();
        }

        public override void setPositionTarget(Vector3 pos)
        {
            foreach (Character character in this.level.charactersNear(pos))
            {
                if (character.isEnemyOf(this))
                {
                    this.setCharacterTarget(character);
                    return;
                }
            }
            base.setPositionTarget(pos);
        }

        private void killNearEnemies()
        {
            foreach(Character target in this.level.Enemies)
            {
                if(this.isNear(target)) {
                    target.die();
                }
            }
        }

        public override bool OwnedByUser
        {
            get { return true; }
        }

        public void talk()
        {
            this.Representation.talk();
            this.speed = WALK_SPEED / 3;
        }


        public void walk()
        {
            this.Representation.walk();
            this.speed = WALK_SPEED;
        }
    }
}
