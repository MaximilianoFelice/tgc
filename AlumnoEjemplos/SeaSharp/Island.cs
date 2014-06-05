﻿using System;
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
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.SeaSharp
{
    public class Island
    {
        public TgcSimpleTerrain island;
        public TgcScene palmeras;

        public TgcBoundingSphere islandSphere;
        public static List<Island> Islands = new List<Island>();


        public Island(Vector3 position)
        {
            this.Load(position);
        }

        public void Load(Vector3 position)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar terreno: cargar heightmap y textura de color
            island = new TgcSimpleTerrain();
            Vector3 Island_Pos = new Vector3(position.X, position.Y - 150, position.Z);
            island.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Island\\island1.jpg", 20, 1, Island_Pos);
            island.loadTexture(GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "tierra.jpg");
            Islands.Add(this);

            TgcSceneLoader loader = new TgcSceneLoader();
            palmeras = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Palmeras\\palmeras2-TgcScene.xml");
            palmeras.Scale (new Vector3(5, 5, 5));
            Vector3 Palmeras_Pos = new Vector3(position.X * 20, island.Position.Y + 255, position.Z * 20);
            palmeras.Position(Palmeras_Pos);

            //BoundingSphere que va a usar el ship
            islandSphere = new TgcBoundingSphere(Palmeras_Pos , 525f);

        }
        
        public void Render()
        {
            island.render();
            palmeras.renderAll();
            //islandSphere.render();
        }

        public void Close()
        {
            island.dispose();
            palmeras.disposeAll();
        }
    }
}
