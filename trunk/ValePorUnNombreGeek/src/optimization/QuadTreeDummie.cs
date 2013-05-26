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

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTreeDummie : AlumnoEjemplos.ValePorUnNombreGeek.src.optimization.IQuadTree
    {
        List<ILevelObject> objects;
        List<Character> characters;
        ITerrain terrain;
        public IRenderer Renderer { get; set; } 

       
        public QuadTreeDummie(ITerrain terrain, IRenderer renderer)
        {
            this.terrain = terrain;
            this.objects = new List<ILevelObject>();
            this.characters = new List<Character>();
            this.Renderer = renderer;
        }

        public void add(ILevelObject obstacle)
        {
            this.objects.Add(obstacle);
        }

        public void add(Character ch)
        {
            this.characters.Add(ch);
        }

        public void render(TgcFrustum frustum)
        {

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();

            foreach(TerrainPatch p in terrain.Patches) this.Renderer.render(p);
            foreach (ILevelObject o in objects) this.Renderer.render(o);
            foreach (Character ch in characters) this.Renderer.render(ch);

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
