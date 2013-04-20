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
        private const float MAX_WAITING_TIME=10;

        private Random rnd = new Random();

        private float haciaDondeQuieroVer;
        private float tiempoMirando;
        private float tiempoEnQueMeCanso;


        public Waiting(Soldier _soldier)
            : base(_soldier)
        {
            this.waitingTime = 0;
            this.haciaDondeQuieroVer = this.haciaDondeEstoyViendo;
            this.tiempoMirando = 0;
            this.tiempoEnQueMeCanso = 2;
        }

        public override void update(float elapsedTime)
        {
            if (this.waitingTime > MAX_WAITING_TIME)
            {
                if (this.haciaDondeQuieroVer != this.haciaDondeEstaLaProximaPosicion)
                {
                    this.haciaDondeQuieroVer = this.haciaDondeEstaLaProximaPosicion;
                }
                else
                {
                    if (GeneralMethods.isCloseTo(this.haciaDondeEstoyViendo, this.haciaDondeQuieroVer, 0.05f))
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
                this.haciaDondeQuieroVer = 2 * (float)rnd.NextDouble();
                this.tiempoMirando = 0;
            }
            else
            {
                if (this.haciaDondeEstoyViendo != this.haciaDondeQuieroVer)
                {
                    this.haciaDondeEstoyViendo += (this.haciaDondeQuieroVer - this.haciaDondeEstoyViendo) * 2 * elapsedTime;
                }
                else
                {
                    //miro un rato hasta que me canse
                }
            }
        }





        private float haciaDondeEstoyViendo
        {
            get
            {
                return (float)Math.Acos(this.soldier.Representation.Facing.X);
            }
            set
            {
                Vector3 direction = new Vector3();
                direction.X = (float)Math.Cos(value);
                direction.Y = 0;
                direction.Z = (float)Math.Sin(value);
                this.soldier.Representation.faceTo(direction);
            }
        }

        private float haciaDondeEstaLaProximaPosicion
        {
            get
            {
                Vector3 direction = this.soldier.getNextPositionTarget() - this.soldier.Position;
                direction.Y = 0;
                direction.Normalize();

                return (float)Math.Acos(direction.X);
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
