using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map
{


    public class MyVertex
    {
        public static readonly VertexElement[] elements = new VertexElement[]
        {
            new VertexElement(0, 0, DeclarationType.Float3,
                                    DeclarationMethod.Default,
                                    DeclarationUsage.PositionTransformed, 0),
        
            new VertexElement(0, 3*sizeof(float), DeclarationType.Float1,
                                    DeclarationMethod.Default,
                                    DeclarationUsage.Depth, 0),              
            new VertexElement(0, 4*sizeof(float), DeclarationType.Float2,
                                     DeclarationMethod.Default,
                                     DeclarationUsage.TextureCoordinate, 0),
                            
            VertexElement.VertexDeclarationEnd 
        };

        // Use the vertex element array to create a vertex declaration.
        public static VertexDeclaration TransformedTexturedDeclaration = new VertexDeclaration(GuiController.Instance.D3dDevice, elements);

        public struct TransformedTextured
        {
            public float X;
            public float Y;
            public float Z;
            public float W;
            public float Tu1;
            public float Tv1;

   

            public TransformedTextured(float X, float Y, float Z, float W, float Tu1, float Tv1)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.W = W;
                this.Tu1 = Tu1;
                this.Tv1 = Tv1;
            }
        }
    }
}
