using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using PFormat = System.Drawing.Imaging.PixelFormat;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using Tao.FreeGlut;

namespace NeHeLessons
{

        public class MoonStages : GameWindow
        {
                static float year = 0, day = 0;
                float rotation = 0.0f;
                float xrot  = 0.0f;
                float yrot  = 0.0f;
                int[] textureID = new int[3];
                
                /// <summary>
                /// Called when your window is resized. Set your viewport here. It is also
                /// a good place to set up your projection matrix (which probably changes
                /// along when the aspect ratio of your window).
                /// </summary>
                /// <param name="e">Not used.</param>
                protected override void OnResize(EventArgs e)
                {
                        base.OnResize(e);
                        
                        GL.Viewport(0, 0, ClientRectangle.Width, ClientRectangle.Height);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadIdentity();
                        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 100.0f);
                        GL.LoadMatrix(ref projection);
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadIdentity();
                        Matrix4 lookAt = Matrix4.LookAt(0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
                        GL.LoadMatrix(ref lookAt);
                }
                
                
                public void DrawSphere(Vector3 Center, float Radius, uint Precision)
                {
                        if (Radius < 0f)
                                Radius = -Radius;
                        if (Radius == 0f)
                                throw new DivideByZeroException("DrawSphere: Radius cannot be 0f.");
                        if (Precision == 0)
                                throw new DivideByZeroException("DrawSphere: Precision of 8 or greater is required.");
                        
                        const float HalfPI = (float)(Math.PI * 0.5);
                        float OneThroughPrecision = 1.0f / Precision;
                        float TwoPIThroughPrecision = (float)(Math.PI * 2.0 * OneThroughPrecision);
                        
                        float theta1, theta2, theta3;
                        Vector3 Normal, Position;
                        
                        for (uint j = 0; j < Precision / 2; j++)
                        {
                                theta1 = (j * TwoPIThroughPrecision) - HalfPI;
                                theta2 = ((j + 1) * TwoPIThroughPrecision) - HalfPI;
                                
                                GL.Begin(BeginMode.TriangleStrip);
                                for (uint i = 0; i <= Precision; i++)
                                {
                                        theta3 = i * TwoPIThroughPrecision;
                                        
                                        Normal.X = (float)(Math.Cos(theta2) * Math.Cos(theta3));
                                        Normal.Y = (float)Math.Sin(theta2);
                                        Normal.Z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
                                        Position.X = Center.X + Radius * Normal.X;
                                        Position.Y = Center.Y + Radius * Normal.Y;
                                        Position.Z = Center.Z + Radius * Normal.Z;
                                        
                                        GL.Normal3(Normal);
                                        GL.TexCoord2(i * OneThroughPrecision, 2.0f * (j + 1) * OneThroughPrecision);
                                        GL.Vertex3(Position);
                                        
                                        Normal.X = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                                        Normal.Y = (float)Math.Sin(theta1);
                                        Normal.Z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                                        Position.X = Center.X + Radius * Normal.X;
                                        Position.Y = Center.Y + Radius * Normal.Y;
                                        Position.Z = Center.Z + Radius * Normal.Z;
                                        
                                        GL.Normal3(Normal);
                                        GL.TexCoord2(i * OneThroughPrecision, 2.0f * j * OneThroughPrecision);
                                        GL.Vertex3(Position);
                                }
                                GL.End();
                        }
                }
                
                /// <summary>
                /// Called when it is time to render the next frame. Add your rendering code here.
                /// </summary>
                /// <param name="e">Contains timing information.</param>
                protected override void OnRenderFrame(FrameEventArgs e)
                {
                
                        base.OnRenderFrame(e);
                        
                        
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        GL.LoadIdentity();
                        
                        // scale square
                        GL.Rotate(rotation, 0.0f, 0.0f, 1.0f);
                        GL.Translate(0.0f, 0.0f, -50.0f);		// Move Left 1.5 Units And Into The Screen 8.0
                        GL.Rotate (xrot, 1.0f, 0.0f, 0.0f); 
                        GL.Rotate (yrot, 0.0f, 1.0f, 0.0f); 
                        
                        //Rotates Sun
                        /*GL.Rotate(year, 0.0, 1.0, 0.0);
                        GL.Rotate(day, 0.0, 1.0, 0.0);
                        GL.PopMatrix();*/
                        
                        //sol
                        GL.PushMatrix();
                        //Texture
                        int id = GL.GenTexture();
                        GL.BindTexture(TextureTarget.Texture2D, id);
                        
                        GL.Color3(1.0f, 1.0f, 0);
                        DrawSphere(new Vector3(0.0f, 0.0f, 0.0f), 7.0f, 16);
                        GL.PopMatrix();
                        
                        //Rotates earth
                        /*GL.Rotate(year, 0.0, 1.0, 0.0);
                        GL.Rotate(day, 0.0, 1.0, 0.0);
                        GL.PopMatrix();*/
                        
                        ////terra
                        GL.PushMatrix();
                        
                        GL.Color3(0.0f, 0.0f, 1.0f);
                        DrawSphere(new Vector3(14.0f, 0.0f, 14.0f), 1.0f, 16);
                        GL.PopMatrix();
                        
                        //Rotates Moon
                        GL.Translate( 14.0f, 0.0f,  14.0f); //Centralizar para a terra o ponto de rotação
                        GL.Rotate(year, 0.0, 1.0, 0.0);
                        GL.Rotate(day,  0.0, 1.0, 0.0);
                        GL.Translate(-14.0f, 0.0f, -14.0f); //Volta ao sol o ponto de rotação
                        GL.PopMatrix();
                        
                        //Lighthing
                        /*float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                        float[] mat_shininess = { 50.0f };
                        float[] light_position = { 1.0f, 1.0f, 1.0f, 0.0f };
                        float[] light_ambient = { 0.5f, 0.5f, 0.5f, 1.0f };
 
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                        GL.ShadeModel(ShadingModel.Flat);
 
                        GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
                        GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);
                        GL.Light(LightName.Light0, LightParameter.Position, light_position);
                        GL.Light(LightName.Light0, LightParameter.Ambient, light_ambient);
                        GL.Light(LightName.Light0, LightParameter.Diffuse, mat_specular);
 
                        GL.Enable(EnableCap.Lighting);
                        GL.Enable(EnableCap.Light0);
                        GL.Enable(EnableCap.DepthTest);
                        GL.Enable(EnableCap.ColorMaterial);
                        GL.Enable(EnableCap.CullFace);*/
                        
                        //Lua
                        GL.PushMatrix();
                        
                        GL.Color3(1.0f, 1.0f, 1.0f);
                        GL.BindTexture (TextureTarget.Texture2D, textureID[0]);
                        DrawSphere(new Vector3(17.0f, 0.0f, 17.0f), 0.35f, 16);
                        GL.PopMatrix();
                        
                        //GL.Flush();
                        
                        SwapBuffers();
                }
                
                void loadTexture(string path){
                        if (!System.IO.File.Exists (path)) {
                                throw new System.IO.FileNotFoundException ();
                        }
                        
                        GL.GenTextures(1, textureID);
                        
                        Bitmap bmp = new Bitmap(path);
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        BitmapData data = bmp.LockBits(
                                                   new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                   ImageLockMode.ReadOnly,
                                                   PFormat.Format32bppArgb
                                                       );
                        
                        GL.BindTexture(TextureTarget.Texture2D, textureID[0]);
                        
                        GL.TexParameter(
                                        TextureTarget.Texture2D, 
                                        TextureParameterName.TextureMinFilter, 
                                        (int)TextureMinFilter.Linear
                                        );
                        GL.TexParameter(
                                        TextureTarget.Texture2D, 
                                        TextureParameterName.TextureMagFilter, 
                                        (int)TextureMinFilter.Linear
                                        );
                        GL.TexImage2D (
                                       TextureTarget.Texture2D, 
                                       0, 
                                       PixelInternalFormat.Rgb, 
                                       bmp.Width, 
                                       bmp.Height, 
                                       0, 
                                       OpenTK.Graphics.OpenGL.PixelFormat.Bgr, 
                                       PixelType.UnsignedByte, 
                                       data.Scan0
                                       );
                        
                        bmp.UnlockBits(data);
                        bmp.Dispose();
                                                   
                }
                
                /// <summary>
                /// Called when it is time to setup the next frame. Add you game logic here.
                /// </summary>
                /// <param name="e">Contains timing information for framerate independent logic.</param>
                protected override void OnUpdateFrame(FrameEventArgs e)
                {
                        base.OnUpdateFrame(e);
                        HandleKeyPressed ();
                        day = (day + 1);
                        year = (year + 0.2f);
                
                        if (Keyboard[Key.Escape])
                                Exit();
                }
                
                public MoonStages()
                : base(640, 480, GraphicsMode.Default, "Jeff Molofee's GL Code Tutorial ... NeHe '99")
                {
                        VSync = VSyncMode.On;
                }
                
                /// <summary>Load resources here.
                /// A general OpenGL initialization function.  Sets all of the initial parameters.
                /// </summary>
                /// <param name="e">Not used.</param>
                protected override void OnLoad(EventArgs e)
                {
                        base.OnLoad(e);
                        //loadTexture("text/crate.bmp");
                        
                        GL.Enable(EnableCap.Texture2D);
                        GL.ShadeModel(ShadingModel.Smooth);                             // Enable Smooth Shading
                        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.5f);                          // Black Background
                        GL.ClearDepth(1.0);                                            // Depth Buffer Setup
                        GL.Enable(EnableCap.DepthTest);                                 // Enables Depth Testing
                        GL.DepthFunc(DepthFunction.Lequal);                             // The Type Of Depth Testing To Do
                        GL.Hint (HintTarget.PerspectiveCorrectionHint,                  // Really Nice Perspective Calculations
                                 HintMode.Nicest);
                        
                        
                        
                        /*GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);		// This Will Clear The Background Color To Black
                        GL.ClearDepth(1.0); // Enables Clearing Of The Depth Buffer
                        GL.DepthFunc(DepthFunction.Less);      // The Type Of Depth Test To Do
                        GL.Enable(EnableCap.DepthTest);        // Enables Depth Testing
                        GL.Enable(EnableCap.AlphaTest);       // Enables Alpha Testing
                        GL.ShadeModel(ShadingModel.Smooth);    // Enables Smooth Color Shading*/
                        
                        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 100.0f);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref projection);
                        
                        GL.MatrixMode(MatrixMode.Modelview);
                
                }
                void HandleKeyPressed ()
                {
                        if (Keyboard[Key.Right]){
                                yrot += 5.00f;
                        }
                        if (Keyboard[Key.Left]){
                                yrot -= 5.00f;
                        }
                        if (Keyboard[Key.Up]){
                                xrot += 5.00f;
                        }
                        if (Keyboard[Key.Down]){
                                xrot -= 5.00f;
                        }
                        
                        
                }
        }
}


