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
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;

/* Clase abstracta que contiene comportamiento en comun de los barcos */
namespace AlumnoEjemplos.SeaSharp{

    abstract public class Ship{

        public TgcScene ship;

        public LifeBar lifeBar;
        public float life = 100;

        public float nitro = 100;

        public TgcSprite targetMap;

        public bool isFiring = false;

        public TgcBoundingSphere shipSphere;
        public TgcBoundingSphere enemySphere;
        public static float time = 0f;

        public TgcArrow _Normal;

        Microsoft.DirectX.Direct3D.Effect effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");


        public void Load()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            ship = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Ships\\Ship 10\\ShipF-TgcScene.xml");
            ship.Scale (new Vector3(1, 1, 1));
            
            /* Getting a new Life Bar */
            lifeBar = new LifeBar();


            // Target Map - Icono que indica en que posicion del mapa esta el barco
            targetMap = new TgcSprite();
            targetMap.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\TargetRed.png");

            targetMap.Position = new Vector2(15, 14);
            targetMap.Scaling = new Vector2(0.1f, 0.1f);
            targetMap.Rotation = Geometry.DegreeToRadian(90);
            targetMap.RotationCenter = new Vector2(targetMap.Texture.Width * targetMap.Scaling.X / 2, targetMap.Texture.Height * targetMap.Scaling.Y / 2);

            //BoundingSphere que va a usar el ship
            shipSphere = new TgcBoundingSphere(ship.BoundingBox.calculateBoxCenter(), ship.BoundingBox.calculateBoxRadius() + 40f);

            // Acomoda al ship dependiendo una posicion específica de spawn
            ship.Position(this.Spawn());

            _Normal = new TgcArrow();
            _Normal.Thickness = 1.5f;

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
            

            /* Life Bar Rendering */
            lifeBar.Render(life);
            _Normal.render();

            //if (shipSphere != null) shipSphere.render();
            //if (enemySphere != null) enemySphere.render();
            ship.renderAll();

            if (GuiController.Instance.D3dInput.keyDown(Key.J))
            {
                TgcStaticSound sonidoBocina = new TgcStaticSound();
                sonidoBocina.loadSound(GuiController.Instance.AlumnoEjemplosMediaDir + "Sound\\horn.wav");
                sonidoBocina.play(false);
            }
            
        }


        public void Close()
        {
            ship.disposeAll();
        }

        public virtual TgcBoundingSphere BoundingSphere
        {
            get { return shipSphere; }
        }

        abstract public Vector3 Spawn();

        public void Fire()
        {
            /*
            Bola NewFireBall = new Bola();
            NewFireBall.Angle = ship.RotationY();
            NewFireBall.Fire();
            NewFireBall.Position = this.Position;
            */
            
            int cantidad = 4;
            float inversion = 0;
            for (int i = 0; i < cantidad; i++)
            {
                Bola NewFireBall = new Bola();

                // La mitad de las bolas disparan para el lado contrario
                inversion = i % 2 == 0 ? Geometry.DegreeToRadian(180) : 0f;

                NewFireBall.Angle = ship.RotationY() - (((cantidad/ 2f) - i) / 10f) - inversion;
                NewFireBall.Fire();
                NewFireBall.Position = this.Position;
                NewFireBall.Owner = this;
                TgcStaticSound sonidoExpl = new TgcStaticSound();
                sonidoExpl.loadSound(GuiController.Instance.AlumnoEjemplosMediaDir + "Sound\\boom.wav");
                sonidoExpl.play(false);
                
            }
             
        }


        protected void HundirShip()
        {
            GuiController.Instance.ThirdPersonCamera.OffsetHeight = 0;
            float j = 0.2f, y = -0.25f, k = 1, i = 0;

            foreach (var Mesh in ship.Meshes)
            {
                j -= 0.01f;
                if (i % 2 == 0)
                    k = 1;
                else
                    k = -1;

                Mesh.Position = new Vector3(Mesh.Position.X, Mesh.Position.Y + y, Mesh.Position.Z);

                Mesh.rotateX(Geometry.DegreeToRadian(j * 0.25f * k * 5.9f));
                Mesh.rotateY(Geometry.DegreeToRadian(0.1f *5.9f));
                Mesh.rotateZ(Geometry.DegreeToRadian(-j * 0.02f * k * 5.9f));
                i++;
            }
        }

        public float calculateHeight(float time, float x, float z, int aux)
        {

            float frecuencia = ConfigParam.Sea.getFrecuencia();
            float amplitud = ConfigParam.Sea.getAmplitud();


            float y = -150;

            float u = (x / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);
            float v = (z / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);

            // calculo de la onda (movimiento grande)
            //float ola = FastMath.Sin(u * frecuencia * 3.14159f * 2 + time) * FastMath.Cos(v * frecuencia * 3.14159f * 2 + time);

            float A = 10;
            float f = 120 + ((x * z) / 100000);
            float Speed = 0.5f;
            float L = 10;
            float phi = Speed * 2 * 3.14159f * 2 / L;

            // Aleatoria
            // Aleatoria

            float ola2 = FastMath.Sin(1 * u * 2 * 3.14159f * frecuencia + time) * amplitud + FastMath.Cos(3 * v * 2 * 3.14159f * frecuencia + time) * amplitud// +
                 //(A / 10) * FastMath.Sin(f * z * 4 + phi * time) * FastMath.Cos(f * x / 4 + phi * time)
                 //+ (A / 20) * FastMath.Sin(f * x / 5 + phi * time) * FastMath.Cos(f * z / 2 + phi * time)
                 //+ (A / 30) * FastMath.Sin(f * (x + 13) / 5 + phi * time) * FastMath.Cos(f * (z + 28) / 10 + phi * time)

                ;

            //y = y + ola * 150 + ola2 * 10;

            //float height = tex2Dlod(heightmap, float4(u, v, 0, 0)).r;

            //if (aux == 1) y = y + ola * amplitud;
            if (aux == -1) y = ola2;

            return y;
        }

        public void reCalculateHeight()
        {

            float yPos = calculateHeight(time, this.Position.X, this.Position.Z, -1);
            ship.Position(new Vector3(this.Position.X, yPos, this.Position.Z));


            //this.ship.RotatationZ(FastMath.Asin(Vector3.Length(Vector3.Cross(normal, new Vector3(0, 1, 0)))));

            	//VS_OUTPUT Output;
        }

        public void reCalculateNormal()
        {

            float dr = 15;
            float heightx = calculateHeight(time, this.Position.X + dr, this.Position.Z, -1);
            float heightz = calculateHeight(time, this.Position.X, this.Position.Z + dr, -1);
            Vector3 dx = Vector3.Normalize(new Vector3(dr, heightx - this.Position.Y, 0));
            Vector3 dz = Vector3.Normalize(new Vector3(0, heightz - this.Position.Y, dr));

            Vector3 normal = Vector3.Cross(dz, dx);

            //Vector3 rotatedNormal = new Vector3(FastMath.Cos(this.ship.RotationX()) * normal.X, normal.Y, FastMath.Sin(this.ship.RotationZ()) * normal.Z);
            Vector3 rotatedNormal = new Vector3(normal.X, normal.Y, normal.Z);
            // (-1) * FastMath.Cos(this.ship.RotationY()) * 
            this.ship.SetNormal(normal);

            _Normal.PStart = this.Position;
            _Normal.PEnd = this.Position + rotatedNormal * 100;
            _Normal.updateValues();

            //FPSCounters.Status.Text = "X Rot: " + this.ship.RotationX().ToString();
            //FPSCounters.Status.Text = FPSCounters.Status.Text + " - Y Rot: " + this.ship.RotationY().ToString();
            //FPSCounters.Status.Text = FPSCounters.Status.Text + " - z Rot: " + this.ship.RotationZ().ToString();
        }

    }


    /* MainShip es la subclase encargada de definir el comportamiento del barco controlado por el usuario */
    public class MainShip : Ship
    {
        static float lastMoveForward = 0;
        static float lastRotate = 0;

        public void CambiarTargetMap()
        {
            targetMap.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\TargetBlack.png");
        }

        /* Define la posicion en la que spawneara la nave. Como es la controlada por el usuario, arranca en (0,0,0) */
        public override Vector3 Spawn()
        {
            return Commons.Vector30;
        }

        /* Define el movimiento del barco controlado por el usuario */
        public void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;

            #region MAINSHIP_MOVEMENT


            // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
            //Calcular proxima posicion de la nave segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;
            float speedForward = 60f;
            float speedRotate = 0.5f;
            float maxSpeed = 0;
            float acc = 0;


            targetMap.Position = new Vector2(-ship.Meshes[0].Position.X / 50 + Environment.MapShipsOffsetX, ship.Meshes[0].Position.Z / 50 + Environment.MapShipsOffsetY);
            lifeBar.calculatePosition(this.Position, elapsedTime, d3dInput, life, nitro);

            // Hundimiento
            if (life < 1)
            {
                this.HundirShip();
            }
            else
            {
                //Adelante
                if (d3dInput.keyDown(Key.W))
                {
                    acc = ConfigParam.Ship.FORWARD - 0.3f*ship.RotationZ();
                    if (ship.RotationZ() > 0)
                    {
                        maxSpeed = 8 - 3*ship.RotationZ();
                        if (lastMoveForward > maxSpeed)
                        {
                            lastMoveForward = lastMoveForward - ship.RotationZ();
                        }
                        else
                        {
                            lastMoveForward = lastMoveForward + acc;
                        }
                        if (lastMoveForward < 0)
                        {
                            lastMoveForward = 0;
                        }
                    }
                    else
                    {
                        maxSpeed = 8 - ship.RotationZ();
                        if(lastMoveForward > maxSpeed)
                        {
                            lastMoveForward = lastMoveForward + ship.RotationZ();
                        }
                        else
                        {
                            lastMoveForward = lastMoveForward + acc;
                        }
                    }
                    //if (lastMoveForward > 7.5f)
                    //{
                    //    lastMoveForward = 8;
                    //}
                    //else
                    //{
                    //    lastMoveForward = lastMoveForward + ConfigParam.Ship.FORWARD;
                    //}
                    moveForward = lastMoveForward;
                    moving = true;
                }
                else
                    //Atras
                    if (d3dInput.keyDown(Key.S))
                    {
                        lastMoveForward = lastMoveForward * 0.9f;
                        moveForward = lastMoveForward;
                        // Cuando la desaceleracion llega a un valor muy bajo, le asignamos 0 para que no tienda a 0 infinitamente
                        if (moveForward < 1)
                        {
                            lastMoveForward = 0;
                            moveForward = 0;
                        }
                    }

                // Nitro
                if (d3dInput.keyDown(Key.LeftShift))
                {
                    if (nitro >= 0)
                    {
                        if (moving)
                        {
                            speedForward *= ConfigParam.Ship.NITRO;
                            nitro -= 0.5f;
                        }
                        else if (nitro < 100)
                            nitro += 0.05f;
                    }
                }
                else
                {
                    if (nitro < 100)
                        nitro += 0.05f;
                }

                //Derecha
                if (d3dInput.keyDown(Key.D))
                {
                    lastRotate = -ConfigParam.Ship.ROTATE;
                    rotate = -ConfigParam.Ship.ROTATE;
                    rotating = true;
                }

                //Izquierda
                if (d3dInput.keyDown(Key.A))
                {
                    lastRotate = ConfigParam.Ship.ROTATE;
                    rotate = ConfigParam.Ship.ROTATE;
                    rotating = true;
                }

                //Chequeo si aprete espacio
                if (d3dInput.keyPressed(Key.Space))
                {
                    this.Fire();
                    isFiring = true;
                }

                if (d3dInput.keyUp(Key.Space))
                {
                    isFiring = false;
                }


                //Si hubo rotacion
                if (rotating)
                {
                    lastRotate = lastRotate * ConfigParam.Ship.DESROTATION;
                    rotate = lastRotate;
                }

                //Rotar la nave y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime * speedRotate);
                ship.RotateY((-1) * rotAngle);


                // Actualizamos el icono que representa al barco en el Mapa
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();
                targetMap.Rotation += (-1) * rotAngle;
                targetMap.render();
                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();


                GuiController.Instance.ThirdPersonCamera.rotateY(-rotAngle);
                //GuiController.Instance.RotCamera.rotateY(rotAngle);

                // Manejamos la camara con las flechitas
                if (d3dInput.keyDown(Key.Left))
                    GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(ConfigParam.Ship.ROTATE * elapsedTime));
                else if (d3dInput.keyDown(Key.Right))
                    GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(-ConfigParam.Ship.ROTATE * elapsedTime));
                if (d3dInput.keyDown(Key.Up))
                    GuiController.Instance.ThirdPersonCamera.OffsetHeight += ConfigParam.Ship.FORWARD * elapsedTime * 10;
                else if (d3dInput.keyDown(Key.Down))
                    GuiController.Instance.ThirdPersonCamera.OffsetHeight -= ConfigParam.Ship.FORWARD * elapsedTime * 10;

                /*//Guardar posicion original antes de cambiarla
                Vector3 originalPos = this.Position;*/

                //Vector de movimiento del barco
                Vector3 movementVector = Vector3.Empty;

                // Si no se movio el barco, lo voy desacelerando desde el ultimo movimiento
                if (!moving)
                {
                    lastMoveForward = lastMoveForward * ConfigParam.Ship.DESFORWARD;
                    moveForward = lastMoveForward;
                    // Cuando la desaceleracion llega a un valor muy bajo, le asignamos 0 para que no tienda a 0 infinitamente
                    if (moveForward < 1)
                    {
                        lastMoveForward = 0;
                        moveForward = 0;
                    }
                }

                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Cos(ship.RotationY()) * moveForward * elapsedTime * speedForward,
                    jump,
                    -FastMath.Sin(ship.RotationY()) * moveForward * elapsedTime * speedForward
                    );

                Vector3 originalPos = this.Position;
                TgcBoundingSphere shipSphereAux = new TgcBoundingSphere(shipSphere.Center, shipSphere.Radius);
                //ship.Move(movementVector);
                //this.reCalculateHeight(elapsedTime);
                shipSphereAux.moveCenter(movementVector);

                //Chequear si el objeto principal en su nueva posición choca con alguno de los enemys.
                bool collisionFound1 = false;
                foreach (EnemyFleet enemy in EnemyFleet.Enemies)
                {
                    //Ejecutar algoritmo de detección de colisiones
                    collisionFound1 = TgcCollisionUtils.testSphereSphere(shipSphereAux, enemy.enemySphere);

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                    if (collisionFound1)
                    {
                        break;
                    }
                }

                bool collisionFound2 = false;
                foreach (Island island in Island.Islands)
                {
                    //Ejecutar algoritmo de detección de colisiones
                    collisionFound2 = TgcCollisionUtils.testSphereSphere(shipSphereAux, island.islandSphere);

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                    if (collisionFound2)
                    {
                        break;
                    }
                }

                bool collisionFound3 = false;
                foreach (TgcBox box in Environment.obstaculos)
                {
                    //Ejecutar algoritmo de detección de colisiones
                    collisionFound3 = TgcCollisionUtils.testSphereAABB(shipSphereAux, box.BoundingBox);

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                    if (collisionFound3)
                    {
                        break;
                    }
                }


                //Si hubo alguna colisión, entonces restaurar la posición original del mesh (el bounding sphere original no lo movimos)
                if (collisionFound1 || collisionFound2 || collisionFound3)
                {
                    this.Position = originalPos;
                    this.reCalculateHeight();
                    this.reCalculateNormal();
                }
                else
                {
                    // Si no hubo colision, entonces movemos el bounding sphere
                    this.reCalculateHeight();
                    this.reCalculateNormal();
                    ship.Move(movementVector);
                    shipSphere.moveCenter(movementVector);
                }

            }
            #endregion
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
            
            // Le asignamos una Posicion y Rotacion inicial aleatoria
            Random r = new Random();
            NewEnemy.Position = new Vector3(r.Next(1000) + 200, 0, r.Next(1000) + 200);
            NewEnemy.RotateShip(r.Next(360));

            NewEnemy.enemySphere = new TgcBoundingSphere(NewEnemy.ship.BoundingBox.calculateBoxCenter() + NewEnemy.Position, NewEnemy.ship.BoundingBox.calculateBoxRadius() + 40f);
            NewEnemy.shipSphere = null;
        }

        public void RotateShip(float degree)
        {
            this.ship.RotateY(degree);
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
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            // Actualiza la Posicion y Rotacion de la Barra de Vida
            targetMap.Position = new Vector2(-ship.Meshes[0].Position.X / 50 + Environment.MapShipsOffsetX, ship.Meshes[0].Position.Z / 50 + Environment.MapShipsOffsetY);
            lifeBar.calculatePosition(this.Position, elapsedTime, d3dInput, life, nitro);
            

            // Hundimiento
            if (life < 1)
            {
                this.HundirShip();
            }

            else
            {

                // Si el enemigo esta cerca de nuestro barco, se detiene y nos ataca
                if (Math.Abs(ship.Meshes[0].Position.X - targetShip.ship.Meshes[0].Position.X) < XDISTANCETOMAINSHIP
                    && Math.Abs(ship.Meshes[0].Position.Z - targetShip.ship.Meshes[0].Position.Z) < ZDISTANCETOMAINSHIP && 1 == 2)
                {
                    movementVector = new Vector3(0, 0, 0);
                    ColisionFire(targetShip);


                    // Disparamos 2 veces por segundo
                    int ms = DateTime.Now.Millisecond;
                    if (!isFiring && ((ms >= 250 && ms <= 333) || (ms >= 666 && ms <= 750)))
                    {
                        Fire();
                        isFiring = true;
                    }
                    else if (ms < 250 || (ms > 333 && ms < 666) || ms > 750)
                    {
                        isFiring = false;
                    }


                    // Rotacion automatica mientras esta frenado
                    //ship.RotateY(Geometry.DegreeToRadian(targetShip.ship.Meshes[0].Rotation.Y * elapsedTime));
                    //ship.Meshes[0].Rotation = targetShip.ship.Meshes[0].Rotation;

                    // Copia nuestra rotacion, para simular que nos apunta
                    //ship.copyShipRotationY(targetShip);
                }
                else
                {
                    // Rota 180 y luego empieza a rotar en direccion opuesta
                    if (ship.Meshes[0].Rotation.Y > Geometry.DegreeToRadian(MAXROTATION))
                        rotationY = -1;
                    else if (ship.Meshes[0].Rotation.Y < Geometry.DegreeToRadian(-MAXROTATION))
                        rotationY = 1;
                    //ship.RotateY(Geometry.DegreeToRadian(SPEEDROTATION * elapsedTime * rotationY));

                    // Actualiza el icono que representa al barco en el Mapa
                    //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    targetMap.Rotation += Geometry.DegreeToRadian(SPEEDROTATION * elapsedTime * rotationY);
                    targetMap.render();
                    //Finalizar el dibujado de Sprites
                    GuiController.Instance.Drawer2D.endDrawSprite();

                    // Avanza cierta distancia y luego vuelve en direccion opuesta
                    //if (ship.Meshes[0].Position.X > DISTANCE)
                    //    directionX = -1;
                    //else if (ship.Meshes[0].Position.X < -DISTANCE)
                    //    directionX = 1;

                    /* Avanza para acercarse al barco del personaje */
                    float _ZDirection;
                    float _XDirection;

                    if (targetShip.Position.X < this.Position.X) _XDirection = -1;
                    else if (targetShip.Position.X > this.Position.X) _XDirection = 1;
                    else _XDirection = 0;

                    if (targetShip.Position.Z < this.Position.Z) _ZDirection = -1;
                    else if (targetShip.Position.Z > this.Position.Z) _ZDirection = 1;
                    else _ZDirection = 0;
                    
                    Vector3 vectorDist = targetShip.Position - ship.Meshes[0].Position;

                    //if (vectorDist.Length() > DISTANCE)
                    //{
                    //    rotationY = 
                    //}


                    movementVector = new Vector3(
                        FastMath.Cos(ship.RotationY()) * moveForward * elapsedTime * speedForward * _XDirection,
                        jump,
                        -FastMath.Sin(ship.RotationY()) * moveForward * elapsedTime * speedForward * _ZDirection
                        );

                }

                //Guardar posicion original antes de cambiarla
                Vector3 originalPos = this.Position;

                //Ejecutar algoritmo de detección de colisiones
                bool collisionFound = false;
                //Chequear si el objeto principal en su nueva posición choca con alguno de los objetos de la escena. bool collisionFound = false;
                //Ejecutar algoritmo de detección de colisiones
                collisionFound = TgcCollisionUtils.testSphereSphere(targetShip.BoundingSphere, this.enemySphere);

                //Si hubo alguna colisión, entonces restaurar la posición original del mesh
                if (collisionFound)
                {
                    this.Position = originalPos;
                    ColisionFire(targetShip);
                    this.reCalculateHeight();
                    this.reCalculateNormal();
                    
                }
                else
                {
                    ship.Move(movementVector);
                    this.reCalculateHeight();
                    this.reCalculateNormal();
                    enemySphere.moveCenter(movementVector);
                    ship.RotateY(Geometry.DegreeToRadian(SPEEDROTATION * elapsedTime * rotationY));
                }

                //ship.Move(movementVector);
                //enemySphere.moveCenter(movementVector);
            }
            
            #endregion
        }

        private void ColisionFire(Ship targetShip)
        {/*
            if (targetShip.life >= 0)
            {
                targetShip.life -= 0.2f;
            }
            if (life >= 0 && targetShip.isFiring)
            {
                life -= 0.2f;
            }
            */
            // Disparamos 1 vez por segundo
            int ms = DateTime.Now.Millisecond;
            if (!isFiring && (ms >= 500))
            {
                Fire();
                isFiring = true;
            }
            else if (ms < 500)
            {
                isFiring = false;
            }
        }

        public override Vector3 Spawn()
        {
            return (new Vector3(500, 0, 500));
        }

        public override TgcBoundingSphere BoundingSphere
        {
            get { return enemySphere; }
        }
    }

}
