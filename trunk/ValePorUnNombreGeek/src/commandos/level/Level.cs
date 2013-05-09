using System.Collections.Generic;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.optimization;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map;

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
        LevelMap map;


        public List<Character> Characters { get { return this.characters; } }
        public List<Enemy> Enemies { get { return this.enemies; } }
        public List<Commando> Commandos { get { return this.commandos; } }
        public List<ILevelObject> Objects { get { return this.objects; } }

       
        public Terrain Terrain{
            get{return this.terrain;}
        }

        public LevelMap Map
        {
            get { return this.map; }
        }
         public Level(Terrain terrain)
        {
            characters = new List<Character>();
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            this.terrain = terrain;
            this.map = new LevelMap(this, 100,100,2);
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
            
            

            //Cuando se pueda hacer que no se traben, se quita character.OwnedByUser
            if (character.OwnedByUser && terrenoMuyEmpinado(previousPosition, direction*speed))
            {

                /*//Busco movimientos alternativos
                foreach (Vector3 alt in getAlternativeMovements(direction))
                {

                    if (!terrenoMuyEmpinado(previousPosition, alt*speed))
                    {
                        realMovement = alt;
                        break;
                    }
                }
               
               */

               if (realMovement == direction)
               {
                   character.manageSteepTerrain();
                   return;
               }
              
               

            }

            //Muevo el personaje
            character.move(realMovement, speed);
            character.Position = this.getPosition(character.Position.X, character.Position.Z);

            Vector3 n;
               
            int intentos;
            int maxIntentos = 50;
            Vector3 centrifugal = Vector3.Empty;
            for (intentos = 0; thereIsCollision(character, out obj, out n); intentos++)
            {
                
                //renderVector(character, n, Color.Black);
                
                //Cancelo el movimiento
                character.Position = previousPosition;
                if (intentos == maxIntentos) break;

                //Si el pj ya arregló el problema, parar.             
                if (character.manageCollision(obj)) break;

                //Calculo un vec que se aleja del centro del objeto para que el pj gire alrededor.                        
                centrifugal = (character.Center - obj.Center);
                

                centrifugal.Y = 0;
                centrifugal.Normalize();
                
                realMovement = centrifugal + realMovement;  //Voy haciendo que la direccion tienda mas hacia la centrifuga.
                realMovement.Normalize();

                character.move(realMovement, speed);
                character.Position = this.getPosition(character.Position.X, character.Position.Z);

            }
           // renderVector(character, centrifugal, Color.Red);
           // renderVector(character, realMovement, Color.Green);

         

          

           
        }

        private Vector3[] getAlternativeMovements(Vector3 direction)
        {
            Vector3[] alternatives = new Vector3[2];
            alternatives[0] = Vector3.TransformCoordinate(direction, Matrix.RotationY(FastMath.PI_HALF));
            alternatives[1] = Vector3.TransformCoordinate(direction, Matrix.RotationY(-FastMath.PI_HALF));
            return alternatives;

        }

        private void renderVector(Character character, Vector3 n, Color color)
        {
            if (n.Equals(Vector3.Empty)) return;
            TgcArrow arrow = new TgcArrow(); //flechita demostrativa de que funciona xd
            arrow.Enabled = true;
            arrow.PStart = character.Center;
            arrow.PEnd = n * 100 + arrow.PStart;
            arrow.Thickness = 5;
            arrow.HeadSize = new Vector2(10, 10);
            arrow.BodyColor = color;
            arrow.updateValues();
            arrow.render();
           
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

       
        private bool thereIsCollision(Character ch, out ILevelObject obj, out Vector3 n)
        {
            obj = null;

            foreach (ILevelObject colisionable in this.getPosibleColliders(ch))
                if (colisionable.collidesWith(ch, out n)){
                    obj = colisionable;
                    return true;
                }

            n = Vector3.Empty;
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
