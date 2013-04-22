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
        private const float MAX_WAITING_TIME = 10;

        private Random rnd = new Random();

        private float haciaDondeQuieroVer;
        private float tiempoMirando;
        private float tiempoEnQueMeCanso;

        private float haciaDondeEstaLaProximaPosicion;
        private bool dirty1 = true;

        private float haciaDondeEstoyViendo;
        private bool dirty2 = true;


        public Waiting(Soldier _soldier)
            : base(_soldier)
        {
            this.waitingTime = 0;
            this.haciaDondeQuieroVer = this.HaciaDondeEstoyViendo;
            this.tiempoMirando = 0;
            this.tiempoEnQueMeCanso = 2;
        }

        public override void update(float elapsedTime)
        {

            this.waitingTime += elapsedTime;

            if (this.waitingTime > MAX_WAITING_TIME)
            {
                if (this.haciaDondeQuieroVer != this.HaciaDondeEstaLaProximaPosicion)
                {
                    this.haciaDondeQuieroVer = this.HaciaDondeEstaLaProximaPosicion;
                }
                else
                {
                    if (GeneralMethods.isCloseTo(this.HaciaDondeEstoyViendo, this.haciaDondeQuieroVer, 2 * elapsedTime))
                    {
                        this.soldier.setNextPositionTarget();
                        this.soldier.setState(new Walking(this.soldier));
                        return;
                    }
                    //si no esta mirando hacia alla entra en el if de mas abajo
                }

            }
            mirar(elapsedTime);
        }

        private void mirar(float elapsedTime)
        {
            this.tiempoMirando += elapsedTime;
            if (this.tiempoMirando > this.tiempoEnQueMeCanso)
            {
                this.haciaDondeQuieroVer = 2 * (float)rnd.NextDouble();
                this.tiempoMirando = 0;
            }
            else
            {
                if (this.HaciaDondeEstoyViendo != this.haciaDondeQuieroVer)
                {
                    this.HaciaDondeEstoyViendo += (this.haciaDondeQuieroVer - this.HaciaDondeEstoyViendo) * 2 * elapsedTime;
                }
                else
                {
                    //miro un rato hasta que me canse
                }
            }
        }






        private float HaciaDondeEstaLaProximaPosicion
        {
            get
            {
                if (dirty1)
                {
                    Vector3 direction = this.soldier.getNextPositionTarget() - this.soldier.Position;
                    direction.Y = 0;
                    direction.Normalize();


                    haciaDondeEstaLaProximaPosicion = GeneralMethods.SignedAcos(direction.X);
                    dirty1 = false;
                }

                return haciaDondeEstaLaProximaPosicion;

            }
        }

        private float HaciaDondeEstoyViendo
        {
            get
            {
                if (dirty2)
                {
                    haciaDondeEstoyViendo = GeneralMethods.SignedAcos(this.soldier.Representation.Facing.X);
                    dirty2 = false;
                }
                return haciaDondeEstoyViendo;
            }
            set
            {
                Vector3 direction = new Vector3();
                direction.X = (float)Math.Cos(value);
                direction.Y = 0;
                direction.Z = (float)Math.Sin(value);
                this.soldier.Representation.faceTo(direction);
                this.haciaDondeEstoyViendo = value;
                dirty1 = true;
            }
        }


        /*
        private float cos(float value)
        {
            float ret = (float)Math.Cos(value);
            //if (value > 0.5f || value < 1.5) ret = -ret;
            return ret;
        }
        private float sin(float value)
        {
            float ret = (float)Math.Sin(value);
            //if (value > 1) ret = -ret;
            return ret;
        }
         */
    }
}
