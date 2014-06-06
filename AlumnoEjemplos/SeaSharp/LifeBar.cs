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
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.SeaSharp
{
    public class LifeBar
    {
        public TgcBox lifeBarBg;
        public TgcBox lifeBar;
        public Vector3 lifeBarBgOffset = new Vector3(-100, 250, 0);
        public Vector3 lifeBarOffset = new Vector3(-1, -1, -1);
        public int lifeWidth; // Ancho de la barra de vida estando al 100%

        public float diffX = 0f;
        public float diffY = 0f;

        public LifeBar()
        {
            // Fondo de la barra de vida

            Vector3 center = new Vector3(0, 0, 0);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarBack.png");
            Vector3 size = new Vector3(0, texture.Size.Height / 2, texture.Size.Width / 2);
            lifeBarBg = TgcBox.fromSize(center, size, texture);

            // Vida variable

            TgcTexture texture2 = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarGreen.png");
            Vector3 size2 = new Vector3(3, (texture.Size.Height / 2) - 10, (texture.Size.Width / 2) - 10);
            Vector3 center2 = new Vector3(0, 0, 0);
            lifeWidth = (texture.Size.Width / 2) - 10;
            lifeBar = TgcBox.fromSize(center2, size2, texture2);

        }

        public void Render(float life)
        {

            if (life >= 50)
                lifeBar.setTexture(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarGreen.png"));
            else if (life < 50 && life > 25)
                lifeBar.setTexture(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarYellow.png"));
            else if (life <= 25)
                lifeBar.setTexture(TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarRed.png"));

            // Actualizamos las posicion de la vida del MainShip  
            if (life > 0)
            {
                lifeBar.Size = new Vector3(lifeBar.Size.X, lifeBar.Size.Y, life * lifeWidth / 100);
                lifeBar.moveOrientedY((lifeWidth - (life * lifeWidth / 100)) / 2);
                lifeBar.updateValues(); lifeBar.render();
            }

            lifeBarBg.render();

        }

        public void calculatePosition(Vector3 initialPosition, float elapsedTime, TgcD3dInput d3dInput, float life, float nitro)
        {
            // Actualizar Posicion, Rotacion y Color de la Barra de Vida
            lifeBarBg.Position = new Vector3(initialPosition.X + lifeBarBgOffset.X, initialPosition.Y + lifeBarBgOffset.Y, initialPosition.Z + lifeBarBgOffset.Z);
            lifeBar.Position = new Vector3(lifeBarBg.Position.X + lifeBarOffset.X, lifeBarBg.Position.Y + lifeBarOffset.Y, lifeBarBg.Position.Z + lifeBarOffset.Z);

            this.Rotate(elapsedTime, d3dInput);
            actualizarColorBarraVida(life, nitro);
        }

        private void actualizarColorBarraVida(float life, float nitro)
        {
            // Cambio de color de las barras segun la cantidad que tengan
            if (life >= 50)
                Environment.lifeBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarGreen.png");
            else if (life < 50 && life > 25)
                Environment.lifeBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarYellow.png");
            else if (life <= 25)
                Environment.lifeBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarRed.png");

            if (nitro >= 50)
                Environment.nitroBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarBlue.png");
            else if (nitro < 50 && nitro > 25)
                Environment.nitroBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarViolet.png");
            else if (nitro <= 25)
                Environment.nitroBar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\LifeBarRed.png");
        }


        public void Rotate(float elapsedTime, TgcD3dInput d3dInput)
        {
            // Rotar vida segun rotacion de la camara

            //Obtener variacion XY del mouse
            float mouseX = 0f;
            float mouseY = 0f;
            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                mouseX = d3dInput.XposRelative;
                mouseY = d3dInput.YposRelative;

                diffX += mouseX * elapsedTime * GuiController.Instance.RotCamera.RotationSpeed;
                diffY += mouseY * elapsedTime * GuiController.Instance.RotCamera.RotationSpeed;
            }
            else
            {
                diffX += mouseX;
                diffY += mouseY;
            }

            //Calcular rotacion a aplicar
            float rotX = (-diffY / FastMath.PI);
            float rotY = (diffX / FastMath.PI);

            //Truncar valores de rotacion fuera de rango
            if (rotX > FastMath.PI * 2 || rotX < -FastMath.PI * 2)
            {
                diffY = 0;
                rotX = 0;
            }

            lifeBarBg.Rotation = new Vector3(0, rotY + Geometry.DegreeToRadian(90), 0);
            lifeBar.Rotation = new Vector3(0, rotY + Geometry.DegreeToRadian(90), 0); 
        }

        public void Load()
        {

        }
    }
}
