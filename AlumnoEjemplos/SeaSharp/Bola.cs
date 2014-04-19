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

namespace AlumnoEjemplos.SeaSharp
{
    public class Bola
    {
        TgcMesh bola;
        float i;

        public void Load()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            bola = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Pelota\\Sphere-TgcScene.xml").Meshes[0];

            bola.Scale = new Vector3(1, 1, 1);
            i = 1f;
        }

        public void Render()
        {
            bola.render();
        }

        public void Close()
        {
            bola.dispose();
        }


        public void CalculatePosition(MainShip ship)
        {
            bola.Position = ship.Position();
        }


        public bool CalculatePath(float elapsedTime, float angle)
        {         
            bola.move(3*FastMath.Cos(angle), 3 - 5*i*elapsedTime,-FastMath.Sin(angle)*3);     
            if((bola.Position.Y) < 0f || i > 500)
            {
                bola.dispose();
                return false;
            }
            else
            {
                i++;
                return true;
            }
        }
    }
}
