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
        ITerrain terrain;
        public IRenderer Renderer { get; set; } 

       
        public QuadTreeDummie(ITerrain terrain, IRenderer renderer)
        {
            this.terrain = terrain;
            this.objects = new List<ILevelObject>();
            this.Renderer = renderer;
        }

        public void add(ILevelObject obstacle)
        {
            objects.Add(obstacle);
        }

        public void render(TgcFrustum frustum, List<Commando> commandos, List<Enemy> enemies)
        {

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();

            foreach (Enemy e in enemies) this.Renderer.render(e);
            foreach(TerrainPatch p in terrain.Patches) this.Renderer.render(p);
            foreach (ILevelObject o in objects) this.Renderer.render(o);
            foreach (Commando c in commandos) this.Renderer.render(c);

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
