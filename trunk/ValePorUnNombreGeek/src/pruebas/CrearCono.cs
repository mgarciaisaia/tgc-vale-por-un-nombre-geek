using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas
{
    /// <summary>
    ///
    /// 
    /// </summary>
    public class ConoVertexBuffer : TgcExample
    {

        //Vertex buffer que se va a utilizar
        VertexBuffer vertexBuffer;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Cono VertexBuffer";
        }

        public override string getDescription()
        {
            return "Prueba de graficado de cono";
        }

        Cono cono;
        public override void init()
        {
            cono = new Cono(new Vector3(0,0,0));

        }


        public override void render(float elapsedTime)
        {
            cono.render();

        }

        public override void close()
        {
            cono.dispose();
        }

    }
}
