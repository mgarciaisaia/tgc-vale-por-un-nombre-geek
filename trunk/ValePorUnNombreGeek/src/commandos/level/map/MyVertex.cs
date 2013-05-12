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
        #region TransformedTextured
        public static readonly VertexElement[] transformedTexturedElements = new VertexElement[]
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

        
        public static VertexDeclaration TransformedTexturedDeclaration = new VertexDeclaration(GuiController.Instance.D3dDevice, transformedTexturedElements);

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

        #endregion


        #region TransformedDoubleTextured
        public static readonly VertexElement[] transformedDoubleTexturedElements = new VertexElement[]
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
           new VertexElement(0, 6*sizeof(float), DeclarationType.Float2,
                                     DeclarationMethod.Default,
                                     DeclarationUsage.TextureCoordinate, 1),
                            
            VertexElement.VertexDeclarationEnd 
        };

        public static VertexDeclaration TransformedDoubleTexturedDeclaration = new VertexDeclaration(GuiController.Instance.D3dDevice, transformedDoubleTexturedElements);

        public struct TransformedDoubleTextured
        {
            public float X;
            public float Y;
            public float Z;
            public float W;
            public float Tu1;
            public float Tv1;
            public float Tu2;
            public float Tv2;



            public TransformedDoubleTextured(float X, float Y, float Z, float W, float Tu1, float Tv1, float Tu2, float Tv2)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.W = W;
                this.Tu1 = Tu1;
                this.Tv1 = Tv1;
                this.Tu2 = Tu2;
                this.Tv2 = Tv2;
            }
        }

    }
        #endregion
}
