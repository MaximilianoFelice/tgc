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
namespace AlumnoEjemplos.SeaSharp{

    abstract public class Ship{

        public TgcScene ship;

        public void Load()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            ship = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Ships\\Ship 09\\Boat11-TgcScene.xml");

            // Acomoda al ship dependiendo una posicion específica de spawn
            ship.Position(this.Spawn());
        }

        public Vector3 Position
        {
            get { return (ship.Meshes[0].Position); }
            set { ship.Position(value); }
        }


        public void Render()
        {
            /*
            *          ZONA DE RENDERIZADO
            */

            ship.renderAll();
        }

        public void Close()
        {
            ship.disposeAll();
        }

        abstract public Vector3 Spawn();

        public void Fire()
        {
            Bola NewFireBall = new Bola();
            NewFireBall.Fire();
            NewFireBall.Position = this.Position;
            NewFireBall.Angle = ship.RotationY();

        }

    }


    /* MainShip es la subclase encargada de definir el comportamiento del barco controlado por el usuario */
    public class MainShip : Ship
    {
        /* Define el movimiento del barco controlado por el usuario */
        public void CalculateMovement(float elapsedTime)
        {
            #region MAINSHIP_MOVEMENT

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


            //Chequeo si aprete espacio
            if (d3dInput.keyPressed(Key.Space))
            {
                this.Fire();

            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar la nave y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                ship.RotateY((-1) * rotAngle);
                //GuiController.Instance.RotCamera.rotateY(rotAngle);
            }

            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Cos(ship.RotationY()) * moveForward,
                    jump,
                    -FastMath.Sin(ship.RotationY()) * moveForward
                    );
            }
            ship.Move(movementVector);

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
            movementVector = new Vector3(1,0,0);
            ship.Move(movementVector);
            #endregion
        }

        public override Vector3 Spawn()
        {
            return (new Vector3(100, 0, 100));
        }
    }

}
