using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTree : AlumnoEjemplos.ValePorUnNombreGeek.src.optimization.IQuadTree
    {
        List<ILevelObject> objects;
        List<Character> characters;
        List<QTSector> sectors;

        public IRenderer Renderer { get; set; }


        public QuadTree(ITerrain terrain, IRenderer renderer)
        {
            this.objects = new List<ILevelObject>();
            this.characters = new List<Character>();
            this.Renderer = renderer;
            sectors = new List<QTSector>();
            foreach(TerrainPatch tp in terrain.Patches)
                sectors.Add(new QTSector(tp));
        }

        public void add(ILevelObject obstacle)
        {
            this.objects.Add(obstacle);
            foreach (QTSector sector in this.sectors)
                sector.addObjectIfCollides(obstacle);
            //GuiController.Instance.UserVars.addVar("obj " + obstacle.GetHashCode().ToString());
        }

        public void add(Character ch)
        {
            this.characters.Add(ch);
            //GuiController.Instance.UserVars.addVar("ch " + ch.GetHashCode().ToString());
        }

        public void render(TgcFrustum frustum)
        {
            List<ILevelObject> objectsToRender = new List<ILevelObject>();

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();
            //foreach (Character asd in this.characters)
                //GuiController.Instance.UserVars.setValue("ch " + asd.GetHashCode().ToString(), false);

            foreach (QTSector sector in this.sectors)
                if (sector.collidesWithFrustum(frustum))
                {
                    objectsToRender.AddRange(sector.Objects);
                    this.Renderer.render(sector.TerrainPatch);
                }
            foreach (Character ch in this.characters)
                if (TgcCollisionUtils.testPointFrustum(frustum, ch.Position))
                {
                    this.Renderer.render(ch);
                    //GuiController.Instance.UserVars.setValue("ch " + ch.GetHashCode().ToString(), true);
                }

            //foreach (ILevelObject asd in this.objects)
                //GuiController.Instance.UserVars.setValue("obj " + asd.GetHashCode().ToString(), false);

            foreach (ILevelObject obj in objectsToRender)
            {
                this.Renderer.render(obj);
                //GuiController.Instance.UserVars.setValue("obj " + obj.GetHashCode().ToString(), true);
            }

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
