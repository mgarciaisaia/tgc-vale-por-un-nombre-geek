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
        //ITerrain terrain;
        List<QTSector> sectors;

        public IRenderer Renderer { get; set; }


        public QuadTree(ITerrain terrain, IRenderer renderer)
        {
            //this.terrain = terrain;
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
            List<ILevelObject> objetosARenderizar = new List<ILevelObject>();
            //List<Character> personajesARenderizar = new List<Character>();

            /*float terrainHeight = this.terrain.Height;
            float terrainWidth = this.terrain.Width;
            Vector3 terrainPos = terrain.Position - new Vector3(terrainHeight / 2, 0, terrainWidth / 2);*/

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();

            foreach (QTSector sector in this.sectors)
                if (sector.collidesWithFrustum(frustum))
                {
                    objetosARenderizar.AddRange(sector.Objects);
                    this.Renderer.render(sector.TerrainPatch);
                }

            //foreach (ILevelObject asd in this.objects)
                //GuiController.Instance.UserVars.setValue("obj " + asd.GetHashCode().ToString(), false);
            //foreach (Character asd in this.characters)
                //GuiController.Instance.UserVars.setValue("ch " + asd.GetHashCode().ToString(), false);

            foreach (ILevelObject obj in objetosARenderizar)
            {
                this.Renderer.render(obj);
                //GuiController.Instance.UserVars.setValue("obj " + obj.GetHashCode().ToString(), true);
            }
            foreach (Character ch in this.characters)//personajesARenderizar)
            {
                this.Renderer.render(ch);
                //GuiController.Instance.UserVars.setValue("ch " + ch.GetHashCode().ToString(), true);
            }

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
