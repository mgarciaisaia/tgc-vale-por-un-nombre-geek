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
    //Intento de cono. No es el cono de vision, solo un cono. 
    class Cono
    {
        VertexBuffer vertexBuffer;
        Vector3 center;
        public Cono(Vector3 center){

            Device d3dDevice = GuiController.Instance.D3dDevice;
            this.center = center;
            //Crear vertexBuffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), 12, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
                

       


            //Configurar camara en rotacion
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0.5f, 0), 3f);

            //Modifiers
            GuiController.Instance.Modifiers.addFloat("Radius",1,10,1);
            GuiController.Instance.Modifiers.addFloat("Angle", 0, 180, 30);


        }

        private CustomVertex.PositionColored[] updateValues()


        {
            float radius = (float)GuiController.Instance.Modifiers.getValue("Radius");
            float angle = (float)GuiController.Instance.Modifiers.getValue("Angle");
            //Cargar informacion de vertices: (X,Y,Z) + Color
            CustomVertex.PositionColored[] data = new CustomVertex.PositionColored[12];



            data[0] = new CustomVertex.PositionColored(center.X, center.Y, center.Z, Color.Red.ToArgb());
            data[1] = new CustomVertex.PositionColored(radius, 0, 0, Color.Green.ToArgb());
            data[2] = new CustomVertex.PositionColored(radius, 1, 1, Color.Blue.ToArgb());

            data[3] = new CustomVertex.PositionColored(center.X, center.Y, center.Z, Color.Red.ToArgb());
            data[4] = new CustomVertex.PositionColored(radius, 0, 0, Color.Green.ToArgb());
            data[5] = new CustomVertex.PositionColored(radius, 1, -1, Color.Blue.ToArgb());


            data[6] = new CustomVertex.PositionColored(center.X, center.Y, center.Z, Color.Red.ToArgb());
            data[7] = new CustomVertex.PositionColored(radius, 1, -1, Color.Green.ToArgb());
            data[8] = new CustomVertex.PositionColored(radius, 2, -1, Color.Blue.ToArgb());//X


            data[9] = new CustomVertex.PositionColored(center.X, center.Y, center.Z, Color.Red.ToArgb());
            data[10] = new CustomVertex.PositionColored(radius, 2, -1, Color.Green.ToArgb());//XX
            data[11] = new CustomVertex.PositionColored(radius, 1, 1, Color.Blue.ToArgb());


            return data;
        }
       
        

      

          public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;




            //Almacenar información en VertexBuffer
            vertexBuffer.SetData( updateValues(), 0, LockFlags.None);

            updateValues();

            //Especificar formato de triangulos
            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            //Cargar VertexBuffer a renderizar
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);
            //Dibujar 1 primitiva
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 4);

        }

        public void dispose()
        {
            //liberar VertexBuffer
            vertexBuffer.Dispose();
        }


    }
}
