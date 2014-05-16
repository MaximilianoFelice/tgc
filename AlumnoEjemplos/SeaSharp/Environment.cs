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

namespace AlumnoEjemplos.SeaSharp
{
    public static class Environment
    {
        public static List<Island> islands = new List<Island>();

        public static TgcSimpleTerrain surroundingArea = new TgcSimpleTerrain();

        public static TgcScene palmeras;

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
