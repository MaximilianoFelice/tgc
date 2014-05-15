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
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.SeaSharp
{
    // TODO Deshacer estática y agregar metodos de movimiento
    public static class Island
    {
        public static TgcSimpleTerrain island;
        public static TgcScene palmeras; 


        public static void Load()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar terreno: cargar heightmap y textura de color
            island = new TgcSimpleTerrain();
            island.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\island1.jpg", 20, 1, new Vector3(1000, -150, 1000));
            island.loadTexture(GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");

     
        }

        

        public static void Render()
        {
            island.render();
        }

        public static void Close()
        {
            island.dispose();
        }
    }
}
