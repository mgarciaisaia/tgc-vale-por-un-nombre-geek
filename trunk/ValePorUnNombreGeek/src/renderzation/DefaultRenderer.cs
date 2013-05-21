using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    class DefaultRenderer:IRenderer
    {
        protected List<Enemy> enemies;
        protected List<Commando> commandos;
        protected List<ILevelObject> objects;
        protected List<TerrainPatch> terrainPatches;
       
        public virtual void beginRender()
        {
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            terrainPatches = new List<TerrainPatch>();
        }

        public virtual void render(commandos.objects.ILevelObject o)
        {
            objects.Add(o);
        }

        public virtual void render(commandos.character.Commando c)
        {
            commandos.Add(c);
        }

        public virtual void render(commandos.character.Enemy e)
        {
            enemies.Add(e);
        }

        public virtual void render(commandos.terrain.divisibleTerrain.TerrainPatch t)
        {
            terrainPatches.Add(t);
        }

        public virtual void endRender()
        {
            foreach (TerrainPatch p in terrainPatches) p.render();
            foreach (ILevelObject o in objects) o.render();
            foreach (Commando c in commandos) c.render();
            foreach (Enemy e in enemies) e.render();
        }

        public virtual void dispose()
        {

        }
    }
}
