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
using TgcViewer.Utils.Shaders;


namespace AlumnoEjemplos.SeaSharp
{
    public class Lluvia
    {
        TgcBox gota;
        List<Lluvia> gotas;

        public void Load()
        {
            gota = TgcBox.fromSize(new Vector3(0, 1000, 0), new Vector3(0.1f, 50, 0.1f), Color.Red);
            //gota = new TgcCylinder(new Vector3(0, 100, 0), 0.2f, 0.1f, 50);
            //gota.Center = new Vector3(0, 100, 0);
            //gota.TopRadius = 10;
            //gota.BottomRadius = 1;
        }

        public void Render()
        {
            gota.move(new Vector3(0, -50, 0));
            if(gota.Position.Y < 0)
            {
                gota.Position = new Vector3(0, 1000, 0);
            }
            gota.render();
        }

        public void Close()
        {
            gota.dispose();
        }
    }
}
