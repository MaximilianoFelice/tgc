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
        const float ROTATESPEED = 10;
        float i = 1f;           // TODO: Renombrar esta variable a un nombre significativo.

        public static List<Bola> FiredBalls = new List<Bola>();

        #region PROPERTIES
        public Vector3 Position
        {
            get { return bola.Position; }
            set { bola.Position = value; }

        }

        private float _Angle;
        public float Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        #endregion 

        #region BALL_HANDLER
        public static void RenderAll()
        {
            foreach (Bola Fired in FiredBalls) Fired.Render();
        }

        public static void CloseAll()
        {
            foreach (Bola Fired in FiredBalls) Fired.Dispose();
        }

        public static void CalculateEveryMovement(float elapsedTime)
        {
            /* Se utiliza una estructura auxiliar para poder recorrer el foreach. Caso contrario, se rompe el indice y no permite remover items. */
            List<Bola> AuxiliarFiredBalls = new List<Bola>(FiredBalls);
            foreach (Bola Fired in AuxiliarFiredBalls) Fired.CalculatePath(elapsedTime);
        }
        #endregion

        public void Fire()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            bola = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Pelota\\Sphere-TgcScene.xml").Meshes[0];

            bola.Scale = new Vector3(1, 1, 1);
            FiredBalls.Add(this);
        }

        public void Render()
        {

            bola.render();
        }

        public void Dispose()
        {
            FiredBalls.RemoveAll(x => x == this);
            bola.dispose();
        }

        public void CalculatePath(float elapsedTime)
        {         
            bola.move(3*FastMath.Cos(_Angle), 3 - 5*i*elapsedTime,-FastMath.Sin(_Angle)*3);   // TODO: Aclarar que es cada valor en esta linea. En lo posible pasarlos a constantes mas expresivas.   
            if((bola.Position.Y) < 0f)
            {
                this.Dispose();
            }
            else
            {
                i++;
                bola.rotateX(ROTATESPEED * elapsedTime);
            }
        }

    }
}
