using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;


namespace AlumnoEjemplos.SeaSharp
{
    public static class MainScreen
    {
        public static TgcSprite mainScreen;
        public static bool mainScreenVisible = true;

        public static TgcSprite menuBack;
        public static bool menuBackVisible = false;

        public static TgcText2d pressStart;
        public static string PRESSSTART = "» PRESIONA ENTER «";
        public static bool pressStartVisible = true;

        public static TgcText2d selector;
        public static TgcText2d titulo1;
        public static TgcText2d opcion1;
        public static TgcText2d opcion2;
        public static TgcText2d opcion3;

        public static int topMenuPosition = GuiController.Instance.Panel3d.Size.Height / 4;
        public static int spaceBetweenOptions = 40;

        public static TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

        public static void Load()
        {

            // Pantalla principal
            mainScreen = new TgcSprite();
            mainScreen.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\MainScreenRed.jpg");
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = mainScreen.Texture.Size;
            mainScreen.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            // TODO: Verificar que ocupe el ancho y alto de la pantalla
            mainScreen.Scaling = new Vector2(0.68f, 0.46f);

            // Presionar Enter
            pressStart = new TgcText2d();
            pressStart.Size = GuiController.Instance.Panel3d.Size;
            pressStart.Align = TgcText2d.TextAlign.CENTER;
            pressStart.Text = PRESSSTART;
            pressStart.changeFont(new System.Drawing.Font("Arial Black", 18, FontStyle.Regular));
            pressStart.Position = new Point(0, GuiController.Instance.Panel3d.Size.Height - 60);
            pressStart.Color = Color.Red;

            // Fondo de Menu
            menuBack = new TgcSprite();
            menuBack.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\MenuBackRed.jpg");
            Size textureSizeMenu = menuBack.Texture.Size;
            menuBack.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeMenu.Width / 2, 0), screenSize.Height/*FastMath.Max(screenSize.Height / 2 - textureSizeMenu.Height / 2, 0)*/);
            // TODO: Verificar que ocupe el ancho y alto de la pantalla
            menuBack.Scaling = new Vector2(0.68f, 0.46f);

        }

        public static void MainScreenRender()
        {
            // Pantalla principal
            if (mainScreenVisible)
            {
                //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
                GuiController.Instance.Drawer2D.beginDrawSprite();

                mainScreen.render();

                // Evento Presionar enter
                if (d3dInput.keyDown(Key.Return))
                {
                    mainScreenVisible = false;
                    menuBackVisible = true;
                }

                //Finalizar el dibujado de Sprites
                GuiController.Instance.Drawer2D.endDrawSprite();

                // Titilar texto "Presionar Enter"
                if (DateTime.Now.Millisecond < 500)
                    pressStart.Text = "";
                else
                    pressStart.Text = PRESSSTART;
                pressStart.render();
            }
        }

        public static void MenuScreenRender()
        {
            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            // Seguimos renderizando la pantalla anterior hasta que se muestre la pantalla de Menu
            if (mainScreen != null)
                mainScreen.render();

            // Efecto de aparicion del Menu y creacion de opciones
            if (menuBack.Position.Y > 5)
            {
                menuBack.Position = new Vector2(menuBack.Position.X, menuBack.Position.Y - (menuBack.Position.Y * 0.09f));
                if (menuBack.Position.Y <= 5)
                {
                    CrearMenu1();
                    topMenuPosition = GuiController.Instance.Panel3d.Size.Height;
                }
            }
            // Termina de aparecer esta pantalla de Menu, entonces borramos la pantalla anterior
            else if (mainScreen != null)
            {
                menuBack.Position = new Vector2(menuBack.Position.X, 0);
                mainScreen.dispose();
                mainScreen = null;
            }
            menuBack.render();

            // Eventos de elegir opciones
            if (d3dInput.keyPressed(Key.Return))
            {
                menuBackVisible = false;
                if (selector != null)
                {
                    if (selector.Position.Y == opcion1.Position.Y)
                    {
                        //ConfigParam.colorMar = Color.FromArgb(5, 50, 116);
                        //SkyDome.texturesPath += "";
                        //SkyDome.SetFaceTextures();
                    }
                    else if (selector.Position.Y == opcion2.Position.Y)
                    {
                        SkyDome.texturesPath += "Tarde\\";
                        SkyDome.SetFaceTextures();
                    }
                    else if (selector.Position.Y == opcion3.Position.Y)
                    {
                        //ConfigParam.colorMar = Color.FromArgb(5, 50, 0);
                        SkyDome.texturesPath += "Noche\\";
                        SkyDome.SetFaceTextures();
                    }
                }
            }

            // Eventos del cursor de menu
            if (d3dInput.keyPressed(Key.UpArrow))
            {
                if (selector.Position.Y > opcion1.Position.Y)
                    selector.Position = new Point(selector.Position.X, selector.Position.Y - 40);
            }

            if (d3dInput.keyPressed(Key.DownArrow))
            {
                if (selector.Position.Y < opcion3.Position.Y)
                    selector.Position = new Point(selector.Position.X, selector.Position.Y + 40);
            }

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();

            // Creacion de las opciones del menu si termino la aparicion de la pantalla
            if (menuBack != null)
                if (menuBack.Position.Y <= 1)
                {
                    RenderizarMenu1();
                }
        }

        // Si se agrega un nuevo texto, agregarlo a RenderizarMenu1()
        public static void CrearMenu1()
        {
            // Opciones de menu
            selector = new TgcText2d();
            selector.Text = "»";
            selector.Size = GuiController.Instance.Panel3d.Size;
            selector.Align = TgcText2d.TextAlign.CENTER;
            selector.changeFont(new System.Drawing.Font("Arial Black", 16, FontStyle.Bold));
            selector.Position = new Point(-60, topMenuPosition + spaceBetweenOptions);
            selector.Color = Color.Red;


            titulo1 = new TgcText2d();
            titulo1.Text = "MODO DE JUEGO";
            titulo1.Size = GuiController.Instance.Panel3d.Size;
            titulo1.Align = TgcText2d.TextAlign.CENTER;
            titulo1.changeFont(new System.Drawing.Font("Arial", 22, FontStyle.Bold));
            titulo1.Position = new Point(0, topMenuPosition);


            opcion1 = new TgcText2d();
            opcion1.Text = "Día";
            opcion1.Size = GuiController.Instance.Panel3d.Size;
            opcion1.Align = TgcText2d.TextAlign.CENTER;
            opcion1.changeFont(new System.Drawing.Font("Arial", 22, FontStyle.Regular));
            opcion1.Position = new Point(0, topMenuPosition + spaceBetweenOptions);


            opcion2 = new TgcText2d();
            opcion2.Text = "Tarde";
            opcion2.Size = GuiController.Instance.Panel3d.Size;
            opcion2.Align = TgcText2d.TextAlign.CENTER;
            opcion2.changeFont(new System.Drawing.Font("Arial", 22, FontStyle.Regular));
            opcion2.Position = new Point(0, topMenuPosition + spaceBetweenOptions * 2);

            opcion3 = new TgcText2d();
            opcion3.Text = "Noche";
            opcion3.Size = GuiController.Instance.Panel3d.Size;
            opcion3.Align = TgcText2d.TextAlign.CENTER;
            opcion3.changeFont(new System.Drawing.Font("Arial", 22, FontStyle.Regular));
            opcion3.Position = new Point(0, topMenuPosition + spaceBetweenOptions * 3);

        }

        public static void RenderizarMenu1()
        {
            titulo1.render();
            opcion1.render();
            opcion2.render();
            opcion3.render();
            selector.render();
        }       
    }

}
