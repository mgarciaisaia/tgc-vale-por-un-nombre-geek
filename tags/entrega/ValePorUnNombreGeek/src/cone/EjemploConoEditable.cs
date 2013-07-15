using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.cone
{
    /// <summary>
    ///
    /// 
    /// </summary>
    public class ConoVertexBuffer : TgcExample
    {
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "VPUNG:Cono";
        }

        public override string getDescription()
        {
            return "ValePorUnNombreGeek. Prueba de cono editable.";
        }

        Cone cono;
        public override void init()
        {
            cono = new Cone(new Vector3(0,0,0));
            
            //Modifiers
            GuiController.Instance.Modifiers.addFloat("Length", 0, 10, 5);
            GuiController.Instance.Modifiers.addFloat("Angle", 0, 90, 30);
            GuiController.Instance.Modifiers.addInt("Triangles", 0, 100, 6);

        }


        public override void render(float elapsedTime)
        {
            cono.Length = (float)GuiController.Instance.Modifiers.getValue("Length");
            cono.Angle = FastMath.ToRad((float)GuiController.Instance.Modifiers.getValue("Angle"));
            int triangles = (int)GuiController.Instance.Modifiers.getValue("Triangles");
            if (triangles == 0)
                triangles = 6;
            cono.Triangles = triangles;
           
            cono.render();

        }

        public override void close()
        {
            cono.dispose();
        }

    }
}
