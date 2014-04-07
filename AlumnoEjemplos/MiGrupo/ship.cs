using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;

/* Clase abstracta que contiene comportamiento en comun de los barcos */
namespace AlumnoEjemplos.TheGroup{

    abstract public class Ship{

        public TgcMesh ship;
        //public TgcScene shipScene2;     // Escena utilizada como prueba para testear ships.

        public void Load()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene shipScene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Ships\\Others\\BoatFishing-TgcScene.xml");
            ship = shipScene.Meshes[0];
            // Acomoda al ship dependiendo una posicion específica de spawn
            ship.Position = this.Spawn();

            //shipScene2 = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Scenes\\Ships\\Ship 03\\Ship03Scene.xml");

        }

        public Vector3 Position()
        {
            return (ship.Position);
        }

        

        public void Render()
        {
            /*
            *          ZONA DE RENDERIZADO
            */
            ship.render();
            //shipScene2.renderAll();
        }

        public void Close()
        {
            ship.dispose();
        }

        abstract public Vector3 Spawn();

    }


    /* MainShip es la subclase encargada de definir el comportamiento del barco controlado por el usuario */
    public class MainShip : Ship
    {
        /* Define el movimiento del barco controlado por el usuario */
        public void CalculateMovement(float elapsedTime)
        {
            #region MAINSHIP_MOVEMENT
            /*
             *          ZONA DE CALCULO
             */

            // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
            //Calcular proxima posicion de la nave segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = 2;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = -2;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = -150;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = 150;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar la nave y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                ship.rotateY((-1) * rotAngle);
                //GuiController.Instance.RotCamera.rotateY(rotAngle);
            }

            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Cos(ship.Rotation.Y) * moveForward,
                    jump,
                    -FastMath.Sin(ship.Rotation.Y) * moveForward
                    );
            }
            ship.move(movementVector);

            #endregion
        }

        /* Define la posicion en la que spawneara la nave. Como es la controlada por el usuario, arranca en (0,0,0) */
        public override Vector3 Spawn(){
            return Commons.Vector30;
        }
    }


    /* EnemyFleet es la subclase encargada de definir el comportamiento de los barcos enemigos.
     * Puede handlear un numero n de enemigos.
     */
    public class EnemyFleet : Ship
    {
        #region ENEMY_HANDLER
        public static List<EnemyFleet> Enemies = new List<EnemyFleet>();

        /* Crea un nuevo enemigo */
        public static void AddEnemy(){
            EnemyFleet NewEnemy = new EnemyFleet();
            NewEnemy.Load();
            Enemies.Add(NewEnemy);
        }

        public static void CalculateEveryMovement(float elapsedTime)
        {
            foreach (EnemyFleet Enemy in Enemies) Enemy.CalculateMovement(elapsedTime);
        }

        /* Renderiza todos los enemigos */
        public static void RenderAll(){
            foreach (EnemyFleet Enemy in Enemies) Enemy.Render();
        }

        /* Cierra todas las instancias de enemigos */
        public static void CloseAll()
        {
            foreach (EnemyFleet Enemy in Enemies) Enemy.Close();
        }

        #endregion

        /* TODO: ESTA ES LA ZONA EN LA QUE HAY QUE DEFINIR LA INTELIGENCIA ARTIFICIAL DE CADA ENEMIGO */
        public void CalculateMovement(float elapsedTime)
        {
            #region ENEMYSHIP_MOVEMENT
            //Aplicar movimiento, se desplaza sobre el eje x.
            Vector3 movementVector;
            movementVector = new Vector3(10,0,0);
            ship.move(movementVector);
            #endregion
        }

        public override Vector3 Spawn()
        {
            return (new Vector3(1000, 0, 1000));
        }
    }

}
