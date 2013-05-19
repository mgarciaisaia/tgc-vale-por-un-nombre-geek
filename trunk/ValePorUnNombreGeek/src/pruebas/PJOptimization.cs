using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using Microsoft.DirectX.DirectInput;

namespace Examples
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class PJOptimization : TgcExample
    {
        TgcSkeletalMesh mesh;
        bool optimizado = false;


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Ejemplo de optimizacion de personaje";
        }

        public override string getDescription()
        {
            return "apretar O para optimizar (only once)";
        }

        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            this.mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "CS_Arctic-TgcSkeletalMesh.xml",
                getAnimations());

            this.mesh.playAnimation("StandBy", true);
            new FreeCamera(new Vector3(0, 0, 0), true);
        }
        protected virtual string[] getAnimations()
        {
            String exMediaDir = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\";
            return new string[] { 
                    exMediaDir + "Walk-TgcSkeletalAnim.xml",
                    exMediaDir + "Talk-TgcSkeletalAnim.xml",
                    exMediaDir + "StandBy-TgcSkeletalAnim.xml",
                    exMediaDir + "Jump-TgcSkeletalAnim.xml",
                    exMediaDir + "CrouchWalk-TgcSkeletalAnim.xml"
                };
        }


        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            if (!this.optimizado && GuiController.Instance.D3dInput.keyPressed(Key.O))
            {
                int[] adjac = new int[this.mesh.D3dMesh.NumberFaces * 3];
                this.mesh.D3dMesh.GenerateAdjacency(0.05f, adjac);
                this.mesh.D3dMesh.OptimizeInPlace(MeshFlags.OptimizeCompact | MeshFlags.OptimizeVertexCache | MeshFlags.VbShare | MeshFlags.OptimizeAttributeSort, adjac);
                /*
                 En INit de tgcskeletalMesh
                 Device device = GuiController.Instance.D3dDevice;
                //GraphicsStream adjacency ;

                int[] adjac = new int[mesh.NumberFaces * 3];
                string errors ="";

                Mesh tempMesh = Mesh.Clean(CleanType.Simplification, mesh, adjac,out adjac, out errors);
                SimplificationMesh simplifiedMesh = new SimplificationMesh(mesh, adjac);

                simplifiedMesh.ReduceVertices(mesh.NumberVertices - 25);

                mesh.Dispose();
                mesh = simplifiedMesh.Clone(simplifiedMesh.Options.Value,
                                            simplifiedMesh.VertexFormat, device);
                simplifiedMesh.Dispose();
                */
                
                GuiController.Instance.Logger.log("Optimizado!");
                this.optimizado = true;
            }

            this.mesh.render();
        }

        public override void close()
        {
            this.mesh.dispose();
        }

    }
}
