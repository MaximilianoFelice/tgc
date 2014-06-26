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
using TgcViewer.Utils._2D;


namespace AlumnoEjemplos.SeaSharp
{
    public class Lluvia
    {
        //TgcBox gota;
        List<TgcBox> gotas;

        TgcSprite lluvia1;
        TgcSprite lluvia2;
        TgcSprite lluvia3;
        TgcSprite lluvia4;
        TgcSprite lluvia5;

        float scalingx = 0.8f;
        float scalingy = 0.8f;

        TgcSprite relampago1;
        TgcSprite relampago2;

        public static int lluvia; //Indica que imagen de lluvia se muestra actualmente
        public static bool relampago;
        public void Load()
        {
            //gota = TgcBox.fromSize(new Vector3(0, 1000, 0), new Vector3(0.1f, 50, 0.1f), Color.Red);
            //gota = new TgcCylinder(new Vector3(0, 100, 0), 0.2f, 0.1f, 50);
            //gota.Center = new Vector3(0, 100, 0);
            //gota.TopRadius = 10;
            //gota.BottomRadius = 1;
            lluvia1 = new TgcSprite();
            lluvia1.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\RainA2.png");
            lluvia1.Position = new Vector2(0, 0);
            lluvia1.Scaling = new Vector2(scalingx, scalingy);

            lluvia2 = new TgcSprite();
            lluvia2.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\RainB2.png");
            lluvia2.Position = new Vector2(0, 0);
            lluvia2.Scaling = new Vector2(scalingx, scalingy);

            lluvia3 = new TgcSprite();
            lluvia3.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\RainC2.png");
            lluvia3.Position = new Vector2(0, 0);
            lluvia3.Scaling = new Vector2(scalingx, scalingy);

            lluvia4 = new TgcSprite();
            lluvia4.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\RainD2.png");
            lluvia4.Position = new Vector2(0, 0);
            lluvia4.Scaling = new Vector2(scalingx, scalingy);

            lluvia5 = new TgcSprite();
            lluvia5.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\RainE2.png");
            lluvia5.Position = new Vector2(0, 0);
            lluvia5.Scaling = new Vector2(scalingx, scalingy);

            lluvia = 1;

            relampago1 = new TgcSprite();
            relampago1.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\relampago1.png");
            relampago1.Position = new Vector2(0, 0);
            relampago1.Scaling = new Vector2(0.9f, 0.9f);

            relampago2 = new TgcSprite();
            relampago2.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Screen\\relampago2.png");
            relampago2.Position = new Vector2(0, 0);
            relampago2.Scaling = new Vector2(0.9f, 0.9f);

            relampago = false;
        }

        public void Render()
        {
            Random r = new Random();

            if (r.Next(80) == 5)
            {
                relampago2.render();
                relampago = true;
            }

            if (relampago)
            {
                if (r.Next(2) == 1)
                    relampago2.render();
                else
                {
                    relampago1.render();
                    relampago = false;
                }
            }
            if (r.Next(150) == 5)
                relampago2.render();

            switch (lluvia)
            {
                case 1:
                    lluvia2.render();
                    lluvia = 2;
                    break;
                case 2:
                    lluvia3.render();
                    lluvia = 3;
                    break;
                case 3:
                    lluvia4.render();
                    lluvia = 4;
                    break;
                case 4:
                    lluvia5.render();
                    lluvia = 5;
                    break;
                case 5:
                    lluvia1.render();
                    lluvia = 1;
                    break;
                default:
                    break;
            }
            //gota.move(new Vector3(0, -50, 0));
            //if(gota.Position.Y < 0)
            //{
            //    gota.Position = new Vector3(0, 1000, 0);
            //}
            //gota.render();
        }

        public void Close()
        {
            //gotas.dispose();
        }
    }
}
