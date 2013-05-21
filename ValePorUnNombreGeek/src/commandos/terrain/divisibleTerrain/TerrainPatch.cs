using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain
{
    public class TerrainPatch
    {
        protected DivisibleTerrain father;
        protected VertexBuffer vbTerrainPatch;
        public bool Enabled { get; set; }
        int totalVertices;

        public TerrainPatch(DivisibleTerrain father, CustomVertex.PositionTextured[] data)
        {
            this.father = father;
            totalVertices = data.Length;
           
            this.vbTerrainPatch  = new VertexBuffer(typeof(CustomVertex.PositionTextured), data.Length, GuiController.Instance.D3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            
            vbTerrainPatch.SetData(data, 0, LockFlags.None);
        }

        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //Textura
            father.Effect.SetValue("texDiffuseMap", father.Texture);
            texturesManager.clear(1);

            GuiController.Instance.Shaders.setShaderMatrix(father.Effect, Matrix.Identity);
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            father.Effect.Technique = father.Technique;
            d3dDevice.SetStreamSource(0, vbTerrainPatch, 0);

            //Render con shader
            father.Effect.Begin(0);
            father.Effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
            father.Effect.EndPass();
            father.Effect.End();
        }

        public void dispose()
        {
            vbTerrainPatch.Dispose();
        }

       
    }
}
