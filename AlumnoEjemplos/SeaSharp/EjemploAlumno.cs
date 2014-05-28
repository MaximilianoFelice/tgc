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


namespace AlumnoEjemplos.SeaSharp
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    /// 


    public class EjemploAlumno : TgcExample
    {
        MainShip ship;
        TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

        public static float fieldOfViewY = FastMath.ToRad(45.0f);
        public static float aspectRatio = 1f;
        public static float zNearPlaneDistance = 1f;
        public static float zFarPlaneDistance = 100000f;


        #region STRUCTURAL_INFO
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory(){
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName(){
            return "SeaSharp";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription(){
            return "Creacion de un modelo basico de nivel";
        }
        #endregion

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init(){

            /* Cargamos el SkyBox, la cajita feliz que nos aporta el fondo */
            SkyDome.Load();
               
            /* Cargamos el mar */
            Sea.Load();

            /* Creamos un nuevo barco y lo cargamos */
            ship = new MainShip();
            ship.Load();
            ship.CambiarTargetMap();
            EnemyFleet.AddEnemy();
            EnemyFleet.AddEnemy();

            /* Cargamos el environment */
            Environment.Load();


            #region (Otras Camaras)
            
            //Configurar camara en Tercer Persona
            //GuiController.Instance.ThirdPersonCamera.Enable = true;
            //GuiController.Instance.ThirdPersonCamera.setCamera(ship.Position, 10, -150);
            //GuiController.Instance.ThirdPersonCamera.TargetDisplacement = new Vector3(0, 45, 0);
            //GuiController.Instance.ThirdPersonCamera.rotateY(Geometry.DegreeToRadian(180));
  
            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */
            #endregion

            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            GuiController.Instance.RotCamera.setCamera(ship.Position, 1700);
            GuiController.Instance.RotCamera.RotationSpeed = 30;
            GuiController.Instance.RotCamera.CameraDistance = 1000;
        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime){
            
            // Pantalla principal
            if (Environment.mainScreenVisible)
            {
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                Environment.mainScreen.render();
                
                // Evento Presionar enter
                if (d3dInput.keyDown(Key.Return))
                {              
                    Environment.mainScreenVisible = false;
                    Environment.menuBackVisible = true;
                }

                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();
                
                // Titilar texto "Presionar Enter"
                if (DateTime.Now.Millisecond < 500)
                    Environment.pressStart.Text = "";
                else 
                    Environment.pressStart.Text = Environment.PRESSSTART;
                Environment.pressStart.render();                
            }
            // Pantalla Menu
            else if (Environment.menuBackVisible)
            {
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                // Seguimos renderizando la pantalla anterior hasta que se muestre la pantalla de Menu
                if (Environment.mainScreen != null)
                    Environment.mainScreen.render();

                // Efecto de aparicion del Menu y creacion de opciones
                if (Environment.menuBack.Position.Y > 5)
                {
                    Environment.menuBack.Position = new Vector2(Environment.menuBack.Position.X, Environment.menuBack.Position.Y - (Environment.menuBack.Position.Y * 0.09f));
                    if (Environment.menuBack.Position.Y <= 5)
                    {
                        Environment.CrearMenu1();
                        Environment.topMenuPosition = GuiController.Instance.Panel3d.Size.Height;
                    }
                }
                // Termina de aparecer esta pantalla de Menu, entonces borramos la pantalla anterior
                else if (Environment.mainScreen != null)
                {
                    Environment.menuBack.Position = new Vector2(Environment.menuBack.Position.X, 0);
                    Environment.mainScreen.dispose(); 
                    Environment.mainScreen = null;                    
                }
                Environment.menuBack.render();
                                
                // Eventos de elegir opciones
                if (d3dInput.keyPressed(Key.Return))
                {
                    Environment.menuBackVisible = false;
                    if (Environment.selector != null) { 
                    if (Environment.selector.Position.Y == Environment.opcion1.Position.Y)
                    {
                        //SkyDome.texturesPath += "";
                        //SkyDome.SetFaceTextures();
                    }
                    else if (Environment.selector.Position.Y == Environment.opcion2.Position.Y)
                    {
                        SkyDome.texturesPath += "Tarde\\";
                        SkyDome.SetFaceTextures();
                    }
                    else if (Environment.selector.Position.Y == Environment.opcion3.Position.Y)
                    {
                        SkyDome.texturesPath += "Noche\\";
                        SkyDome.SetFaceTextures();
                    }}
                }

                // Eventos del cursor de menu
                if (d3dInput.keyPressed(Key.UpArrow))
                {
                    if(Environment.selector.Position.Y > Environment.opcion1.Position.Y)
                        Environment.selector.Position = new Point(Environment.selector.Position.X, Environment.selector.Position.Y - 40);
                }

                if (d3dInput.keyPressed(Key.DownArrow))
                {
                    if (Environment.selector.Position.Y < Environment.opcion3.Position.Y)
                        Environment.selector.Position = new Point(Environment.selector.Position.X, Environment.selector.Position.Y + 40);
                }

                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();

                // Creacion de las opciones del menu si termino la aparicion de la pantalla
                if (Environment.menuBack != null)
                    if (Environment.menuBack.Position.Y <= 1)
                    {
                        Environment.RenderizarMenu1();
                    }
            }
            // Pantalla en Juego
            else
            {
                // Borramos la pantalla anterior
                if (Environment.menuBack != null)
                {
                    Environment.menuBack.dispose();
                    Environment.menuBack = null;
                }

                //GuiController.Instance.D3dDevice.Transform.Projection =
                //   Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                //   aspectRatio, zNearPlaneDistance, zFarPlaneDistance);
                GuiController.Instance.D3dDevice.Transform.Projection =
                   Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                   aspectRatio, zNearPlaneDistance, zFarPlaneDistance);

                /*
                *          ZONA DE CALCULO
                */

                ship.CalculateMovement(elapsedTime);
                EnemyFleet.CalculateEveryMovement(elapsedTime, ship);

                
                Bola.CalculateEveryMovement(elapsedTime);

                //Hacer que la camara siga a la nave en su nueva posicion
                GuiController.Instance.RotCamera.CameraCenter = ship.Position; //TODO: Make camara follow rotation
                SkyDome.CalculateMovement();
                Sea.CalculateMovement(elapsedTime);
                

                /* Preparamos el device para aplicar shaders */

                GuiController.Instance.D3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

                /*
                *          ZONA DE RENDERIZADO
                */
                SkyDome.Render();
                SkyDome.Close();
                Sea.Render();
                                
                // Actualizamos las posicion de la vida del MainShip                
                ship.lifeBar.Size = new Vector3(ship.lifeBar.Size.X, ship.lifeBar.Size.Y, ship.life * ship.lifeWidth / 100);
                ship.lifeBar.moveOrientedY((ship.lifeWidth - (ship.life * ship.lifeWidth / 100)) / 2);
                ship.lifeBar.updateValues();                
                ship.lifeBarBg.render();
                ship.lifeBar.render();
                ship.Render();
                
                // Actualizamos las posiciones de las vidas de los barcos enemigos
                foreach (EnemyFleet e in EnemyFleet.Enemies)
                {
                    e.lifeBar.Size = new Vector3(e.lifeBar.Size.X, e.lifeBar.Size.Y, e.life * e.lifeWidth / 100);
                    e.lifeBar.moveOrientedY((e.lifeWidth - (e.life * e.lifeWidth / 100)) / 2);
                    e.lifeBar.updateValues();                    
                    e.lifeBarBg.render();                    
                    e.lifeBar.render();
                }                
                EnemyFleet.RenderAll();
                Environment.Render();
                Bola.RenderAll();


                // Actualizamos los Sprites en pantalla

                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                // Barra de vida en pantalla
                Environment.lifeBar.Scaling = new Vector2(ship.life * Environment.lifeScalingX / 100, Environment.lifeBar.Scaling.Y);
                Environment.lifeBarBg.render();
                Environment.lifeBar.render();                

                // Nitro
                Environment.nitroBar.Scaling = new Vector2(ship.nitro * Environment.nitroScalingX / 100, Environment.nitroBar.Scaling.Y);
                Environment.nitroBarBg.render();
                Environment.nitroBar.render();

                Environment.FlagBlack.render();
                
                // Mapa
                Environment.MapBack.render();

                // Iconos del Mapa
                ship.targetMap.render();
                foreach (EnemyFleet e in EnemyFleet.Enemies)
                {
                    e.targetMap.render();
                }

                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();

                // Texto que indica la cantidad de vida (encima de la barra de vida)
                Environment.lifeText.Text = (int)ship.life + "/100";
                Environment.lifeText.render();    
            }


            
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close(){
            SkyDome.Close();
            Sea.Close();
            ship.Close();
            EnemyFleet.CloseAll();
            Bola.CloseAll();
            if (Environment.mainScreen != null)
                Environment.mainScreen.dispose();
            Environment.Close();

        }

    }
}
