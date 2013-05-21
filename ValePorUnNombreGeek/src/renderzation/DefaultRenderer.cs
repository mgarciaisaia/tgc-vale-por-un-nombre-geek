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
        List<Enemy> enemies;
        List<Commando> commandos;
        List<ILevelObject> objects;
        List<TerrainPatch> terrainPatches;
       
        public void beginRender()
        {
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            terrainPatches = new List<TerrainPatch>();
        }

        public void render(commandos.objects.ILevelObject o)
        {
            objects.Add(o);
        }

        public void render(commandos.character.Commando c)
        {
            commandos.Add(c);
        }

        public void render(commandos.character.Enemy e)
        {
            enemies.Add(e);
        }

        public void render(commandos.terrain.divisibleTerrain.TerrainPatch t)
        {
            terrainPatches.Add(t);
        }

        public void endRender()
        {
            foreach (TerrainPatch p in terrainPatches) p.render();
            foreach (ILevelObject o in objects) o.render();
            foreach (Commando c in commandos) c.render();
            foreach (Enemy e in enemies) e.render();
        }
    }
}
