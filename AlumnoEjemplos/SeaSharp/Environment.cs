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

namespace AlumnoEjemplos.SeaSharp
{
    public static class Environment
    {
        public static List<Island> islands = new List<Island>();

        public static TgcSimpleTerrain surroundingArea = new TgcSimpleTerrain();

        public static TgcScene palmeras;

        public static TgcSprite mainScreen;
        public static bool mainScreenVisible = true;

        public static TgcSprite menuBack;
        public static bool menuBackVisible = false;

        public static TgcText2d pressStart;
        public static string PRESSSTART = "» PRESIONA ENTER «";
        public static bool pressStartVisible = true;

        public static TgcSprite lifeBarBg;
        public static TgcSprite lifeBar;
        public static float lifeScalingX; // Ancho de la barra de vida estando al 100%
        public static TgcText2d lifeText;

        public static TgcSprite nitroBarBg;
        public static TgcSprite nitroBar;
        public static float nitroScalingX;
        public static TgcText2d nitroText;

        public static TgcText2d selector;
        public static TgcText2d titulo1;
        public static TgcText2d opcion1;
        public static TgcText2d opcion2;
        public static TgcText2d opcion3;

        public static int topMenuPosition = GuiController.Instance.Panel3d.Size.Height / 4;
        public static int spaceBetweenOptions = 40;

        public static float MapShipsOffsetX = (GuiController.Instance.Panel3d.Width * 0.86f);
        public static float MapShipsOffsetY = (GuiController.Instance.Panel3d.Height * 0.8f);

        public static TgcSprite MapBack;
        public static TgcText2d mapText;

        public static TgcSprite FlagBlack;

        // public static int limit = 250; // Establece el limite del skybox y lo ajusta a la escala (20)

        // public static int differential = 35; // Establece la diferencia entre islas, esta en funcion de la escala de las mismas (20)

        public static void Load()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar terreno: cargar heightmap y textura de color
            Vector3 Island_Pos = new Vector3(0, -100, 0);
            surroundingArea.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\islandGrande2.jpg", 157, 5, Island_Pos);
            surroundingArea.loadTexture(GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");
            
            TgcSceneLoader loader = new TgcSceneLoader();
            palmeras = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Palmeras\\palmerasIslaGrande-TgcScene.xml");
            palmeras.Scale(new Vector3(5, 5, 5));
            palmeras.Move(new Vector3(4300, 100, 4300));

            // Pantalla principal
            mainScreen = new TgcSprite();
            mainScreen.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\MainScreenRed.jpg");                        
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = mainScreen.Texture.Size;
            mainScreen.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            // TODO: Verificar que ocupe el ancho y alto de la pantalla
            mainScreen.Scaling = new Vector2(0.48f,0.26f);

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
            menuBack.Scaling = new Vector2(0.48f, 0.26f);

            // Fondo de la barra de vida
            lifeBarBg = new TgcSprite();
            lifeBarBg.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarBack.png");
            lifeBarBg.Position = new Vector2(10, 10);
            lifeBarBg.Scaling = new Vector2(0.5f, 0.4f);            

            // Vida variable
            lifeBar = new TgcSprite();
            lifeBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarGreen.png");
            lifeBar.Position = new Vector2(15, 14);
            lifeScalingX = 0.48f;
            lifeBar.Scaling = new Vector2(lifeScalingX, 0.28f);

            // Bandera Pirata 
            FlagBlack = new TgcSprite();
            FlagBlack.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\FlagBlack.png");
            FlagBlack.Position = new Vector2(1, 1);
            FlagBlack.Scaling = new Vector2(0.15f, 0.15f);

            // Texto Vida
            lifeText = new TgcText2d();
            lifeText.Text = "100/100";
            lifeText.Align = TgcText2d.TextAlign.RIGHT;
            lifeText.changeFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));            
            int lifeBarPositionRightX = (int)(lifeBar.Position.X /*+ lifeBar.SrcRect.Width * lifeBar.Scaling.X*/);
            int lifeBarPositionRightY = (int)(lifeBar.Position.Y + lifeBar.SrcRect.Height * lifeBar.Scaling.Y);
            lifeText.Position = new Point(-GuiController.Instance.Panel3d.Width / 2 - 320, lifeBarPositionRightY);
            lifeText.Color = Color.White;
            
            // Fondo de la barra de nitro
            nitroBarBg = new TgcSprite();
            nitroBarBg.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarBack.png");
            nitroBarBg.Position = new Vector2(10, 40);
            nitroBarBg.Scaling = new Vector2(0.5f, 0.25f);

            // Nitro variable
            nitroBar = new TgcSprite();
            nitroBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarBlue.png");
            nitroBar.Position = new Vector2(15, 43);
            nitroScalingX = 0.48f;
            nitroBar.Scaling = new Vector2(nitroScalingX, 0.15f);

            // Fondo del Mapa
            MapBack = new TgcSprite();
            MapBack.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\MapBack.png");
            MapBack.Position = new Vector2(screenSize.Width * 0.75f, screenSize.Height * 0.6f);
            MapBack.Scaling = new Vector2(0.45f, 0.40f);

            // Islas
            islands.Add(new Island(new Vector3(100, 0, 100)));
            islands.Add(new Island(new Vector3(-70, 0, -150)));
            

            /* LOGICA DEPRECIADA: Reducia mucho los FPS*/
            // TODO: Borrar cuando llegue el nuevo terrain.

            //int upper_limit = limit;
            //int side_limit = limit;

            //for (; upper_limit > (-1) * limit; upper_limit -= differential)
            //{
            //    islands.Add(new Island (new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; side_limit > (-1) * limit; side_limit -= differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; upper_limit < limit; upper_limit += differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; side_limit < limit; side_limit += differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}
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
            opcion2.Position = new Point(0, topMenuPosition + spaceBetweenOptions*2);

            opcion3 = new TgcText2d();
            opcion3.Text = "Noche";
            opcion3.Size = GuiController.Instance.Panel3d.Size;
            opcion3.Align = TgcText2d.TextAlign.CENTER;
            opcion3.changeFont(new System.Drawing.Font("Arial", 22, FontStyle.Regular));
            opcion3.Position = new Point(0, topMenuPosition + spaceBetweenOptions*3);

        }

        public static void RenderizarMenu1()
        {
            titulo1.render();
            opcion1.render();
            opcion2.render();
            opcion3.render();
            selector.render();
        }               

        public static void Render(){
            foreach (Island island in islands) island.Render();
            surroundingArea.render();
            palmeras.renderAll();

        }

        public static void Close()
        {
            foreach (Island island in islands) island.Close();
            surroundingArea.dispose();
            palmeras.renderAll();
        }
    }
}
