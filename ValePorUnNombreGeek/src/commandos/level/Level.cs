using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.levelObject;
using AlumnoEjemplos.ValePorUnNombreGeek.src.optimization;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level
{
    class Level
    {
        List<Character> characters;
        List<Enemy> enemies;
        List<Commando> commandos;
        List<ILevelObject> objects;
        Terrain terrain;
        IQuadTree quadtree;


        public List<Character> Characters { get { return this.characters; } }
        public List<Enemy> Enemies { get { return this.enemies; } }
        public List<Commando> Commandos { get { return this.commandos; } }
        public List<ILevelObject> Objects { get { return this.objects; } }

       
        public Terrain Terrain{
            get{return this.terrain;}
        }

         public Level(Terrain terrain)
        {
            characters = new List<Character>();
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            this.terrain = terrain;
            quadtree = new QuadTreeDummie(terrain);           
            
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

     
        public void add(ILevelObject levelObject)
        {
            objects.Add(levelObject);
            quadtree.add(levelObject);
        }

        private void addCharacter(Character c)
        {
            characters.Add(c);
            c.setLevel(this);
        }


        public void render(float elapsedTime)
        {
                           
            foreach (Character character in this.characters) 
                
                character.update(elapsedTime);
                
         
            quadtree.render(GuiController.Instance.Frustum, commandos, enemies);

        }


        public void dispose()
        {
            
            foreach (Character ch in this.characters)
            
                ch.dispose();
            

            foreach (ILevelObject o in this.objects)
            
                o.dispose();
            

            terrain.dispose();
        }



        public Vector3 getPosition(float x, float z)
        {
           return this.terrain.getPosition(x, z);
        }

        /// <summary>
        /// Retorna false si el personaje no se puede mover.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public bool moveCharacter(Character character, Vector3 movementVector){

            Vector3 previousPosition = character.Position;
            character.move(movementVector);
            character.Position = this.getPosition(character.Position.X, character.Position.Z);

            
            if (this.positionAllowed(character) /*&& !terrenoMuyEmpinado(previousPosition, character.Position)*/)
            {
                return true;
            }
            else
            {
                character.Position = previousPosition;
                return false;
            }

           
        }

        private bool positionAllowed(Character character)
        {   
           return !thereIsCollision(character);
        }

        private bool thereIsCollision(ILevelObject collider)
        {
            TgcCollisionUtils.BoxBoxResult result;

            foreach (ILevelObject colisionable in this.getPosibleColiders(collider))
            {
                if (colisionable != collider)
                {
                    result = TgcCollisionUtils.classifyBoxBox(collider.BoundingBox, colisionable.BoundingBox);

                    if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)

                        return true;

                }
            }

            return false;
        }

        private IEnumerable<ILevelObject> getPosibleColiders(ILevelObject collider)
        {
            List<ILevelObject> colisionables = new List<ILevelObject>();
            colisionables.AddRange(characters);
            colisionables.AddRange(objects);
            return colisionables;
        }
      
    }
}
