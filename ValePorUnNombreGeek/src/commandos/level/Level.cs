﻿using System.Collections.Generic;
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
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level
{
    class Level
    {
        List<Character> characters;
        List<Enemy> enemies;
        List<Commando> commandos;
        List<ILevelObject> objects;
        ITerrain terrain;
        IQuadTree quadtree;
        public IRenderer Renderer { get { return this.quadtree.Renderer; } set { this.quadtree.Renderer = value; } }
        LevelMap map;


        public List<Character> Characters { get { return this.characters; } }
        public List<Enemy> Enemies { get { return this.enemies; } }
        public List<Commando> Commandos { get { return this.commandos; } }
        public List<ILevelObject> Objects { get { return this.objects; } }

       
        public ITerrain Terrain{
            get{ return this.terrain; }
        }

        public LevelMap Map
        {
            get { return this.map; }
        }
        
        public Level(ITerrain terrain)
        {
            characters = new List<Character>();
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            this.terrain = terrain;
            this.map = new LevelMap(this, 100,100,2);
            quadtree = new QuadTreeDummie(terrain, new DefaultRenderer());
        }

       
        public void add(Commando commando)
        {
            addCharacter(commando);
            commandos.Add(commando);
            if (commandos.Count > 1)
            {   Commando last = commandos[commandos.IndexOf(commando) - 1];
                commando.Life.Position = last.Life.Position + new Vector2(last.Life.Width+10, 0);
            }else commando.Life.Position = new Vector2(60, 10);
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
            
            foreach (Commando c in this.commandos)    
                c.Life.render();
            

        }


        public void dispose()
        {
            
            foreach (Character ch in this.characters)
            
                ch.dispose();
            

            foreach (ILevelObject o in this.objects)
            
                o.dispose();

            map.dispose();
            terrain.dispose();
            quadtree.dispose();
        }



        private Vector3[] getAlternativeMovements(Vector3 direction)
        {
            Vector3[] alternatives = new Vector3[2];
            alternatives[0] = Vector3.TransformCoordinate(direction, Matrix.RotationY(FastMath.PI_HALF));
            alternatives[1] = Vector3.TransformCoordinate(direction, Matrix.RotationY(-FastMath.PI_HALF));
            return alternatives;

        }


       
        public bool thereIsCollision(Character ch, out ILevelObject obj, out Vector3 n)
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
