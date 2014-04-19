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
            // Boat11 - Le sacamos el mesh de agua
            //ship.Meshes[1].move(0, 0, 1000);
            ship.Meshes[0].rotateY(Geometry.DegreeToRadian(90));
            
            // Acomoda al ship dependiendo una posicion específica de spawn
            ship.Position(this.Spawn());
        }

        public Vector3 Position()
        {
            return (ship.Meshes[0].Position);            
        }

        public TgcBoundingBox BoundingBox()
        {
            return ship.BoundingBox;
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

    }


    /* MainShip es la subclase encargada de definir el comportamiento del barco controlado por el usuario */
    public class MainShip : Ship
    {
        static float lastMoveForward = 0;
        static float lastRotate = 0;
        /* Define el movimiento del barco controlado por el usuario */
        public void CalculateMovement(float elapsedTime)
        {
            #region MAINSHIP_MOVEMENT
            /*
             *          ZONA DE CALCULO
             */

            // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
            //Calcular proxima posicion de la nave segun Input
            const float FORWARD = 3;
            const float ROTATE = 50;
            const float NITRO = 2.5f;
            const float DESROTATION = 0.97f;
            const float DESFORWARD = 0.99f;
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;
            float speedForward = 60f;
            float speedRotate = 0.5f;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                lastMoveForward = FORWARD;
                moveForward = FORWARD;                
                moving = true;
            }
            else      
            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                lastMoveForward = -FORWARD;
                moveForward = -FORWARD;
                moving = true;
            } 
           
            // Nitro
            if (d3dInput.keyDown(Key.LeftShift))
            {
                speedForward *= NITRO;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                lastRotate = -ROTATE;
                rotate = -ROTATE;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                lastRotate = ROTATE;
                rotate = ROTATE;
                rotating = true;
            }

            //Si no hubo rotacion, voy desacelerando la ultima rotacion
            if (!rotating)
            {
                lastRotate = lastRotate * DESROTATION;
                rotate = lastRotate;
            }

            //Rotar la nave y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
            float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime * speedRotate);
            ship.RotateY((-1) * rotAngle);
            GuiController.Instance.ThirdPersonCamera.rotateY(-rotAngle);
            //GuiController.Instance.RotCamera.rotateY(rotAngle);

            // Manejamos la camara con las flechitas
            if(d3dInput.keyDown(Key.Left))
                GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(ROTATE * elapsedTime));
            else if (d3dInput.keyDown(Key.Right))
                GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(-ROTATE * elapsedTime));
            if (d3dInput.keyDown(Key.Up))
                GuiController.Instance.ThirdPersonCamera.OffsetHeight += FORWARD * elapsedTime * 10;
            else if (d3dInput.keyDown(Key.Down))
                GuiController.Instance.ThirdPersonCamera.OffsetHeight -= FORWARD * elapsedTime * 10;
           
            //Vector de movimiento del barco
            Vector3 movementVector = Vector3.Empty;

            // Si no se movio el barco, lo voy desacelerando desde el ultimo movimiento
            if (!moving)
            {
                lastMoveForward = lastMoveForward * DESFORWARD;
                moveForward = lastMoveForward;
            }

            //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
            movementVector = new Vector3(
                FastMath.Cos(ship.RotationY()) * moveForward * elapsedTime * speedForward,
                jump,                    
                -FastMath.Sin(ship.RotationY()) * moveForward * elapsedTime * speedForward
                );
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
        static float directionX = 1;
        static float rotationY = 1;
        const int DISTANCE = 500;
        const int MAXROTATION = 180;
        const int XDISTANCETOMAINSHIP = 300;
        const int ZDISTANCETOMAINSHIP = 300;
        const float SPEEDROTATION = 20;

        #region ENEMY_HANDLER
        public static List<EnemyFleet> Enemies = new List<EnemyFleet>();

        /* Crea un nuevo enemigo */
        public static void AddEnemy(){
            EnemyFleet NewEnemy = new EnemyFleet();
            NewEnemy.Load();
            Enemies.Add(NewEnemy);
        }

        public static void CalculateEveryMovement(float elapsedTime, Ship targetShip)
        {
            foreach (EnemyFleet Enemy in Enemies) Enemy.CalculateMovement(elapsedTime, targetShip);
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
        public void CalculateMovement(float elapsedTime, Ship targetShip)
        {
            #region ENEMYSHIP_MOVEMENT
            //Aplicar movimiento, se desplaza sobre el eje x.
            Vector3 movementVector;

            Vector3 shipPosition = new Vector3();
            Vector3 targetShipPosition = new Vector3();    
            float moveForward = 3;
            float speedForward = 60f;
            float jump = 0;


            // Si el enemigo esta cerca de nuestro barco, se detiene y nos ataca
            if (Math.Abs(ship.Meshes[0].Position.X - targetShip.ship.Meshes[0].Position.X) < XDISTANCETOMAINSHIP
                && Math.Abs(ship.Meshes[0].Position.Z - targetShip.ship.Meshes[0].Position.Z) < ZDISTANCETOMAINSHIP)
            {                
                movementVector = new Vector3(0,0,0);
                
                // Rotacion automatica mientras esta frenado
                //ship.RotateY(Geometry.DegreeToRadian(targetShip.ship.Meshes[0].Rotation.Y * elapsedTime));
                //ship.Meshes[0].Rotation = targetShip.ship.Meshes[0].Rotation;
                
                // Copia nuestra rotacion, para simular que nos apunta
                //ship.copyShipRotationY(targetShip);

                // Atacar
            }
            else { 
            // Rota 180 y luego empieza a rotar en direccion opuesta
            if (ship.Meshes[0].Rotation.Y > Geometry.DegreeToRadian(MAXROTATION))
                rotationY = -1;
            else if (ship.Meshes[0].Rotation.Y < Geometry.DegreeToRadian(-MAXROTATION))
                rotationY = 1;
            ship.RotateY(Geometry.DegreeToRadian(SPEEDROTATION * elapsedTime * rotationY));

            // Avanza cierta distancia y luego vuelve en direccion opuesta
            if (ship.Meshes[0].Position.X > DISTANCE)
                directionX = -1;
            else if (ship.Meshes[0].Position.X < -DISTANCE)
                directionX = 1;
                
            movementVector = new Vector3(
                FastMath.Cos(ship.RotationY()) * moveForward * elapsedTime * speedForward * directionX,
                jump,                    
                -FastMath.Sin(ship.RotationY()) * moveForward * elapsedTime * speedForward * directionX
                );
          }
           
          ship.Move(movementVector);

            
            #endregion
        }

        public override Vector3 Spawn()
        {
            return (new Vector3(100, 0, 100));
        }
    }

}
