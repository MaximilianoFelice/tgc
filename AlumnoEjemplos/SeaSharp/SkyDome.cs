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
    public static class SkyDome
    {
        public static TgcSkyBox skyBox;
        public static string texturesPath;

        public static void Load()
        {
            /* 
             *  Cconfiguramos el SkyBox, objeto que nos permite armar una cajita con el fondo.
             */
            skyBox = new TgcSkyBox();

            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(20000, 20000, 20000);
            texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Sky\\";
            SetFaceTextures();
        }

        public static void SetFaceTextures()
        {
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "arriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "abajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "centro.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "der2.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "izq.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "der.jpg");
            skyBox.updateValues();
        }


        public static void CalculateMovement()
        {
            skyBox.Center = GuiController.Instance.RotCamera.getPosition();
            skyBox.updateValues();
        }

        public static void Render()
        {
            skyBox.render();
        }

        public static void Close()
        {
            skyBox.dispose();
        }

    }
}
