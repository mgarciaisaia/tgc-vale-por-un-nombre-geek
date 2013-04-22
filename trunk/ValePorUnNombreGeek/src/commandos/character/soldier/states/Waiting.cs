using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Waiting : SoldierState
    {
        private float waitingTime;
        private const float MAX_WAITING_TIME = 30;

        private Random rnd = new Random();

        private float haciaDondeEstoyViendo;
        private float haciaDondeQuieroVer;
        private float velocidadDeRotacion;
        private float tiempoMirando;
        private float tiempoEnQueMeCanso;
        private const float ROTATION_SPEED = 2;


        public Waiting(Soldier _soldier)
            : base(_soldier)
        {
            this.waitingTime = 0;
            this.haciaDondeEstoyViendo = this.actualFacingAngle;
            this.haciaDondeQuieroVer = this.haciaDondeEstoyViendo;
            this.tiempoMirando = 0;
            this.tiempoEnQueMeCanso = 0;
        }

        public override void update(float elapsedTime)
        {
            if (this.waitingTime > MAX_WAITING_TIME)
            {
                if (this.haciaDondeQuieroVer != this.nextPositionFacingAngle)
                {
                    this.haciaDondeQuieroVer = this.nextPositionFacingAngle;
                }
                else
                {
                    if (GeneralMethods.isCloseTo(this.haciaDondeEstoyViendo, this.haciaDondeQuieroVer, ROTATION_SPEED * elapsedTime))
                    {
                        this.soldier.setNextPositionTarget();
                        this.soldier.setState(new Walking(this.soldier));
                        return;
                    }
                    //si no esta mirando hacia alla entra en el if de mas abajo
                }
            }
            else
            {
                this.waitingTime += elapsedTime;
            }


            this.tiempoMirando += elapsedTime;
            if (this.tiempoMirando > this.tiempoEnQueMeCanso)
            {
                float newAngle = (float)(this.haciaDondeEstoyViendo - 0.5 + rnd.NextDouble());
                if (newAngle < 0) newAngle += 2;
                if (newAngle >= 2) newAngle -= 2;

                this.haciaDondeQuieroVer = newAngle;

                if (2 - this.haciaDondeQuieroVer + this.haciaDondeEstoyViendo < this.haciaDondeQuieroVer - this.haciaDondeEstoyViendo)
                    this.velocidadDeRotacion = -ROTATION_SPEED;
                else this.velocidadDeRotacion = ROTATION_SPEED;

                this.tiempoMirando = 0;
                this.tiempoEnQueMeCanso = (float)(2 + 5 * rnd.NextDouble());
            }
            else
            {
                if (this.haciaDondeEstoyViendo != this.haciaDondeQuieroVer)
                {
                    this.haciaDondeEstoyViendo += (this.haciaDondeQuieroVer - this.haciaDondeEstoyViendo) * this.velocidadDeRotacion * elapsedTime;
                    this.actualFacingAngle = this.haciaDondeEstoyViendo;
                }
                else
                {
                    //miro un rato hasta que me canse
                }
            }
        }


        #region FacingFunctions

        private float actualFacingAngle
        {
            get
            {
                return this.angle(this.soldier.Representation.Facing.X, this.soldier.Representation.Facing.Z);
            }
            set
            {
                Vector3 direction = new Vector3();
                direction.X = this.cos(value);
                direction.Y = 0;
                direction.Z = this.sin(value);
                this.soldier.Representation.faceTo(direction);
            }
        }

        private float nextPositionFacingAngle
        {
            get
            {
                Vector3 direction = this.soldier.getNextPositionTarget() - this.soldier.Position;
                direction.Y = 0;
                direction.Normalize();

                return this.angle(direction.X, direction.Z);
            }
        }

        #endregion


        #region AngleFunctions

        //Funciones auxiliares para trabajar con angulos.
        private float cos(float angle)
        {
            return (float)Math.Cos(angle * Math.PI);
        }
        private float sin(float angle)
        {
            return (float)Math.Sin(angle * Math.PI);
        }
        private float angle(float x, float y)
        {
            /* Antes que nada, si, esta funcion es espantosa.
             * Poco performante, poco intuitiva, muy pero muy fea.
             * Lo unico bueno es que se la llama una sola vez por waitpoint.
             * 
             * Si alguno tiene una mejor idea de como resolver este problema
             * por favor reescriba toda la funcion. A mi no me da para mas
             * la cabeza.
             */

            double acosX = Math.Acos(x) / Math.PI;
            double asinY = Math.Asin(y) / Math.PI;
            acosX = Math.Round(acosX, 1);
            asinY = Math.Round(asinY, 1);

            List<double> xResults = new List<double> { acosX, 2 - acosX };
            List<double> yResults = new List<double> { asinY, 1 - asinY };

            var results = xResults.Intersect(yResults);

            return (float)results.First();
        }

        #endregion
    }
}
