#region File Description
//-----------------------------------------------------------------------------
// SpinningTriangleControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
#endregion

namespace ShaderStudio
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    class SceneControl : GraphicsDeviceControl
    {
        Effect effect;
        RenderTarget2D shadowMapA;
        RenderTarget2D shadowMapB;
        bool shadowMapFlip = false;
        Texture2D texture;
        //SpriteBatch sb;

        static float planeReflectivity = 0;
        static float triangleReflectivity = .3f;
        static float cubeReflectivity = .2f;

        static Vec2 TexX = new Vec2(0, 0);
        static Vec2 TexY = new Vec2(1, 0);
        static Vec2 TexZ = new Vec2(1, 1);
        static Vec2 TexW = new Vec2(0, 1);

        //Vertex positions and colors used to display a cube.
        public readonly LitTextureVertex[] CubeVertices =
        {
            new LitTextureVertex(Vec3.NNN * .5, Vec3.NNN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPP * .5, Vec3.NPP, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPN * .5, Vec3.NPN, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNN * .5, Vec3.NNN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNP * .5, Vec3.NNP, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPP * .5, Vec3.NPP, TexX, 0, cubeReflectivity),

            new LitTextureVertex(Vec3.PNP * .5, Vec3.PNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNN * .5, Vec3.NNN, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNN * .5, Vec3.PNN, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNP * .5, Vec3.PNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNP * .5, Vec3.NNP, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNN * .5, Vec3.NNN, TexX, 0, cubeReflectivity),

            new LitTextureVertex(Vec3.PNN * .5, Vec3.PNN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPN * .5, Vec3.NPN, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPN * .5, Vec3.PPN, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNN * .5, Vec3.PNN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNN * .5, Vec3.NNN, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPN * .5, Vec3.NPN, TexX, 0, cubeReflectivity),

            new LitTextureVertex(Vec3.NNP * .5, Vec3.NNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPP * .5, Vec3.PPP, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPP * .5, Vec3.NPP, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NNP * .5, Vec3.NNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNP * .5, Vec3.PNP, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPP * .5, Vec3.PPP, TexX, 0, cubeReflectivity),

            new LitTextureVertex(Vec3.PNP * .5, Vec3.PNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPN * .5, Vec3.PPN, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPP * .5, Vec3.PPP, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNP * .5, Vec3.PNP, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PNN * .5, Vec3.PNN, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPN * .5, Vec3.PPN, TexX, 0, cubeReflectivity),
        
            new LitTextureVertex(Vec3.PPN * .5, Vec3.PPN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPP * .5, Vec3.NPP, TexX, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPP * .5, Vec3.PPP, TexY, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.PPN * .5, Vec3.PPN, TexZ, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPN * .5, Vec3.NPN, TexW, 0, cubeReflectivity),
            new LitTextureVertex(Vec3.NPP * .5, Vec3.NPP, TexX, 0, cubeReflectivity),
        };

        // Vertex positions and colors used to display a spinning triangle.
        public readonly LitTextureVertex[] TriangleVertices =
        {
            new LitTextureVertex(new Vec3( 0,  1, 0), new Vec3(0, 0, -1), new Vec2(1, 1), 0, triangleReflectivity), //2
            new LitTextureVertex(new Vec3( 1, -1, 0), new Vec3(0, 0, -1), new Vec2(1, 0), 0, triangleReflectivity), //1
            new LitTextureVertex(new Vec3(-1, -1, 0), new Vec3(0, 0, -1), new Vec2(0, 0), 0, triangleReflectivity), //0

            new LitTextureVertex(new Vec3(-1, -1, 1), new Vec3(-1, 1, 0), new Vec2(0, 0), 0, triangleReflectivity), //3
            new LitTextureVertex(new Vec3( 0,  1, 0), new Vec3(-1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //2
            new LitTextureVertex(new Vec3(-1, -1, 0), new Vec3(-1, 1, 0), new Vec2(0, 0), 0, triangleReflectivity), //0
            
            new LitTextureVertex(new Vec3( 0,  1, 0), new Vec3(-1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //2
            new LitTextureVertex(new Vec3(-1, -1, 1), new Vec3(-1, 1, 0), new Vec2(0, 0), 0, triangleReflectivity), //3
            new LitTextureVertex(new Vec3( 0,  1, 1), new Vec3(-1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //5
            
            new LitTextureVertex(new Vec3( 0,  1, 0), new Vec3(1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //2
            new LitTextureVertex(new Vec3( 1, -1, 1), new Vec3(1, 1, 0), new Vec2(1, 0), 0, triangleReflectivity), //4
            new LitTextureVertex(new Vec3( 1, -1, 0), new Vec3(1, 1, 0), new Vec2(1, 0), 0, triangleReflectivity), //1
            
            new LitTextureVertex(new Vec3( 0,  1, 1), new Vec3(1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //5
            new LitTextureVertex(new Vec3( 1, -1, 1), new Vec3(1, 1, 0), new Vec2(1, 0), 0, triangleReflectivity), //4
            new LitTextureVertex(new Vec3( 0,  1, 0), new Vec3(1, 1, 0), new Vec2(1, 1), 0, triangleReflectivity), //2
            
            new LitTextureVertex(new Vec3( 1, -1, 0), new Vec3(0, -1, 0), new Vec2(1, 0), 0, triangleReflectivity), //1
            new LitTextureVertex(new Vec3( 1, -1, 1), new Vec3(0, -1, 0), new Vec2(1, 0), 0, triangleReflectivity), //4
            new LitTextureVertex(new Vec3(-1, -1, 0), new Vec3(0, -1, 0), new Vec2(0, 0), 0, triangleReflectivity), //0
            
            new LitTextureVertex(new Vec3(-1, -1, 1), new Vec3(0, -1, 0), new Vec2(0, 0), 0, triangleReflectivity), //3
            new LitTextureVertex(new Vec3(-1, -1, 0), new Vec3(0, -1, 0), new Vec2(0, 0), 0, triangleReflectivity), //0
            new LitTextureVertex(new Vec3( 1, -1, 1), new Vec3(0, -1, 0), new Vec2(1, 0), 0, triangleReflectivity), //4

            new LitTextureVertex(new Vec3(-1, -1, 1), new Vec3(0, 0, 1), new Vec2(0, 0), 0, triangleReflectivity), //3
            new LitTextureVertex(new Vec3( 1, -1, 1), new Vec3(0, 0, 1), new Vec2(1, 0), 0, triangleReflectivity), //4
            new LitTextureVertex(new Vec3( 0,  1, 1), new Vec3(0, 0, 1), new Vec2(1, 1), 0, triangleReflectivity), //5
        };

        // Vertex positions and colors used to display a plane
        public readonly LitTextureVertex[] PlaneVertices =
        {
            new LitTextureVertex(new Vec3(-5,  10, 10), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //3
            new LitTextureVertex(new Vec3( 5,  10, 10), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //4
            new LitTextureVertex(new Vec3(-5, -10,  0), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //1
            new LitTextureVertex(new Vec3( 5, -10,  0), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //2
            new LitTextureVertex(new Vec3(-5, -10,  0), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //1
            new LitTextureVertex(new Vec3( 5,  10, 10), new Vec3(0, 0, -1), new Vec2(0, 1), 0, planeReflectivity), //4 
           
            new LitTextureVertex(new Vec3(-5, -10,  0.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //1
            new LitTextureVertex(new Vec3( 5,  10, 10.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //4
            new LitTextureVertex(new Vec3(-5,  10, 10.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //3
            new LitTextureVertex(new Vec3( 5,  10, 10.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //4
            new LitTextureVertex(new Vec3(-5, -10,  0.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //1
            new LitTextureVertex(new Vec3( 5, -10,  0.1f), new Vec3(0, 0, 1), new Vec2(0, 1), 0, planeReflectivity), //2
        };

        public Color ClearColor { get; set; }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Create our effect.
            shadowMapA = new RenderTarget2D(GraphicsDevice, 800, 600, false, SurfaceFormat.Single, DepthFormat.Depth24);
            shadowMapB = new RenderTarget2D(GraphicsDevice, 800, 600, false, SurfaceFormat.Single, DepthFormat.Depth24);
            FileStream fs = File.OpenRead("Data/Shader.mgfx");
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = new byte[fs.Length];
            br.Read(bytes, 0, (int)fs.Length);
            br.Close();
            effect = new Effect(GraphicsDevice, bytes);
            FileStream tx = File.OpenRead("Data/Texture.png");
            texture = Texture2D.FromStream(GraphicsDevice, tx);
            tx.Close();
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
            //sb = new SpriteBatch(GraphicsDevice);
        }


        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Textures[0] = texture;
            GraphicsDevice.SamplerStates[0] = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Point };
            GraphicsDevice.Textures[1] = shadowMapA;
            GraphicsDevice.SamplerStates[1] = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Point };
            GraphicsDevice.Textures[2] = shadowMapB;
            GraphicsDevice.SamplerStates[2] = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Point };
            GraphicsDevice.Clear(ClearColor);

            // Set transform matrices.
            float aspect = GraphicsDevice.Viewport.AspectRatio;
            Matrix view = Matrix.CreateLookAt(Globals.CameraPosition, Vec3.Zero, Vec3.Up);
            Matrix perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, .1f, 5000);
            Matrix translation = Matrix.CreateTranslation(Globals.TrianglePosition);
            Matrix spin = Matrix.CreateFromYawPitchRoll(Globals.Rotation.Y, Globals.Rotation.X, Globals.Rotation.Z);
            Matrix triangleWorld = Matrix.Multiply(Matrix.Multiply(spin, translation), Matrix.Identity);
            Matrix lightView = Matrix.CreateLookAt(Globals.LightPosition, new Vec3(0, 0, 1), Vec3.Up);
            Matrix lightPerspective = Matrix.CreatePerspectiveFieldOfView(1, 1, 1, 30);

            // Set renderstates.
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.DepthStencilState.DepthBufferFunction = CompareFunction.Less;

            GraphicsDevice.BlendState = BlendState.Opaque;

            //Set effect parameters
            //effect.Parameters["ShadowDepthBias"].SetValue(.0007f);
            effect.Parameters["CameraPosition"].SetValue(Globals.CameraPosition);
            //effect.Parameters["LightPosition"].SetValue(Globals.LightPosition);

            //Preprocessing
            if (!shadowMapFlip)
            {
                GraphicsDevice.SetRenderTarget(shadowMapA);
                shadowMapFlip = true;
            }
            else
            {
                GraphicsDevice.SetRenderTarget(shadowMapB);
                shadowMapFlip = false;
            }
            GraphicsDevice.Clear(Color.Black);
            effect.Parameters["mW"].SetValue(Matrix.Identity);
            effect.Parameters["mWL"].SetValue(lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(view * perspective);
            effect.Techniques["Pre"].Passes["Depth"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, PlaneVertices, 0, 4, LitTextureVertex.VertexDeclaration); //plane
            effect.Parameters["mW"].SetValue(triangleWorld);
            effect.Parameters["mWL"].SetValue(triangleWorld * lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(triangleWorld * view * perspective);
            effect.Techniques["Pre"].Passes["Depth"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, TriangleVertices, 0, 8, LitTextureVertex.VertexDeclaration); //triangle
            effect.Parameters["mW"].SetValue(spin);
            effect.Parameters["mWL"].SetValue(spin * lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(spin * view * perspective);
            effect.Techniques["Pre"].Passes["Depth"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CubeVertices, 0, 36, LitTextureVertex.VertexDeclaration); //cube

            //Rendering
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            effect.Parameters["mW"].SetValue(Matrix.Identity);
            effect.Parameters["mWL"].SetValue(lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(view * perspective);
            effect.Techniques["Render"].Passes["Opaque"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, PlaneVertices, 0, 4, LitTextureVertex.VertexDeclaration); //plane
            effect.Parameters["mW"].SetValue(triangleWorld);
            effect.Parameters["mWL"].SetValue(triangleWorld * lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(triangleWorld * view * perspective);
            effect.Techniques["Render"].Passes["Opaque"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, TriangleVertices, 0, 8, LitTextureVertex.VertexDeclaration); //triangle
            effect.Parameters["mW"].SetValue(spin);
            effect.Parameters["mWL"].SetValue(spin * lightView * lightPerspective);
            effect.Parameters["mWVP"].SetValue(spin * view * perspective);
            effect.Techniques["Render"].Passes["Opaque"].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, CubeVertices, 0, 36, LitTextureVertex.VertexDeclaration); //cube

            /*sb.Begin();
            GraphicsDevice.BlendState = BlendState.Opaque;
            sb.Draw(shadowMap, new Rectangle(0, 0, 256, 256), Color.White);
            sb.End();*/
        }
    }
}
