using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class Nivel
    {
        List<Character> characters;
        List<Enemy> enemies;
        List<Commando> commandos;
        List<ITransformObject> obstacles;
        Terrain terrain;



        public List<Character> Characters { get { return this.characters; } }
        public List<Enemy> Enemies { get { return this.enemies; } }
        public List<Commando> Commandos { get { return this.commandos; } }
        public List<ITransformObject> Obstacles { get { return this.obstacles; } }

        public Terrain Terrain{
            get{return this.terrain;}
        }

         public Nivel(Terrain terrain)
        {
            characters = new List<Character>();
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            obstacles = new List<ITransformObject>();
            this.terrain = terrain;    
        }

       
        public void add(Commando commando)
        {
            addCharacter(commando);
            commandos.Add(commando);
        }

        public void add(Enemy enemy)
        {
            addCharacter(enemy);
            enemies.Add(enemy);
        }

     
        public void add(ITransformObject obstacle)
        {
            obstacles.Add(obstacle);
        }
        private void addCharacter(Character c)
        {
            characters.Add(c);
            c.Nivel = this;
        }

        //Despues esta clase se va a modificar para que renderice segun un quadtree
        public void render(float elapsedTime)
        {
            terrain.render();

            foreach (IRenderObject o in this.obstacles)
            {
                o.render();
            }

          
            foreach (Commando commando in this.commandos)
            {
                commando.render(elapsedTime);
            }

            foreach (Enemy enemy in this.enemies)
            {
                
                enemy.render(elapsedTime);
                           
            }
        }

        public void dispose()
        {
            foreach (Character ch in this.characters)
            {
                ch.dispose();
            }

            foreach (IRenderObject o in this.obstacles)
            {
                o.dispose();
            }

            terrain.dispose();
        }



        public Vector3 getPosition(float x, float z)
        {
           return this.terrain.getPosition(x, z);
        }
    }
}
