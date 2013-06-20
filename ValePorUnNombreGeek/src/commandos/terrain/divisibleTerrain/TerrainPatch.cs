using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain
{
    public class TerrainPatch: IRenderObject
    {
        protected DivisibleTerrain father;
        protected VertexBuffer vbTerrainPatch;
        public bool Enabled { get; set; }
        int totalVertices;
        public TgcBoundingBox BoundingBox { get; set; }
        public DivisibleTerrain Father { get { return this.father; } }


        public bool AlphaBlendEnable { get; set; }
        public Effect Effect { get; set; }
        public string Technique { get; set; }
        


        public TerrainPatch(DivisibleTerrain father, CustomVertex.PositionTextured[] data, TgcBoundingBox bb)
        {
            this.father = father;
            totalVertices = data.Length;
            this.BoundingBox = bb;
            this.vbTerrainPatch  = new VertexBuffer(typeof(CustomVertex.PositionTextured), data.Length, GuiController.Instance.D3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            this.Effect = father.Effect;
            this.Technique = father.Technique;
            this.Enabled = father.Enabled;
            this.RenderBB = false;
            vbTerrainPatch.SetData(data, 0, LockFlags.None);

            
        }

        public void render()
        {
            if (!Enabled) return;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            bool alphaBlendEnable = d3dDevice.RenderState.AlphaBlendEnable;
            d3dDevice.RenderState.AlphaBlendEnable = AlphaBlendEnable;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //Textura
            Effect.SetValue("texDiffuseMap", father.Texture);
            texturesManager.clear(1);

            GuiController.Instance.Shaders.setShaderMatrix(Effect, Matrix.Identity);
            
            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionTextured;
            Effect.Technique = Technique;
            d3dDevice.SetStreamSource(0, vbTerrainPatch, 0);
            
            //Render con shader
            Effect.Begin(0);
            Effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
            Effect.EndPass();
            Effect.End();

            if(RenderBB)BoundingBox.render();

            d3dDevice.RenderState.AlphaBlendEnable = alphaBlendEnable;
        }

        public void dispose()
        {
            vbTerrainPatch.Dispose();
        }





        public bool RenderBB { get; set; }
    }
}
