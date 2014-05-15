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
    public class Island
    {
        public TgcSimpleTerrain island;
        public TgcScene palmeras; 


        public void Load()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar terreno: cargar heightmap y textura de color
            island = new TgcSimpleTerrain();
            island.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\island1.jpg", 20, 1, new Vector3(0, -150, 0));
            island.loadTexture(GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");

            TgcSceneLoader loader = new TgcSceneLoader();
            palmeras = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Palmeras\\palmeras2-TgcScene.xml");
            palmeras.Position(new Vector3(0, 250, 0));

     
        }

        public void Render()
        {
            island.render();
            palmeras.renderAll();
        }

        public void Close()
        {
            island.dispose();
            palmeras.disposeAll();
        }
    }
}
