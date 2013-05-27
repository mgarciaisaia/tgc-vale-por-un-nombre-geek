using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer;
using System.Windows.Forms;
using TgcViewer.Utils.Shaders;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    class ShadowRenderer:DefaultRenderer
    {
        // Shadow map
        readonly int SHADOWMAP_SIZE = 1024;
        Texture g_pShadowMap;    // Texture to which the shadow map is rendered
        Surface g_pDSShadow;     // Depth-stencil buffer for rendering to shadow map
        Matrix g_mShadowProj;    // Projection matrix for shadow map
        Vector3 g_LightPos;						// posicion de la luz actual (la que estoy analizando)
        Vector3 g_LightDir;						// direccion de la luz actual
        Matrix g_LightView;						// matriz de view del light
        float near_plane = 2f;
        float far_plane = 1500f;
        Matrix projection;
        TgcArrow arrow;

        public Vector3 LightLookFrom { get { return g_LightPos; } set { g_LightPos = value; updateLightDir(); updateArrow(); } }

        private void updateLightDir()
        {
            g_LightDir = lightLookAt - g_LightPos; 
            g_LightDir.Normalize();
        }

        private void updateArrow()
        {
            arrow.PStart = g_LightPos;
            arrow.PEnd = g_LightPos + g_LightDir * 300f;
            arrow.updateValues();
        }

        private Vector3 lightLookAt;
        public Vector3 LightLookAt { get { return lightLookAt; } set { lightLookAt = value; updateLightDir(); updateArrow(); } }
        
        public ShadowRenderer():base()
        {


            Device d3dDevice = GuiController.Instance.D3dDevice;
           
            //--------------------------------------------------------------------------------------
            // Creo el shadowmap. 
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(d3dDevice, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
                                        1, Usage.RenderTarget, Format.R32F,
                                        Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamaño que el shadowmap, y que no tenga 
            // multisample, etc etc.
            g_pDSShadow = d3dDevice.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                                                             SHADOWMAP_SIZE,
                                                             DepthFormat.D24S8,
                                                             MultiSampleType.None,
                                                             0,
                                                             true);

            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya 
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(80),
                aspectRatio, 50, 5000);

            this.projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                aspectRatio, near_plane, far_plane);

            arrow = new TgcArrow();
            arrow.Thickness = 10f;
            arrow.HeadSize = new Vector2(20f, 20f);
            arrow.BodyColor = Color.Blue;
        }


        public override void render()
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;

            Device device = GuiController.Instance.D3dDevice;
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;

                 
            // Shadow maps:
            device.EndScene();      // termino el thread anterior

           
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //Genero el shadow map
            RenderShadowMap();

            device.BeginScene();
            // dibujo la escena pp dicha
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            RenderScene(false);

            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            arrow.render();
        }



        public void RenderShadowMap()
        {
            Device device = GuiController.Instance.D3dDevice;
            Matrix OldProjection = device.Transform.Projection;

            device.Transform.Projection = this.projection;

            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            effect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = Matrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new Vector3(0, 0, 1));


            // inicializacion standard: 
            effect.SetValue("g_mProjLight", g_mShadowProj);
            effect.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades. 
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pShadowSurf);
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDSShadow;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();

            // Hago el render de la escena pp dicha
            effect.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);

            // Termino 
            device.EndScene();

            //TextureLoader.Save("shadowmap.bmp", ImageFileFormat.Bmp, g_pShadowMap);

            // restuaro el render target y el stencil
            device.DepthStencilSurface = pOldDS;
            device.SetRenderTarget(0, pOldRT);
            device.Transform.Projection = OldProjection;

        }

        public void RenderScene(bool shadow)
        {
            string technique;


            if (shadow) technique = "SHADOW_MAP";
            else technique = "SHADOWS";
                
            
            foreach (TerrainPatch p in this.patches)
            {
                p.Effect = effect;
                p.Technique = technique;
                p.render();
            }

            foreach (ILevelObject o in this.objects)
            {
                o.Effect = effect;
                o.Technique = technique;
                o.render();
            }
            
            foreach (Character c in this.characters)
            {
                c.Effect = effect;
                c.Technique = technique;
                c.render();
            }

           



        }


        public override void dispose()
        {

            
            g_pShadowMap.Dispose();
            g_pDSShadow.Dispose();
            
        }

    }
}
