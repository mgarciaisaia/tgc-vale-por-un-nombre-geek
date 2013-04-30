using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.levelObject;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTreeDummie : AlumnoEjemplos.ValePorUnNombreGeek.src.optimization.IQuadTree
    {
        List<ILevelObject> objects;
        Terrain terrain;

       
        public QuadTreeDummie(Terrain terrain)
        {
            this.terrain = terrain;
            this.objects = new List<ILevelObject>();
        }

        public void add(ILevelObject obstacle)
        {
            objects.Add(obstacle);
        }

        public void render(TgcFrustum frustum, List<Commando> commandos, List<Enemy> enemies)
        {
            terrain.render();
            foreach (ILevelObject o in objects) o.render();
            foreach(Commando c in commandos) c.render();
            foreach (Enemy e in enemies) e.render();
        }


    }
}
