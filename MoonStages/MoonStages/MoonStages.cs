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
		bool light = true;              // lighting on/off
		bool visao = false;
        //float rotation = 0.0f;
        float xrot  = 0.0f;
        float yrot  = 0.0f;
		int[] texture = new int[3];								// Storage for 3 textures.int[]
		
		float Xpos = 0.0f;
		float Ypos = 0.0f;
		float Zpos = 0.0f;
		
		float[] LightAmbient =  { 0.2f, 0.2f, 0.2f, 1.0f };     // white ambient light at half intensity (rgba)                 
        float[] LightPosition = { 0.0f, 0.0f, 0.0f, 1.0f };     // position of light (x, y, z, (position of light)) 
		
		float[] LightColorS = { 1.0f, 1.0f, 0.0f, 1.0f };
		float[] LightColorB = { 0.0f, 0.0f, 0.0f, 1.0f };
		
		Matrix4 projection1;
		
		//Atualiza a tela quando maximiza e minimiza a mesma
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
			
			GL.Viewport(
				ClientRectangle.X, 
				ClientRectangle.Y,
				ClientRectangle.Width, 
				ClientRectangle.Height
			);
			GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
										(float)Math.PI / 4,
										Width / (float)Height, 
										1.0f, 
										100.0f
									);
			GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
			
        }
        
		//Desenha a cada tempo
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
			//Atualiza a cada tempo o frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);	//Limpa a GL
            GL.LoadIdentity();
			
			//Olho para a lua
			if(visao){
				projection1 = Matrix4.LookAt(
					13.5f, 0.0f, -50.0f,
					-2.0f, 0.0f, -50.0f,
				 	 0.0f, 1.0f,   0.0f
				);
				GL.LoadMatrix(ref projection1);
				//GL.Rotate(day, 1.0f, 0.0f, -50.0f);
			}
            
            // scale square
            GL.Translate(0.0f, 0.0f, -50.0f);	//Move para frente
            GL.Rotate (xrot, 1.0f, 0.0f, 0.0f); //Move Left and Right
            GL.Rotate (yrot, 0.0f, 1.0f, 0.0f); //Move Up and down
			GL.Translate(Xpos, Ypos, Zpos);		//Move ASDW
            
			// Draw objects in the scene
            DrawModels();
            
            this.SwapBuffers();
        }
		
		//"Monitora"
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
			//Console.WriteLine("Day = "+day+" and year = "+year);
			//Console.WriteLine("X = "+Xpos+"; Y = "+Ypos+"; Z = "+Zpos+"; rotX = "+xrot+"; rotY = "+yrot);
            HandleKeyPressed ();
            day = (day + 0.5f);
            year = (year + 0.00137f);
            if (Keyboard[Key.Escape])
            	Exit();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
			LoadGLTextures();							// Load textures
            
			GL.ClearColor(0.0f, 0.0f, 0.0f, 0.5f);		// Black Background
			//Hidden surface removal
			GL.Enable(EnableCap.DepthTest);				// Enables Depth Testing
			//GL.DepthFunc(DepthFunction.Lequal);			// The Type Of Depth Testing To Do
			GL.DepthFunc(DepthFunction.Less);			// The Type Of Depth Testing To Do
	
			//Some functions
            GL.Enable(EnableCap.Texture2D);				// Enable textures
            GL.ClearDepth(1.0);							// Depth Buffer Setup
            // Really Nice Perspective Calculations
            GL.Hint (
				HintTarget.PerspectiveCorrectionHint,                  
            	HintMode.Nicest
			);											
    		
			// Set up some lighting state that never changes
			GL.ShadeModel(ShadingModel.Smooth);			// Enable Smooth Shading
			GL.Enable(EnableCap.Light0);
    		GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			
			// set up light number 1.
			GL.LightModel(LightModelParameter.LightModelAmbient, LightAmbient);
            //GL.Light (LightName.Light1, LightParameter.Ambient, LightAmbient);      // add lighting. (ambient)

        }
		
		#region DrawModels()

        void DrawModels()
        {
			//Sun
			GL.Light (LightName.Light0, LightParameter.Position, LightPosition);
            GL.Material(
					MaterialFace.Front, 
					MaterialParameter.Emission, 
					LightColorS
			);
            GL.BindTexture(TextureTarget.Texture2D, texture[0]);					//Adiciona textura
			GL.Rotate(day, 0.0, 1.0, 0.0);											//Em torno dela 
            DrawSphere(new Vector3(0.0f, 0.0f, 0.0f), 1.75f, 100);					//Desenha o sol
			
			//Gera material escuro para outros planetas
			GL.Material(
					MaterialFace.Front, 
					MaterialParameter.Emission, 
					LightColorB
			);
			
            //earth
            GL.PushMatrix();
				GL.Rotate(year, 0.0, 1.0, 0.0);									//Em torno do sol - Earth
				GL.Translate( 14.0f, 0.0f, 0.0f);								//Move para a posição da terra
				GL.BindTexture(TextureTarget.Texture2D, texture[1]);			//Adiciona textura - Earth
            	GL.PushMatrix();
					GL.Rotate(day, 0.0, 1.0, 0.0);								//Em torno dela mesma - Earth
					DrawSphere(new Vector3(0.0f, 0.0f, 0.0f), 0.25f, 16);		//Earth
					GL.PushMatrix();
						GL.Rotate(year, 0.0, 1.0, 0.0);							//Em torno da terra - Moon
						GL.Translate(-2.0f, 0.0f, 0.0f); 						//Move para a posição da lua
            			GL.PushMatrix();
							GL.Rotate(day, 0.0, 1.0, 0.0);						//Em torno dele mesmo - Moon
							GL.BindTexture(TextureTarget.Texture2D, texture[2]);		//Moon
            				DrawSphere(new Vector3(0.0f, 0.0f, 0.0f), 0.0875f, 16);		//Moon
            			GL.PopMatrix();
            		GL.PopMatrix();
            	GL.PopMatrix();
            GL.PopMatrix();
            
		}
		
		#endregion
	
        public MoonStages()
        : base(640, 480, GraphicsMode.Default, "MoonStages")
        {

            VSync = VSyncMode.On;
			
			Keyboard.KeyDown += (object sender, KeyboardKeyEventArgs e) => {
                switch (e.Key) {
					case Key.Escape :
                        Exit ();
                        break;
                	case Key.F1:
                        if (WindowState == WindowState.Fullscreen) 
                                WindowState = WindowState.Normal;
                        else 
                                WindowState = WindowState.Fullscreen;
                        break;                                        
                	case Key.L :
                        light = !light;
                        // switch the current value of light, between 0 and 1.
                        Console.WriteLine ("Light is now: {0}", light);
                        if (!light) 
                                GL.Disable (EnableCap.Lighting);
                        else 
                                GL.Enable (EnableCap.Lighting);
                        break;
					case Key.V :
						visao = !visao;
						Console.WriteLine ("Vision is now: {0}", visao);
						break;
                }
				Console.WriteLine("Day = "+day+" and year = "+year);
				Console.WriteLine("X = "+Xpos+"; Y = "+Ypos+"; Z = "+Zpos+"; rotX = "+xrot+"; rotY = "+yrot);
        	};
        }
        
        void HandleKeyPressed ()
        {
            if (Keyboard[Key.Right]){
                    yrot += 0.4f;
            }
            if (Keyboard[Key.Left]){
                    yrot -= 0.4f;
            }
            if (Keyboard[Key.Up]){
                    xrot += 0.4f;
            }
            if (Keyboard[Key.Down]){
                    xrot -= 0.4f;
            }

			if(Keyboard[Key.W]){
				Zpos += 0.4f;
            }
			if(Keyboard[Key.S]){
				Zpos -= 0.4f;
            }
			if(Keyboard[Key.A]){
				Xpos += 0.4f;
            }
			if(Keyboard[Key.D]){
				Xpos -= 0.4f;
            }
			
			if(Keyboard[Key.I]){
				Ypos += 0.4f;
			}
			if(Keyboard[Key.K]){
				Ypos -= 0.4f;
			}	
                
        }
		
		#region LoadGLTextures
		
		void LoadGLTextures ()
        {
                
                GL.GenTextures (3, texture);                            // Create Texture
				string file = "";
				for (int i = 0; i < 3; i++) {
					if(i == 0)
						file = "textures/sol.png";
					else if(i == 1)
						file = "textures/terra.png";
					else {
						file = "textures/lua.png";
					}
					if (!System.IO.File.Exists (file)) {
                        throw new System.IO.FileNotFoundException ();
                	}
                
                    Bitmap image = new Bitmap (file);                       // Load Texture
                    image.RotateFlip (RotateFlipType.RotateNoneFlipY);      // Flip The Bitmap Along The Y-Axis
                    Rectangle rectangle = new Rectangle (0, 0,              // Rectangle For Locking The Bitmap In Memory
                                                         image.Width, 
                                                         image.Height);
                    
                    BitmapData bitmapData = image.LockBits (rectangle,      // Get The Bitmap's Pixel Data From The Locked Bitmap
                                                            ImageLockMode.ReadOnly, 
                                                            PFormat.Format32bppArgb);
                    
                    
                    // Create Nearest Filtered Texture
                    // cheap scaling when image bigger than texture
                    GL.BindTexture (TextureTarget.Texture2D, texture[i]);
                    GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                                     (int)TextureMinFilter.Nearest);
                    GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                                     (int)TextureMinFilter.Nearest);
//                    GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 
//                                   image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, 
//                                   PixelType.UnsignedByte, bitmapData.Scan0);
                    
                    // Requieres OpenGL >= 1.4
                    GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                                   image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
                                   PixelType.UnsignedByte, bitmapData.Scan0);
                    GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);
                    
                    // Unlock The Pixel Data From Memory
                    image.UnlockBits (bitmapData);
                    // Dispose The Bitmap
                    image.Dispose ();
			}
        }
		
		#endregion
        
		#region DrawSphere
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
		#endregion
	
	}
}



