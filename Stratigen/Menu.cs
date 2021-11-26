using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
using Stratigen.Datatypes;
using Stratigen.Framework;
using Stratigen.Libraries;
using Model = Stratigen.Datatypes.Model;
using Plane = Microsoft.Xna.Framework.Plane;
using Ray = Microsoft.Xna.Framework.Ray;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework.Media;

namespace Stratigen
{
    public class Menu : Game
    {
        public DateTime VirtualTime = new DateTime(1, 1, 1, 7, 0, 0);
        MouseState previousMouseState;
        RenderTarget2D renderBuffer;
        RenderTarget2D shadowBuffer;
        RenderTarget2D depthBuffer;
        ///RenderTarget2D nearShadowMap;
        RenderTarget2D shadowMap;
        ///RenderTarget2D farShadowMap;
        Texture2D map;
        Vec2 mouseRotationBuffer = Vec2.Zero;
        Random random;
        GraphicsDeviceManager gdm;
        Camera camera;
        Scene scene;
        RasterizerState rasterizerState;
        DepthStencilState depthStencilState;
        SamplerState samplerStateClampPoint;
        SamplerState samplerStateClampLinear;
        SamplerState samplerStateClampAniso;
        //SamplerState randomSamplerState;
        BlendState blendState;
        SpriteBatch spriteBatch;
        Effect effect;
        Effect shadowMapper;
        double seed;
        Font consolas;
        int frameRate = 0;
        int frameCounter = 0;
        int memUsage = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        float POS89RADS;
        float NEG89RADS;
        Texture2D blockTextures;
        TimeSpan airTime = TimeSpan.Zero;
        Vec3 initialPosition = Vec3.Zero;
        BasicEffect colorEffect;
        Vec3 selectionPoint = Vec3.Zero;
        Ray pickRay = new Ray(Vec3.Zero, Vec3.Zero);
        TimeSpan lastInteraction = TimeSpan.Zero;
        Inventory inv = new Inventory(4, 4);
        List<Block> surroundingBlocks = new List<Block>();
        Texture2D crosshairTexture;
        Vec3 pickNormal = new Vec3(0, 0, 0);
        List<Chunk> surroundingChunks = new List<Chunk>();
        List<int> FPSStack = new List<int>(capacity: 200);
        DateTime startTime;
        public int threadCount = 0;
        public List<Vec2> ChunkPositionsToGenerate = new List<Vec2>();
        public Thread ChunkGenerationThread;
        public List<Thread> ChildThreads = new List<Thread>();
        Color skyColor = new Color(75, 100, 0); //red and green locked at 3:4 ratio, blue adjusts based on pitch of camera to be bluer when looking higher up and whiter when looking further down
        Vec2 lastChunk = Vec2.Zero;
        Vec3 sunPosition = new Vec3(-100, 600, -100);
        float phase = 0;
        Quad quad = new Quad();
        BoundingFrustum lightFrustum;
        bool wireframe = false;
        Rectangle playButton = new Rectangle(40, 65, 60, 20);
        float sunlight = 1;

        public Menu()
        {
            startTime = DateTime.Now;
            random = new Random();
            Window.Title = Console.Title = "Stratigen Menu"; 
            Console.Write("Initializing \"" + Window.Title + "\"... ");
            Console.CursorVisible = false;
            gdm = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Globals.Width,
                PreferredBackBufferHeight = Globals.Height,
                //PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false,
            };
            System.Reflection.Assembly mono = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game));
            IEnumerable<string> monoReferencedAssemblies = mono.GetReferencedAssemblies().Select(an => an.Name);
            if (monoReferencedAssemblies.Contains("OpenTK")) Platform.API = Platform.APIs.OpenTK;
            else if (monoReferencedAssemblies.Contains("SharpDX")) Platform.API = Platform.APIs.SharpDX;
            else Console.WriteLine("Unknown API - might crash..?");
            seed = 1;//random.NextDouble();
            Content.RootDirectory = "Data";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            consolas = new Font(Content.RootDirectory + "/Fonts/Consolas-36.xml");
            if (Platform.API == Platform.APIs.OpenTK)
            {
                effect = Content.Load<Effect>("Shaders/SM2Blocks.mgfx");
                shadowMapper = Content.Load<Effect>("Shaders/SM2ShadowMap.mgfx");
            }
            else if (Platform.API == Platform.APIs.SharpDX)
            {
                effect = Content.Load<Effect>("Shaders/SM4Blocks.mgfx");
                shadowMapper = Content.Load<Effect>("Shaders/SM4ShadowMap.mgfx");
            }
            blockTextures = Content.Load<Texture2D>("Textures/Blocks.png");
            crosshairTexture = Content.Load<Texture2D>("Textures/Crosshair.png");
            base.LoadContent();
        }

        protected override void Initialize()
        {
            //monoGamePlatform = MonoGameContentProcessors.ContentHelper.GetMonoGamePlatform();
            POS89RADS = MathHelper.ToRadians(89);
            NEG89RADS = MathHelper.ToRadians(-89);
            Globals.GraphicsDevice = GraphicsDevice;
            //for (int i = 0; i < 200; i++) FPSStack.Insert(0, gdm.PreferredBackBufferHeight - 60);
            XNA.Initialize2D(GraphicsDevice);
            colorEffect = new BasicEffect(GraphicsDevice);
            colorEffect.TextureEnabled = false;
            colorEffect.VertexColorEnabled = true;
            colorEffect.LightingEnabled = false;
            scene = new Scene(GraphicsDevice, camera = new Camera(this) { FOV = MathHelper.PiOver4, Aspect = Globals.Width/Globals.Height, NearClip = .05f, FarClip = 1000f });
            //shadowCamera = new ShadowCamera(this) { FOV = MathHelper.PiOver4, Aspect = 1.33f, NearClip = .05f, FarClip = 10000f };
            //shadowCamera.Rotation = new Vec3(2.2f, 0, 0);
            //shadowMap = new RenderTarget2D(GraphicsDevice, gdm.PreferredBackBufferWidth / 4, gdm.PreferredBackBufferHeight / 4, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            renderBuffer = new RenderTarget2D(GraphicsDevice, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            shadowBuffer = new RenderTarget2D(GraphicsDevice, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);
            depthBuffer = new RenderTarget2D(GraphicsDevice, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            int smRes = 2048;
            shadowMap = new RenderTarget2D(GraphicsDevice, smRes, smRes, false, SurfaceFormat.Single, DepthFormat.Depth24); //shadowmap size is key to the flickering, as well as blurs/etc. that enhance at lower res
            ///nearShadowMap = new RenderTarget2D(GraphicsDevice, smRes * 2, smRes * 2, false, SurfaceFormat.Single, DepthFormat.Depth24);
            ///farShadowMap = new RenderTarget2D(GraphicsDevice, smRes / 2, smRes / 2, false, SurfaceFormat.Single, DepthFormat.Depth24); 
            //shadowMapB = new RenderTarget2D(GraphicsDevice, smRes, smRes, false, SurfaceFormat.Single, DepthFormat.Depth24); //shadowmap size is key to the flickering, as well as blurs/etc. that enhance at lower res
            map = new Texture2D(GraphicsDevice, 48, 48);
            //map.SetColor(Color.Red);
            //randomTexture = new Texture2D(GraphicsDevice, 16, 16);
            #region Generate World
            scene.World = new World();
            scene.World.Seed = seed;
            scene.World.GenerateChunk(0, 0, seed);
            scene.World[0, 0].Build();
            scene.World[0, 0].Menu = true;
            scene.Menu = true;
            scene.World.LoadedChunks = new List<Chunk> { scene.World[0, 0] };
            #endregion

            camera.Position = initialPosition = new Vec3(26, 120, 26); 
            spriteBatch = new SpriteBatch(GraphicsDevice);

            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };//, MultiSampleAntiAlias = true };
            depthStencilState = new DepthStencilState { DepthBufferFunction = CompareFunction.LessEqual };
            samplerStateClampPoint = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Point };
            samplerStateClampLinear = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Linear };
            samplerStateClampAniso = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Anisotropic, MaxAnisotropy = 16 };
            blendState = BlendState.Opaque;

            camera.Rotation = new Vec3(.44f, 3.93f, 0);

            base.Initialize();
            Console.Write("Finished (" + Math.Round((DateTime.Now - startTime).TotalMilliseconds, 2) + "ms).\n");
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.Textures[0] = blockTextures;
            GraphicsDevice.SamplerStates[0] = samplerStateClampPoint;
            GraphicsDevice.Textures[1] = renderBuffer;
            GraphicsDevice.SamplerStates[1] = samplerStateClampPoint;
            GraphicsDevice.Textures[2] = shadowMap;
            GraphicsDevice.SamplerStates[2] = samplerStateClampAniso;
            GraphicsDevice.Textures[3] = shadowBuffer;
            GraphicsDevice.SamplerStates[3] = samplerStateClampAniso;
            GraphicsDevice.Textures[4] = depthBuffer;
            GraphicsDevice.SamplerStates[4] = samplerStateClampLinear;
            /*GraphicsDevice.Textures[5] = nearShadowMap;
            GraphicsDevice.SamplerStates[5] = samplerStateClampAniso;
            GraphicsDevice.Textures[6] = farShadowMap;
            GraphicsDevice.SamplerStates[6] = samplerStateClampAniso;*/
            //Dynamic sky color based on angle of observation
            int val = (int)Maths.RadToDeg(camera.Pitch);
            skyColor.B = (byte)Maths.Clamp(160 - val, 140, 200);
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = depthStencilState;
            GraphicsDevice.BlendState = blendState;
            float shadowNear = .1f;
            float shadowFar = 1600;
            effect.Parameters["mW"].SetValue(Matrix.Identity);
            effect.Parameters["mWVP"].SetValue(camera.View * camera.Perspective);
            effect.Parameters["LightPosition"].SetValue(sunPosition);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);
            effect.Parameters["WaveAmplitude"].SetValue(.25f);
            effect.Parameters["WaveFrequency"].SetValue(30f);
            effect.Parameters["Phase"].SetValue(phase);
            //effect.Parameters["ChromaKey"].SetValue(Color.Magenta.ToVector4());
            //(100^(sin((x+.5)/8)))/100)
            effect.Parameters["Sunlight"].SetValue(sunlight = (float)Math.Pow(100, Math.Sin((VirtualTime.Hour + .5) / 8)) / 100);
            effect.Parameters["SkyColor"].SetValue((skyColor * sunlight).ToVector4());
            //effect.Parameters["FarClip"].SetValue(camera.FarClip);
            Matrix lightPerspective = Matrix.CreatePerspectiveFieldOfView(1, 1, shadowNear, shadowFar);
            //the 64 below is based on the size of a chunk and the number of chunks of view distance, roughly 16x16 chunk of 4 view distance is around 64 blocks, and 1 block per unit of distance
            Matrix lightView = Matrix.CreateLookAt(sunPosition, new Vec3((float)Math.Floor(camera.Position.X / 64) * 64, 255, (float)Math.Floor(camera.Position.Z / 64) * 64), Vec3.Up) * lightPerspective;
            effect.Parameters["mWL"].SetValue(lightView);
            colorEffect.World = Matrix.Identity;
            colorEffect.View = camera.View;
            colorEffect.Projection = camera.Perspective;
            lightFrustum = new BoundingFrustum(lightView);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            #region Texture Vertices
            /*GraphicsDevice.SetRenderTarget(nearShadowMap);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            effect.Techniques["Pre"].Passes["LightDepth"].Apply(); //gather info for shadows
            scene.Draw(Chunk.Layer.Opaque, camera.NearFrustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            scene.Draw(Chunk.Layer.Transparent, camera.NearFrustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            */
            GraphicsDevice.SetRenderTarget(shadowMap);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            effect.Techniques["Pre"].Passes["LightDepth"].Apply(); //gather info for shadows
            scene.Draw(Chunk.Layer.Opaque, camera.Frustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            scene.Draw(Chunk.Layer.Transparent, camera.Frustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            /*
            GraphicsDevice.SetRenderTarget(farShadowMap);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            effect.Techniques["Pre"].Passes["LightDepth"].Apply(); //gather info for shadows
            scene.Draw(Chunk.Layer.Opaque, camera.FarFrustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            scene.Draw(Chunk.Layer.Transparent, camera.FarFrustum, sunPosition, gameTime); //cast shadows //was set to use lightFrustum but since we're talking abuot culling chunks, camera frustum is more efficient with no apparent difference in quality
            */
            GraphicsDevice.SetRenderTarget(renderBuffer);
            if (wireframe) rasterizerState.FillMode = FillMode.WireFrame;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, skyColor * sunlight, 1f, 0); //was sky color
            effect.Techniques["Render"].Passes["Opaque"].Apply(); //draw solid
            scene.Draw(Chunk.Layer.Opaque, camera.Frustum, camera.Position, gameTime);

            effect.Techniques["Render"].Passes["Transparent"].Apply(); //draw transparent
            scene.Draw(Chunk.Layer.Transparent, camera.Frustum, camera.Position, gameTime);

            effect.Techniques["Render"].Passes["Liquid"].Apply(); //draw liquid
            scene.Draw(Chunk.Layer.Liquid, camera.Frustum, camera.Position, gameTime);

            rasterizerState.FillMode = FillMode.Solid;

            GraphicsDevice.SetRenderTarget(shadowBuffer);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
            effect.Techniques["Render"].Passes["Shadow"].Apply(); //draw shadows
            scene.Draw(Chunk.Layer.Opaque, camera.Frustum, camera.Position, gameTime); //catch shadows
            scene.Draw(Chunk.Layer.Transparent, camera.Frustum, camera.Position, gameTime); //catch shadows
            /*effect.Techniques["Post"].Passes["ShadowBlur"].Apply();
            GraphicsDevice.DrawUserPrimitives<TextureVertex>(PrimitiveType.TriangleList, quad.Vertices, 0, 2); */

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, skyColor * sunlight, 1f, 0);
            effect.Techniques["Post"].Passes["Lighting"].Apply(); //apply screen-space post FX to buffer
            GraphicsDevice.DrawUserPrimitives<TextureVertex>(PrimitiveType.TriangleList, quad.Vertices, 0, 2); //draw buffer to screen

            #endregion

            #region Color Vertices
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            foreach (EffectPass ep in colorEffect.CurrentTechnique.Passes)
            {
                ep.Apply(); 
                //GraphicsDevice.DrawSun(new Vec3(-100, 600, -100), 50, Color.OldLace);
                GraphicsDevice.DrawSun(sunPosition, 50, Color.OldLace);
                //GraphicsDevice.DrawCube(new Vec3(camera.BoundingBox.Min.X, camera.BoundingBox.Min.Y + .001f, camera.BoundingBox.Min.Z), camera.BoundingBox.Max, Color.Yellow); //draw the camera's bounding box for debug purposes
                Vec2 center = new Vec2(Window.ClientBounds.Width / 2 + Window.ClientBounds.X, Window.ClientBounds.Height / 2 + Window.ClientBounds.Y);
                pickRay = camera.UnprojectPoint(center.ToXNA(), GraphicsDevice.Viewport);
                //GraphicsDevice.DrawLine(camera.Position, camera.Position + (Vec3)r.Direction * 5, Col3.Red); //Draw Selection Ray
                selectionPoint = (Vec3)pickRay.Position + (Vec3)pickRay.Direction * camera.NearClip;
                //Axis Display
                /*GraphicsDevice.DrawLine(selectionPoint - new Vec3(.01f, 0, 0), selectionPoint + new Vec3(.01f, 0, 0), Color.Red); //X (Red)
                GraphicsDevice.DrawLine(selectionPoint - new Vec3(0, .01f, 0), selectionPoint + new Vec3(0, .01f, 0), Color.Green); //Y (Green)
                GraphicsDevice.DrawLine(selectionPoint - new Vec3(0, 0, .01f), selectionPoint + new Vec3(0, 0, .01f), Color.Blue); //Z (Blue)*/
                //Draw Box Around Selected Block
                /*if (selectedBlock != null)
                    GraphicsDevice.DrawCube(selectedBlock.BoundingBox.Min, selectedBlock.BoundingBox.Max, Color.White);*/
                /*if (block != null)
                {
                    //Draw downrays to the top face corners of the block under the player
                    GraphicsDevice.DrawLine(new Vec3(block.BoundingBox.Min.X, block.BoundingBox.Max.Y, block.BoundingBox.Min.Z), camera.DownRays[0].Position, Color.Green);
                    GraphicsDevice.DrawLine(new Vec3(block.BoundingBox.Min.X, block.BoundingBox.Max.Y, block.BoundingBox.Max.Z), camera.DownRays[1].Position, Color.Green);
                    GraphicsDevice.DrawLine(new Vec3(block.BoundingBox.Max.X, block.BoundingBox.Max.Y, block.BoundingBox.Min.Z), camera.DownRays[2].Position, Color.Green);
                    GraphicsDevice.DrawLine(block.BoundingBox.Max, camera.DownRays[3].Position, Color.Green);
                }*/
                /*if (Globals.FPSGraph)
                {
                    List<Vec2> linePoints = new List<Vec2>();
                    for (int i = 1; i < 200; i++) linePoints.Add(new Vec2(i, FPSStack[i]));
                    GraphicsDevice.DrawLine2(camera, linePoints, Color.White);
                }*/
                GraphicsDevice.DrawFilledRectangle(camera, new Vec2(40, 65), 100, 20, Color.White, Color.Black); //draw menu box
                //GraphicsDevice.DrawFilledRectangle(camera, new Vec2(Window.ClientBounds.Center.X - 56, Window.ClientBounds.Center.Y - 198), 110, 20, Color.Gray, Color.White); //button
                //GraphicsDevice.DrawFilledRectangle(camera, new Vec2(320, Window.ClientBounds.Height - 20), 323 + inv.Items.Keys.Count(i => i.IconPath != null) * 17, 20, Color.White, Color.Black); //draw middle rectangle for the block type/item hotbar
            }
            #endregion

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); //won't draw until the the batch is ended, supports alpha blending
            #region HUD
            //top right
            ////spriteBatch.Draw(renderBuffer, new Rectangle(gdm.PreferredBackBufferWidth - gdm.PreferredBackBufferWidth / 4, 0, gdm.PreferredBackBufferWidth / 4, gdm.PreferredBackBufferHeight / 4), Color.White);
            //bottom left
            //spriteBatch.Draw(shadowBufferClose, new Rectangle(0, gdm.PreferredBackBufferHeight - gdm.PreferredBackBufferHeight / 4, gdm.PreferredBackBufferWidth / 4, gdm.PreferredBackBufferHeight / 4), Color.White);
            //bottom right
            ////spriteBatch.Draw(shadowBuffer, new Rectangle(gdm.PreferredBackBufferWidth - gdm.PreferredBackBufferWidth / 4, gdm.PreferredBackBufferHeight - gdm.PreferredBackBufferHeight / 4, gdm.PreferredBackBufferWidth / 4, gdm.PreferredBackBufferHeight / 4), Color.White);
            //spriteBatch.Draw(map, new Rectangle(gdm.PreferredBackBufferWidth - 96, 0, 96, 96), Color.White);
            //spriteBatch.Draw(shadowMap, new Rectangle(gdm.PreferredBackBufferWidth - 256, gdm.PreferredBackBufferHeight - 256, 256, 256), Color.White);
            ////spriteBatch.DrawTexture(Vec2.Zero, depthBuffer);
            Coordinate cc = camera.Position.ToCoordinate();
            spriteBatch.DrawShadowedString(consolas,
                "Stratigen Menu (" + (Platform.OpenGL ? "GL" : Platform.DirectX ? "DX" : "?") + ") - " + frameRate + "FPS - " + memUsage + "MB", Vec2.Zero); //debug display
            spriteBatch.DrawShadowedString(consolas,
                "Play", new Vec2(55, 67));
            spriteBatch.DrawShadowedString(consolas, "Stratigen", new Vec2(10, 30));
            #endregion
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            base.EndDraw();
        }

        protected override void Update(GameTime gameTime)
        {
            #region FPS Counter/Virtual Time
            VirtualTime = VirtualTime.AddMilliseconds(gameTime.ElapsedGameTime.Milliseconds * 2000);
            //VirtualTime = DateTime.Now;
            //VirtualTime.AddSeconds(-VirtualTime.Second);
            //VirtualTime.AddMilliseconds(-VirtualTime.Millisecond);
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
                //VirtualTime = VirtualTime.AddMinutes(10); //every second, 10 mins pass in game
            }
            #endregion
            //cos(a) = x / h
            //cos(a) * h = x
            //x = cos(a) * h

            //sin(a) = y / h
            //sin(a) * h = y
            //y = sin(a) * h

            float sunPosMod = (float)Math.Round(-500f + (float)(VirtualTime.TimeOfDay.TotalDays * 1000));
            sunPosition.X = (float)Math.Floor(camera.Position.X) + sunPosMod;
//            sunPosition.Z = (float)Math.Floor(camera.Position.Z) + sunPosMod;
            sunPosition.Y = (float)Math.Round(255 + (Maths.Saturate((float)(.5f + VirtualTime.TimeOfDay.TotalDays) / 4) * (600 - 255)));

            phase += .001f;
            if (phase > Math.PI) phase = 0;

            /*byte[] bytes = new byte[16*16*4];
            Globals.Random.NextBytes(bytes);
            randomTexture.SetData<byte>(bytes);*/

            /*try
            {
                Vec2 origin = scene.World[camera.Position].WorldPosition;
                if (origin != lastChunk)
                {
                    List<Chunk> chunks = scene.World.SurroundingChunks(camera.Position, 2);
                    lock (map) map = chunks.ToTexture(GraphicsDevice, origin);
                    lastChunk = origin;
                }
            }
            catch { lock (map) map = new Texture2D(GraphicsDevice, 48, 48); }*/

            memUsage = (int)Math.Ceiling((double)GC.GetTotalMemory(false) / 1024 / 1024);
            lastInteraction += gameTime.ElapsedGameTime;
            KeyboardHandler(gameTime); //keyboard input
            Vec3 old = camera.Position; //store player position

            surroundingChunks = scene.World.SurroundingChunks(camera.Position, 4);
            MouseHandler(gameTime); //mouse input
            //TouchHandler(gameTime); //touch input
            base.Update(gameTime);
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            MouseState ms = Mouse.GetState();
            if (ms != previousMouseState)
            {
                if (ms.ButtonPressed(XNA.MouseButton.Left))
                {
                    //left click
                    if (playButton.Contains(ms.Position))
                    {
                        Program.StartGame = true;
                        foreach (Thread t in ChildThreads) t.Abort();
                        this.Exit();
                    }
                }

                if (ms.ButtonPressed(XNA.MouseButton.Middle))
                {
                    
                }

                if (ms.ButtonPressed(XNA.MouseButton.Right))
                {
                    //right click
                    
                }
                previousMouseState = ms;
            }
        }

        public void KeyboardHandler(GameTime gameTime)
        {
            //Keyboard Handling
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
            {
                foreach (Thread t in ChildThreads) t.Abort();
                this.Exit();
            }

            /*if (ks.IsKeyDown(Keys.Up))
                camera.Rotation = new Vec3(camera.Rotation.X - .01f, camera.Rotation.Y, 0);
            if (ks.IsKeyDown(Keys.Down))
                camera.Rotation = new Vec3(camera.Rotation.X + .01f, camera.Rotation.Y, 0);
            if (ks.IsKeyDown(Keys.Left))
                camera.Rotation = new Vec3(camera.Rotation.X, camera.Rotation.Y - .01f, 0);
            if (ks.IsKeyDown(Keys.Right))
                camera.Rotation = new Vec3(camera.Rotation.X, camera.Rotation.Y + .01f, 0);

            if (ks.IsKeyDown(Keys.O))
                VirtualTime = VirtualTime.AddHours(1);
            if (ks.IsKeyDown(Keys.P))
                VirtualTime = VirtualTime.AddMinutes(1);*/
            if (ks.IsKeyDown(Keys.G))
                GC.Collect();
            if (ks.IsKeyDown(Keys.Home))
                if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
            if (ks.KeyPressed(Keys.OemTilde))
                wireframe = !wireframe;
            /*if (ks.KeyPressed(Keys.F3))
                Globals.FPSGraph = !Globals.FPSGraph;*/
        }

        public void TouchHandler(GameTime gameTime)
        {
            TouchCollection tc = TouchPanel.GetState();
            if (tc.Count == 0) return;
            Console.WriteLine(tc[0].Position.ToString());
        }

        
    }
}
