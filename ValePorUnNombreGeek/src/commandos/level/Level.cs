using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
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
        /// Retorna false si el personaje no puede realizar el movimiento.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public void moveCharacter(Character character, Vector3 direction, float speed){

            Vector3 previousPosition = character.Position;
            Vector3 realMovement = direction;
            ILevelObject obj;
            
            //Muevo el personaje
            character.move(direction, speed);
            character.Position = this.getPosition(character.Position.X, character.Position.Z);

            
            while(thereIsCollision(character, out obj)){

                //Cancelo el movimiento
                character.Position = previousPosition;
                
                //Si el pj ya arregló el problema, parar.             
                if (character.manageCollision(obj)) break;

                //Hace que el pj se desvie                              
                Vector3 reaction = (character.Center - obj.Center);
                reaction.Y = 0;
                reaction.Normalize();
                realMovement = reaction + realMovement*0.4f;
                
                character.move(realMovement, speed);
                character.Position = this.getPosition(character.Position.X, character.Position.Z);

              
            }

           //Cuando se pueda hacer que no se traben, se quita character.OwnedByUser
           if (character.OwnedByUser && terrenoMuyEmpinado(previousPosition, direction)){
                
                character.Position = previousPosition;
                if (!character.manageSteepTarrain())
                {
                    //Do something
                }
                
           }

          

           
        }

        const float MAX_DELTA_Y = 20f;

        private bool terrenoMuyEmpinado(Vector3 origin, Vector3 direction)
        {
           
                              
            Vector3 deltaXZ = direction*5;
           
            Vector3 target = new Vector3(origin.X, 0, origin.Z);
            target.Add(deltaXZ);
            float targetDeltaY = this.getPosition(target.X, target.Z).Y - origin.Y;
            
            
            //if(targetDeltaY > MAX_DELTA_Y)GuiController.Instance.Logger.log("Pendiente: " + origin.Y + " -> " + (origin.Y + targetDeltaY) + " = " + targetDeltaY );
            
            return targetDeltaY > MAX_DELTA_Y;
        }

       
        private bool thereIsCollision(ILevelObject collider, out ILevelObject obj)
        {
            TgcCollisionUtils.BoxBoxResult result;
            obj = null;

            foreach (ILevelObject colisionable in this.getPosibleColliders(collider))
            {
               
              result = TgcCollisionUtils.classifyBoxBox(collider.BoundingBox, colisionable.BoundingBox);


              if (result != TgcCollisionUtils.BoxBoxResult.Afuera)
              {
                  obj = colisionable;
                  return true;
              }
              
            }

            return false;
        }


        private IEnumerable<ILevelObject> getPosibleColliders(ILevelObject collider)
        {
            List<ILevelObject> collisionables = new List<ILevelObject>();
          
                        
            //Solo agrego a colisionables aquellos cuyas esferas chocan con el collider
            foreach (Character c in this.Characters)
            
                if(thereIsSphereCollision(collider, c)) collisionables.Add(c);


            foreach (ILevelObject o in this.Objects)

                if (thereIsSphereCollision(collider, o)) collisionables.Add(o);
               
                        

            return collisionables;

        }


        private bool thereIsSphereCollision(ILevelObject collider, ILevelObject o)
        {
            if (o == collider) return false;
           
            //Ver Unidad 6, pagina 13
            Vector3 distance = collider.Center - o.Center;

            float radiusSum = collider.Radius + o.Radius;

            return distance.LengthSq() <= radiusSum * radiusSum;
            
        }

    }
}
