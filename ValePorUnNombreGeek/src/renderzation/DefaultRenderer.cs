using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Shaders;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    class DefaultRenderer:IRenderer
    {
        //protected List<Enemy> enemies;
        //protected List<Commando> commandos;
        //protected List<ILevelObject> objects;
        //protected List<TerrainPatch> terrainPatches;
        protected Effect effect;

        public DefaultRenderer()
        {
            effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "shaders.fx");
        }

        //public virtual void beginRender()
        //{
        //    enemies = new List<Enemy>();
        //    commandos = new List<Commando>();
        //    objects = new List<ILevelObject>();
        //    terrainPatches = new List<TerrainPatch>();

        //}

        //public virtual void render(ILevelObject o)
        //{
        //    objects.Add(o);
        //    o.Effect = effect;
        //}

        //public virtual void render(Commando c)
        //{
        //    commandos.Add(c);
        //    c.Effect = effect;
        //}

        //public virtual void render(Enemy e)
        //{
        //    enemies.Add(e);
        //    e.Effect = effect;
        //}

        //public virtual void render(TerrainPatch t)
        //{
        //    terrainPatches.Add(t);
        //    t.Effect = GuiController.Instance.Shaders.VariosShader;
        //}

        public override void render()
        {
            string technique;


            technique = "DIFFUSE_MAP";


            foreach (TerrainPatch p in this.patches)
            {
                p.Effect = GuiController.Instance.Shaders.VariosShader;
                p.Technique = TgcShaders.T_POSITION_TEXTURED;
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

        }
    }
}
