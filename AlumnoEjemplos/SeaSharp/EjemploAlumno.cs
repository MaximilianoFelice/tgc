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
        public static int aux = 0;
        public static bool aux2 = false;
        public static Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
        CubeTexture g_pCubeMap = new CubeTexture(device, 256, 1, Usage.RenderTarget,
               Format.A16B16G16R16F, Pool.Default);

        public static int r;
        public Random rand;

        public static Lluvia lluvia;


        #region STRUCTURAL_INFO
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "SeaSharp";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Creacion de un modelo basico de nivel";
        }
        #endregion

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            FPSCounters.Load();

            ConfigParam.Load();
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

            lluvia = new Lluvia();
            lluvia.Load();

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
            GuiController.Instance.RotCamera.RotationSpeed = 3;
            GuiController.Instance.RotCamera.CameraDistance = 1000;          

        }

        
        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            ConfigParam.UpdateValues();

            // Pantalla principal
            if (MainScreen.mainScreenVisible)
            {
                MainScreen.MainScreenRender();
            }
            // Pantalla Menu
            else if (MainScreen.menuBackVisible)
            {
                MainScreen.MenuScreenRender();
            }
            // Pantalla en Juego
            else
            {
                // Borramos la pantalla anterior                // TODO Super refactor here.
                if (MainScreen.menuBack != null)
                {
                    MainScreen.menuBack.dispose();
                    MainScreen.menuBack = null;
                }

                //GuiController.Instance.D3dDevice.Transform.Projection =
                //   Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                //   aspectRatio, zNearPlaneDistance, zFarPlaneDistance);
                GuiController.Instance.FpsCounterEnable = true;
                //////////////////////////////////////
                Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

                if (aux == 0)
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
                        SkyDome.Render();
                        //SkyDome.Close();
                        //EnemyFleet.RenderAll();
                        //ship.Render();
                        //Environment.Render();
                        Bola.RenderAll();
                        //SurfaceLoader.Save("prueba.bmp", ImageFileFormat.Bmp, pFace);

                        // pSurf.Dispose();
                        //string fname = string.Format("face{0:D}.bmp", nFace);
                        //SurfaceLoader.Save(fname, ImageFileFormat.Bmp, pFace);
                    }
                    //device.BeginScene();
                device.SetRenderTarget(0, pOldRT);
                aux = 1;
                }
                //    //Renderizar 
                //    render(elapsedTime);
                //////////////////////////////////////

                rand = new Random();
                r = rand.Next(100);

                GuiController.Instance.RotCamera.CameraCenter = ship.Position; //TODO: Make camara follow rotation

                GuiController.Instance.CurrentCamera.updateViewMatrix(device);
                device.Transform.Projection =
                   Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                   aspectRatio, zNearPlaneDistance, zFarPlaneDistance);


                /*
                *          ZONA DE CALCULO
                */

                ship.CalculateMovement(elapsedTime);
                //EnemyFleet.CalculateEveryMovement(elapsedTime, ship);


                Bola.CalculateEveryMovement(elapsedTime);

                //Hacer que la camara siga a la nave en su nueva posicion
                Sea.CalculateMovement(elapsedTime, ship.Position);
                //GuiController.Instance.Frustum.FarPlane 


                /* Preparamos el device para aplicar shaders */

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

                /*
                *          ZONA DE RENDERIZADO
                */
                SkyDome.CalculateMovement();
                SkyDome.Render();
                SkyDome.Close();


                ship.Render();
                

                EnemyFleet.RenderAll();
                Environment.Render();
                Bola.RenderAll();

                FPSCounters.Render(elapsedTime);

                Sea.Render(g_pCubeMap, GuiController.Instance.Frustum, r);

                // Actualizamos los Sprites en pantalla

                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                //GuiController.Instance.Drawer2D.beginDrawSprite();
                lluvia.Render();
                //GuiController.Instance.Drawer2D.endDrawSprite();

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

            //Sea.Render(g_pCubeMap, GuiController.Instance.Frustum, r);

            //GuiController.Instance.Drawer2D.beginDrawSprite();
            //lluvia.Render();
            //GuiController.Instance.Drawer2D.endDrawSprite();

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            SkyDome.Close();
            Sea.Close();
            ship.Close();
            EnemyFleet.CloseAll();
            Bola.CloseAll();
            if (MainScreen.mainScreen != null)
                MainScreen.mainScreen.dispose();
            Environment.Close();
            FPSCounters.Close();
            lluvia.Close();

        }

    }
}
