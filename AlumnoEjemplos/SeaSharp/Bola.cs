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
        public Ship Owner { get; set; }

        public bool mainShipFire { get; set; }

        public static List<Bola> FiredBalls = new List<Bola>();

        public TgcBoundingBox BoundingBox
        {
            get { return this.bola.BoundingBox; }
        }

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

        private Vector3 _Velocity;
        public Vector3 Velocity 
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }

        private float _Gravity = -5f;

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
            bola = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Bola\\Sphere-TgcScene.xml").Meshes[0];

            bola.Scale = new Vector3(0.3f, 0.3f, 0.3f);

            _Velocity = new Vector3(-10 * FastMath.Sin(_Angle), 4, -10 * FastMath.Cos(_Angle));

            FiredBalls.Add(this);
        }

        public void Render()
        {
            List<Ship> ships = new List<Ship>();

            ships.AddRange(EnemyFleet.Enemies);
            ships.Add(EjemploAlumno.ship);
           
            foreach (Ship ship in ships)
            {
                if (this.isExplodedOnShip(ship) && !ship.Equals(Owner))
                {
                    //this.Position = enemy.Position;
                    ship.life -= 0.3f;
                }
                else
                {
                    bola.render();
                    //bola.BoundingBox.render();
                }
            }
        }

        public void Dispose()
        {
            FiredBalls.RemoveAll(x => x == this);
            bola.dispose();
        }

        public bool isExplodedOnShip(Ship unShip)
        {
            return (TgcCollisionUtils.testSphereAABB(unShip.BoundingSphere, this.bola.BoundingBox));
        }

        public void CalculatePath(float elapsedTime)
        {
            Vector3 _acceleration = new Vector3(0, _Gravity * elapsedTime, 0);
            _Velocity = Microsoft.DirectX.Vector3.Add(_Velocity * 0.9999f, _acceleration);
            
            bola.move(_Velocity);   // TODO: Aclarar que es cada valor en esta linea. En lo posible pasarlos a constantes mas expresivas.   

            if((bola.Position.Y) < -100f)

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
