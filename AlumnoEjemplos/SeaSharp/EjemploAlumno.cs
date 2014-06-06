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
        public static MainShip ship;
        TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

        public static float time = 0f;

        public static float fieldOfViewY = FastMath.ToRad(45.0f);
        public static float aspectRatio = 1f;
        public static float zNearPlaneDistance = 1f;
        public static float zFarPlaneDistance = 100000f;

        public Texture g_pRenderTarget;

        public static Surface pOldRT;
        public static int aux = 1;
        public static bool aux2 = false;
        public static Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
             CubeTexture g_pCubeMap = new CubeTexture(device, 256, 1, Usage.RenderTarget,
                    Format.A16B16G16R16F, Pool.Default);



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
            GuiController.Instance.FpsCounterEnable = true;
            //////////////////////////////////////
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            if (aux % 1 == 0)
            {
           
                //g_pRenderTarget = new Texture(device, device.PresentationParameters.BackBufferWidth
                //     , device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                //       Format.X8R8G8B8, Pool.Default);

                //Surface pSurf = g_pRenderTarget.GetSurfaceLevel(0);

                pOldRT = device.GetRenderTarget(0);
                //// ojo: es fundamental que el fov sea de 90 grados.
                //// asi que re-genero la matriz de proyeccion
                device.Transform.Projection =
                    Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(90.0f),
                        1f, zNearPlaneDistance, zFarPlaneDistance);

                //device.SetRenderTarget(0, pSurf);

                // Genero las caras del enviroment map
                for (CubeMapFace nFace = CubeMapFace.PositiveX; nFace <= CubeMapFace.NegativeZ; ++nFace)
                {
                    Surface pFace = g_pCubeMap.GetCubeMapSurface(nFace, 0);
                    device.SetRenderTarget(0, pFace);
                    Vector3 Dir, VUP;
                    Color color;
                    switch (nFace)
                    {
                        default:
                        case CubeMapFace.PositiveX:
                            // Left
                            Dir = new Vector3(1, 0, 0);
                            VUP = new Vector3(0, 1, 0);
                            color = Color.Black;
                            break;
                        case CubeMapFace.NegativeX:
                            // Right
                            Dir = new Vector3(-1, 0, 0);
                            VUP = new Vector3(0, 1, 0);
                            color = Color.Red;
                            break;
                        case CubeMapFace.PositiveY:
                            // Up
                            Dir = new Vector3(0, 1, 0);
                            VUP = new Vector3(0, 0, -1);
                            color = Color.Gray;
                            break;
                        case CubeMapFace.NegativeY:
                            // Down
                            Dir = new Vector3(0, -1, 0);
                            VUP = new Vector3(0, 0, 1);
                            color = Color.Yellow;
                            break;
                        case CubeMapFace.PositiveZ:
                            // Front
                            Dir = new Vector3(0, 0, 1);
                            VUP = new Vector3(0, 1, 0);
                            color = Color.Green;
                            break;
                        case CubeMapFace.NegativeZ:
                            // Back
                            Dir = new Vector3(0, 0, -1);
                            VUP = new Vector3(0, 1, 0);
                            color = Color.Blue;
                            break;
                    }

                    //    //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
                    Vector3 Pos = ship.Position;
                    

                    //Vector3 aux = GuiController.Instance.RotCamera.getPosition() - GuiController.Instance.RotCamera.getLookAt();

                    //Vector3 Refl = new Vector3(-aux.X, aux.Y, -aux.Z);

                    device.Transform.View = Matrix.LookAtLH(Pos, Pos + Dir, VUP);

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);



                    SkyDome.CalculateMovement();

                    //SkyDome.Close();
                    SkyDome.Render();
                    EnemyFleet.RenderAll();
                    //ship.Render();
                    Environment.Render();
                    Bola.RenderAll();
                    //SurfaceLoader.Save("prueba.bmp", ImageFileFormat.Bmp, pFace);

                    // pSurf.Dispose();
                //string fname = string.Format("face{0:D}.bmp", nFace);
                //SurfaceLoader.Save(fname, ImageFileFormat.Bmp, pFace);
                }
                //device.BeginScene();
            }
            //    //Renderizar 
            //    render(elapsedTime);
            //////////////////////////////////////

            GuiController.Instance.RotCamera.CameraCenter = ship.Position; //TODO: Make camara follow rotation

            device.SetRenderTarget(0, pOldRT);
            GuiController.Instance.CurrentCamera.updateViewMatrix(device);
            device.Transform.Projection =
               Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
               aspectRatio, zNearPlaneDistance, zFarPlaneDistance);


            /*
            *          ZONA DE CALCULO
            */ 
    
            ship.CalculateMovement(elapsedTime);
            EnemyFleet.CalculateEveryMovement(elapsedTime, ship);

            Bola.CalculateEveryMovement(elapsedTime);

            //Hacer que la camara siga a la nave en su nueva posicion
            Sea.CalculateMovement(elapsedTime);
            //GuiController.Instance.Frustum.FarPlane 


            /* Preparamos el device para aplicar shaders */

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            /*
            *          ZONA DE RENDERIZADO
            */
            SkyDome.Render();
            SkyDome.Close();


            ship.Render();

             
                EnemyFleet.RenderAll();
                Environment.Render();
                Bola.RenderAll();


                // Actualizamos los Sprites en pantalla

                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                // Barra de vida en pantalla
                if (ship.life >= 0)
                {
                    Environment.lifeBar.Scaling = new Vector2(ship.life * Environment.lifeScalingX / 100, Environment.lifeBar.Scaling.Y);
                    Environment.lifeBarBg.render();
                    Environment.lifeBar.render(); 
                }               

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
                if (ship.life >= 0)
                {
                    Environment.lifeText.Text = (int)ship.life + "/100";
                    Environment.lifeText.render();
                }
            }

            Sea.Render(g_pCubeMap, GuiController.Instance.Frustum);

            aux++;
            if (aux >= 3)
            {
                aux = 1;
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
