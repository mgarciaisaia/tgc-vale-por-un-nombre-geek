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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.renderization
{
    class DefaultRenderer:Renderer
    {
        protected Effect effect;

        public DefaultRenderer()
        {
            effect = TgcShaders.loadEffect(CommandosUI.Instance.ShadersDir + "shaders.fx");
        }

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

            List<Character> enemies = new List<Character>();
            List<Character> commandos = new List<Character>();
            foreach (Character ch in this.characters)
                if (ch is Enemy)
                    enemies.Add(ch);
                else
                    commandos.Add(ch);

            foreach (Character ch in commandos)
            {
                ch.Effect = effect;
                ch.Technique = technique;
                ch.render();
            }

            foreach (Character ch in enemies)
            {
                ch.Effect = effect;
                ch.Technique = technique;
                ch.render();
            }

        }

        public override void dispose()
        {
            //...
        }
    }
}
