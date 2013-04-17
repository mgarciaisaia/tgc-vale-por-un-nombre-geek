using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono;

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
            
            //Modifiers
            GuiController.Instance.Modifiers.addFloat("Radius", 0, 10, 5);
            GuiController.Instance.Modifiers.addFloat("Angle", 0, 180, 30);
            GuiController.Instance.Modifiers.addInt("Triangles", 4, 100, 6);

        }


        public override void render(float elapsedTime)
        {
            cono.Radius = (float)GuiController.Instance.Modifiers.getValue("Radius");
            cono.Angle = (float)GuiController.Instance.Modifiers.getValue("Angle");
            cono.Triangles = (int)GuiController.Instance.Modifiers.getValue("Triangles");
           
            cono.render();

        }

        public override void close()
        {
            cono.dispose();
        }

    }
}
