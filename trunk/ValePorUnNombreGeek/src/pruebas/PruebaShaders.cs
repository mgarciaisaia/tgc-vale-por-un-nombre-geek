using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas
{
 
    public class PruebaShaders : TgcExample
    {

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "PruebaShaders";
        }

        public override string getDescription()
        {
            return "Pruebas con HLSL.";
        }

        SkeletalRepresentation skeletal;
        Effect effect;
        
        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            skeletal = new SkeletalRepresentation(new Vector3(0, 0, 0));
          
            effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\shaders.fx");
            skeletal.Effect = effect;
            
            GuiController.Instance.RotCamera.targetObject(skeletal.BoundingBox);
            GuiController.Instance.RotCamera.CameraDistance = 100;
        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            skeletal.Effect.SetValue("selectionColor", ColorValue.FromColor(Color.Red));
            skeletal.Technique = "SKELETAL_DIFFUSE_MAP_SELECTED";
            skeletal.render();
       }

        public override void close()
        {
            
            skeletal.dispose();

        }

    }
}

