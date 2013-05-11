﻿using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class GeneralMethods //TODO darle un mejor nombre
    {

        public static bool isCloseTo(float a, float b, float delta)
        {
            return Math.Abs(a - b) < delta;
        }

        public static bool isCloseTo(Vector2 a, Vector2 b, float delta)
        {
            return Math.Abs(a.X - b.X) < delta &&
                  Math.Abs(a.Y - b.Y) < delta;
        }

        public static bool isCloseTo(Vector3 a, Vector3 b, float delta)
        {
            return Vector3.LengthSq(a-b)<= FastMath.Pow2(delta); 
        }

        public static float checkAngle(float angle)
        {
            float ret = angle;
            float limit = FastMath.TWO_PI;
            while (ret >= limit) ret -= limit;
            while (ret < 0) ret += limit;
            return ret;
        }

        public static float random(float min, float max)
        {
            Random rnd = new Random();
            float delta = max - min;
            return min + delta * (float)rnd.NextDouble();
        }

       


        public static float angleBetweenVersors( Vector3 versor1, Vector3 versor2)
        {
            float dot = Vector3.Dot(versor1, versor2);
            Vector3 cross = Vector3.Cross(versor1, versor2);
            float angle = FastMath.Acos(dot);

            if (dot < 0) //Esta atras
            {
                if (cross.Y > 0)
                //Atras y a izquierda   
                {
                    /*     ^   |
                     *      \  |
                     *       \ |  
                     *    -----|------>
                     *    
                     */

                    angle = FastMath.PI - angle;
                }
                else
                //Atras y la derecha
                {
                    /*     
                    *         |
                    *         |  
                    *    -----|------>
                    *        /|
                     *      / | 
                     *     v 
                    */

                   angle = FastMath.PI + angle;
                   
                }


            } else if (cross.Y < 0)
            {
                        /*           
                           *         | 
                           *         |   
                           *    -----|------>
                           *         | \
                            *        |  \
                            *            v
                            *      
                           */
                        
                        angle = FastMath.TWO_PI - angle;
            }
            

            return angle;
        }


        public static void renderVector(Vector3 origin, Vector3 n, Color color)
        {
            //TODO ver donde meter este metodo
            if (n.Equals(Vector3.Empty)) return;
            TgcArrow arrow = new TgcArrow();
            //arrow.Enabled = true;
            arrow.PStart = origin;
            arrow.PEnd = n * 70 + arrow.PStart;
            arrow.Thickness = 3;
            arrow.HeadSize = new Vector2(6, 6);
            arrow.BodyColor = color;
            arrow.updateValues();
            arrow.render();
        }
        
    }
}
