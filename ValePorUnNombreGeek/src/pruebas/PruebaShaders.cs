﻿using System;
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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using System.Windows.Forms;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;

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
        Terrain terrain;
        float time=0;

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

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            terrain = new Terrain();
            skeletal = new SkeletalRepresentation(terrain.getPosition(477, 129));
            
            effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Shaders\\shaders.fx");
            skeletal.Effect = effect;
            terrain.Effect = effect;
            GuiController.Instance.Modifiers.addFloat("timeSpeed", 0.01f, 0.5f, 0.25f);

            FreeCamera camera = new FreeCamera();
            camera.Enable = true;
            //initShadows(d3dDevice);

        }

        private void initShadows(Device d3dDevice)
        {


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
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                aspectRatio, near_plane, far_plane);

            float K = 300;
            GuiController.Instance.Modifiers.addVertex3f("LightLookFrom", new Vector3(-K, -K, -K), new Vector3(K, K, K), new Vector3(80, 120, 0));
            GuiController.Instance.Modifiers.addVertex3f("LightLookAt", new Vector3(-K, -K, -K), new Vector3(K, K, K), new Vector3(0, 0, 0));
        }


        public override void render(float elapsedTime)
        {

           
            renderNight(elapsedTime);
            //renderShadows();

       }

        private void renderNight(float elapsedTime)
        {
            time += (float)GuiController.Instance.Modifiers.getValue("timeSpeed")* elapsedTime;
            effect.SetValue("daytime", FastMath.Abs(FastMath.Cos(time)));
           
            skeletal.Technique = "SKELETAL_NIGHT";

            terrain.Technique = "NIGHT";
            
            skeletal.render();

            terrain.render();
        }

        private void renderShadows()
        {
            Device device = GuiController.Instance.D3dDevice;
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;

            g_LightPos = (Vector3)GuiController.Instance.Modifiers["LightLookFrom"];
            g_LightDir = (Vector3)GuiController.Instance.Modifiers["LightLookAt"] - g_LightPos;
            g_LightDir.Normalize();

            // Shadow maps:
            device.EndScene();      // termino el thread anterior

            GuiController.Instance.RotCamera.CameraCenter = new Vector3(0, 0, 0);
            GuiController.Instance.RotCamera.CameraDistance = 100;
            GuiController.Instance.RotCamera.RotationSpeed = 2f;
            GuiController.Instance.CurrentCamera.updateCamera();
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //Genero el shadow map
            RenderShadowMap();

            device.BeginScene();
            // dibujo la escena pp dicha
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            RenderScene(false);
        }



        public void RenderShadowMap()
        {
            Device device = GuiController.Instance.D3dDevice;

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

        }

        public void RenderScene(bool shadow)
        {
          
          
            if (shadow)
            {

                skeletal.Technique = "SKELETAL_SHADOW_MAP";
                

            }
            else
            {
                skeletal.Technique = "SKELETAL_DIFFUSE_MAP_SHADOWS";
            }
            
            skeletal.render();

            terrain.render();
        }


        public override void close()
        {
            
            skeletal.dispose();
            terrain.render();
            g_pShadowMap.Dispose();
            g_pDSShadow.Dispose();
            effect.Dispose();

        }

    }
}

