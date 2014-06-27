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
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.SeaSharp
{
    public static class Environment
    {
        public static List<Island> islands = new List<Island>();

        public static TgcSimpleTerrain surroundingArea = new TgcSimpleTerrain();

        public static TgcScene palmeras;

        public static TgcSprite lifeBarBg;
        public static TgcSprite lifeBar;
        public static float lifeScalingX; // Ancho de la barra de vida estando al 100%
        public static TgcText2d lifeText;

        public static TgcSprite nitroBarBg;
        public static TgcSprite nitroBar;
        public static float nitroScalingX;
        public static TgcText2d nitroText;


        public static float MapShipsOffsetX = (GuiController.Instance.Panel3d.Width * 0.86f);
        public static float MapShipsOffsetY = (GuiController.Instance.Panel3d.Height * 0.8f);

        public static TgcSprite MapBack;
        public static TgcText2d mapText;

        public static TgcSprite FlagBlack;

        public static List<TgcBox> obstaculos;

        public static Texture text;
        public static Texture height;

        // public static int limit = 250; // Establece el limite del skybox y lo ajusta a la escala (20)

        // public static int differential = 35; // Establece la diferencia entre islas, esta en funcion de la escala de las mismas (20)

        public static void Load()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar terreno: cargar heightmap y textura de color
            Vector3 Island_Pos = new Vector3(0, -250, 0);
            surroundingArea.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\Terrain.jpg", 305, 100, Island_Pos);
            text = TextureLoader.FromFile(GuiController.Instance.D3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");
            height = TextureLoader.FromFile(GuiController.Instance.D3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\Terrain.jpg");
            surroundingArea.loadTexture(GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");
            //surroundingArea.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx"); ;
            //surroundingArea.Technique = "RenderScene";
            
            TgcSceneLoader loader = new TgcSceneLoader();
            palmeras = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Palmeras\\palmerasIslaGrande-TgcScene.xml");
            palmeras.Scale(new Vector3(5, 5, 5));
            palmeras.Move(new Vector3(8300, 500, 8300));


            MainScreen.Load();

            Size screenSize = GuiController.Instance.Panel3d.Size;

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
//            MapBack.Scaling = new Vector2(0.45f, 0.40f);
            MapBack.Scaling = new Vector2(screenSize.Width / MapBack.Texture.Width, screenSize.Height / MapBack.Texture.Height);
            
            // Islas
            islands.Add(new Island(new Vector3(100, 0, 100)));
            islands.Add(new Island(new Vector3(-70, 0, -150)));

            //Cargar obstaculos y posicionarlos. Los obstáculos se crean con TgcBox en lugar de cargar un modelo.
            obstaculos = new List<TgcBox>();
            TgcBox obstaculo;


            float wallSize = 16000;
            float wallHeight = 1000;

            //Obstaculo 1
            obstaculo = TgcBox.fromExtremes(
                new Vector3(0, 0, 0),
                new Vector3(wallSize, wallHeight, 10),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\baldosaFacultad.jpg"));
            obstaculo.move(-8000, 0, -8000);
            obstaculos.Add(obstaculo);

            //Obstaculo 2
            obstaculo = TgcBox.fromExtremes(
                new Vector3(0, 0, 0),
                new Vector3(10, wallHeight, wallSize),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\madera.jpg"));
            obstaculo.move(-8000, 0, -8000);
            obstaculos.Add(obstaculo);

            //Obstaculo 3
            obstaculo = TgcBox.fromExtremes(
                new Vector3(0, 0, wallSize),
                new Vector3(wallSize, wallHeight, wallSize + 10),
                TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\granito.jpg"));
            obstaculo.move(-8000, 0, -8000);
            obstaculos.Add(obstaculo);

            //Obstaculo 4
            obstaculo = TgcBox.fromExtremes(
                new Vector3(wallSize, 0, 0),
                new Vector3(wallSize + 10, wallHeight, wallSize));
            obstaculo.move(-8000, 0, -8000);
            obstaculos.Add(obstaculo);

            
        }

               

        public static void Render(bool islaGrande){
            //surroundingArea.Effect.SetValue("texDiffuseMap", text);
            //surroundingArea.Effect.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(ConfigParam.Sea.getLightPos()));
            //surroundingArea.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getPosition()));
            //surroundingArea.Effect.SetValue("fvEyeLookAt", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getLookAt()));
            //surroundingArea.Effect.SetValue("k_la", ConfigParam.Sea.getAmbient());
            //surroundingArea.Effect.SetValue("k_ld", ConfigParam.Sea.getDiffuse());
            //surroundingArea.Effect.SetValue("k_ls", ConfigParam.Sea.getSpecular());
            //surroundingArea.Effect.SetValue("fSpecularPower", ConfigParam.Sea.getSpecularPower());

            foreach (Island island in islands) island.Render();
            if(islaGrande && ConfigParam.Environment.getEnvironment()) surroundingArea.render();
            palmeras.renderAll();
            
            /*foreach (TgcBox obstaculo in obstaculos)
            {
                obstaculo.render();
            }*/

        }

        public static void Close()
        {
            foreach (Island island in islands) island.Close();
            surroundingArea.dispose();
            palmeras.renderAll();
            foreach (TgcBox obstaculo in obstaculos)
            {
                obstaculo.dispose();
            }
        }
    }
}
