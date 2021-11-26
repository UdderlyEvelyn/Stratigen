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

/*
 * Known Bugs:
 * 
 * 
 * To Do:
 * Cascaded Shadow Maps
 * Clouds
 * Non-block Items
 * Lakes
 * Upgraded Water Effects (Proper Reflection/Refraction)
*/

namespace Stratigen
{
    public class Kernel : Game
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
        RenderCamera camera;
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
        Font consolas = new Font("Consolas");
        int frameRate = 0;
        int frameCounter = 0;
        int memUsage = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        float POS89RADS;
        float NEG89RADS;
        Texture2D blockTextures;
        TimeSpan airTime = TimeSpan.Zero;
        Vec3 initialPosition = Vec3.Zero;
        bool jumping = false;
        float gravity = 2f;
        BasicEffect colorEffect;
        Vec3 selectionPoint = Vec3.Zero;
        float? blockDistance = null;
        Ray pickRay = new Ray(Vec3.Zero, Vec3.Zero);
        Block selectedBlock = null;
        TimeSpan lastInteraction = TimeSpan.Zero;
        Inventory inv = new Inventory(4, 4);
        List<Block> surroundingBlocks = new List<Block>();
        Block block = null;
        Texture2D crosshairTexture;
        Vec3 pickNormal = new Vec3(0, 0, 0);
        List<Chunk> surroundingChunks = new List<Chunk>();
        List<int> FPSStack = new List<int>(capacity: 200);
        DateTime startTime;
        Font consolasSmall = new Font("Consolas Small");
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
        float sunlight = 1;

        public Kernel()
        {
            startTime = DateTime.Now;
            Globals.Kernel = this;
            random = new Random();
            Window.Title = Console.Title = "Stratigen Kernel";
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
            IsMouseVisible = false;
        }

        protected override void LoadContent()
        {
            consolas.Texture = Content.Load<Texture2D>("Fonts/Consolas36.png");
            consolas.LoadMetrics(Content.RootDirectory + "/Fonts/Consolas36.xml");
            consolas.Scale = .45f;
            consolasSmall.Texture = Content.Load<Texture2D>("Fonts/Consolas36.png");
            consolasSmall.LoadMetrics(Content.RootDirectory + "/Fonts/Consolas36.xml");
            consolasSmall.Scale = .25f;
            if (Platform.API == Platform.APIs.OpenTK)
            {
                Globals.Effect = effect = Content.Load<Effect>("Shaders/SM2Blocks.mgfx");
                shadowMapper = Content.Load<Effect>("Shaders/SM2ShadowMap.mgfx");
            }
            else if (Platform.API == Platform.APIs.SharpDX)
            {
                Globals.Effect = effect = Content.Load<Effect>("Shaders/SM4Blocks.mgfx");
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
            Globals.Scene = scene = new Scene(GraphicsDevice, Globals.Camera = camera = new RenderCamera(this) { FOV = MathHelper.PiOver4, Aspect = Globals.Width/Globals.Height, NearClip = .05f, FarClip = 1000f });
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
            #endregion

            #region Child Thread Initialization
            ChunkGenerationThread = new Thread((ThreadStart)delegate
            {
                while (1 == 1)
                {
                    if (ChunkPositionsToGenerate.Count > 0)
                    {
                        lock (ChunkPositionsToGenerate)
                        {
                            Parallel.ForEach<Vec2>(ChunkPositionsToGenerate,
                                delegate(Vec2 v)
                                {
                                    Globals.Scene.World.GenerateChunk(v.Xi, v.Yi, Globals.Kernel.seed);
                                }
                            );
                            ChunkPositionsToGenerate.Clear();
                        }
                    }
                    else Thread.Sleep(10);
                    if (Thread.CurrentThread.ThreadState == System.Threading.ThreadState.AbortRequested) break; //try to speed aborting along by leaving the loop.
                }
            });
            ChunkGenerationThread.Start();
            ChildThreads.Add(ChunkGenerationThread);
            #endregion

            camera.Position = initialPosition = new Vec3(8, 200, 8); 
            spriteBatch = new SpriteBatch(GraphicsDevice);

            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };//, MultiSampleAntiAlias = true };
            depthStencilState = new DepthStencilState { DepthBufferFunction = CompareFunction.LessEqual };
            samplerStateClampPoint = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Point };
            samplerStateClampLinear = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Linear };
            samplerStateClampAniso = new SamplerState { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, Filter = TextureFilter.Anisotropic, MaxAnisotropy = 16 };
            blendState = BlendState.Opaque; 

            #region Item System

            //Populate items for each block type.
            foreach (BlockType bt in BlockType.Types.Values)
                ItemManager.Add(new Item(bt.Name, bt, 128) { IconPath = System.IO.File.Exists(Content.RootDirectory + @"\Icons\" + bt.Name + ".png") ? @"Icons\" + bt.Name + ".png" : null });

            for (int i = 0; i < 11; i++)
            {
                inv.Put(ItemManager.Get("Stone"));
                inv.Put(ItemManager.Get("Dirt"));
                inv.Put(ItemManager.Get("Iron"));
                inv.Put(ItemManager.Get("Gold"));
                inv.Put(ItemManager.Get("Log"));
                inv.Put(ItemManager.Get("Leaves"));
                inv.Put(ItemManager.Get("Water"));
            }

            inv.Select(ItemManager.Get("Water"));

            #endregion

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
            effect.Parameters["Sunlight"].SetValue((float)Math.Pow(100, Math.Sin((VirtualTime.Hour + .5) / 8)) / 100);
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

            /*GraphicsDevice.SetRenderTarget(farShadowMap);
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
                if (selectedBlock != null)
                    GraphicsDevice.DrawCube(selectedBlock.BoundingBox.Min, selectedBlock.BoundingBox.Max, Color.White);
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
                GraphicsDevice.DrawFilledRectangle(camera, new Vec2(2, Window.ClientBounds.Height - 20), 130, 20, Color.White, Color.Black); //draw leftmost rectangle for the current item display
                GraphicsDevice.DrawFilledRectangle(camera, new Vec2(320, Window.ClientBounds.Height - 20), 323 + inv.Items.Keys.Count(i => i.IconPath != null) * 17, 20, Color.White, Color.Black); //draw middle rectangle for the block type/item hotbar
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
                "Stratigen Kernel (" + (Platform.OpenGL ? "GL" : Platform.DirectX ? "DX" : "?") + ") - " + frameRate + "FPS - " + memUsage + "MB" +
                "\nRotation: " + Math.Round(camera.Pitch, 2) + ", " + Math.Round(camera.Yaw, 2) +
                "\nAbsolute: " + Math.Round(camera.Position.X, 2) + ", " + Math.Round(camera.Position.Y, 2) + ", " + Math.Round(camera.Position.Z, 2) +
                "\nChunk: " + cc.CX + ", " + cc.CZ +
                "\nRegion: " + (camera.ChunkCoordinates.Xi / 16) + ", " + (camera.ChunkCoordinates.Yi / 16) +
                "\nLocal: " + cc.X + ", " + cc.Y + ", " + cc.Z +
                "\nTime: " + VirtualTime.ToShortTimeString() + //" " + VirtualTime.Hour + "(" + ((float)Math.Pow(100, Math.Sin((VirtualTime.Hour + .5) / 8)) / 100) + ")" +
                "\nSun: " + Math.Round(sunPosition.X) + ", " + Math.Round(sunPosition.Y) + ", " + Math.Round(sunPosition.Z) +
                "", Vec2.Zero); //debug display
            spriteBatch.Draw(crosshairTexture, new Vector2(Window.ClientBounds.Center.X - crosshairTexture.Width / 2, Window.ClientBounds.Center.Y - crosshairTexture.Height / 2), Color.White); //draw crosshair
            if (selectedBlock != null && selectedBlock.Type != null) spriteBatch.DrawShadowedString(consolas,
                selectedBlock.Type.Name + " [" + selectedBlock.Health + "%]" +
                "", new Vec2(Window.ClientBounds.Center.X - 16, Window.ClientBounds.Center.Y + 16), Color.White); //display block name that you are aimed at
            if (inv.SelectedItem != null && inv.SelectedItem.IconPath != null) spriteBatch.DrawImage(new Vec2(4, Window.ClientBounds.Height - 18), inv.SelectedItem.IconPath); //display the currently selected item icon
            spriteBatch.DrawShadowedString(consolas, inv.SelectedItem.Name + " x " + inv.Count(inv.SelectedItem), new Vec2(22, Window.ClientBounds.Height - 18)); //display the item name and count
            int j = 0;
            foreach (Item i in inv.Items.Keys)
            {
                if (i.IconPath != null)
                {
                    if (inv.SelectedItem == i)
                    {
                        for (int x = 0; x < 16; x++)
                            spriteBatch.DrawPixel(new Vec2(322 + 17 * j + x, Window.ClientBounds.Height - 19), Color.Red); //draw red line above selected block type
                    }
                    spriteBatch.DrawImage(new Vec2(322 + 17 * j, Window.ClientBounds.Height - 18), i.IconPath); //draw block type icon
                    spriteBatch.DrawString(consolasSmall, inv.Count(i).ToString(), new Vec2(322 + 17 * j, Window.ClientBounds.Height - 18));
                }
                j++;
            }
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
            VirtualTime = VirtualTime.AddMilliseconds(gameTime.ElapsedGameTime.Milliseconds * 10);
            //VirtualTime = DateTime.Now;
            //VirtualTime.AddSeconds(-VirtualTime.Second);
            //VirtualTime.AddMilliseconds(-VirtualTime.Millisecond);
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                FPSStack.Insert(0, Window.ClientBounds.Height - frameRate); //push the latest framerate onto the stack
                FPSStack.RemoveAt(FPSStack.Count - 1); //pop the oldest framerate off the stack
                frameCounter = 0;
                //VirtualTime = VirtualTime.AddMinutes(10); //every second, 10 mins pass in game
                //www.desmos.com/calculator
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

            #region Gravity & Jumping
            if (!scene.World.BlockAt(camera.Position.X, camera.Position.Y - camera.LegLength, camera.Position.Z))
            {
                airTime += gameTime.ElapsedGameTime;
                camera.Velocity.Y -= (float)(gravity * airTime.TotalSeconds * airTime.TotalSeconds); //if there's no block below the camera, enact gravity
                camera.Velocity.Y = (float)Math.Max(camera.Velocity.Y, -3);
            }
            else
            {
                Block b = scene.World.GetBlock(camera.Position.X, camera.Position.Y - camera.LegLength, camera.Position.Z);
                if (b.Type != null && b.Type.Material.Hardness < 1)
                {
                    airTime += gameTime.ElapsedGameTime;
                    camera.Velocity.Y -= (float)(gravity / 3 * airTime.TotalSeconds * airTime.TotalSeconds); //if there's a non-solid block below the camera, enact inhibited gravity
                    camera.Velocity.Y = (float)Math.Max(camera.Velocity.Y, -1);
                }
            }
            #endregion

            camera.Move(camera.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * camera.MoveSpeed); //move camera according to keyboard input & gravity
            if (scene.World[camera.Position] == null)
                camera.Position = new Vec3(old.X, camera.Position.Y, old.Z); //don't let the player leave the playable area
            if (camera.Position.Y < 0) camera.Position = new Vec3(camera.Position.X, camera.LegLength, camera.Position.Z); //don't let the player fall below y=0

            #region Collision Handling & Selection
            selectedBlock = null;
            blockDistance = null;
            surroundingBlocks = scene.World.Surrounding(camera.Position, 4);
            foreach (Block b in surroundingBlocks) //iterate through blocks within a short distance of the player
            {
                SelectionRoutine(b);
                //Resolve horizontal first or the player can collide vertically with a block that they should be sliding against vertically 
                //since they'll clip by some small value before the horizontal collision is resolved.
                Resolve(camera, b, old); //collision detection & resolution
                if (camera.Velocity.Y < 0) //if falling
                {
                    if (b.Type.Material.Hardness >= 1 && DownRayCheck(camera, b))
                    {
                        //Y is set to .0001f above the block to avoid getting "stuck" without any visual jitter from a higher fractional value.
                        camera.Position = new Vec3(camera.Position.X, b.BoundingBox.Max.Y + camera.LegLength + .0001f, camera.Position.Z);
                        airTime = TimeSpan.Zero;
                        camera.Velocity.Y = 0;
                        jumping = false;
                        block = b;
                    }
                }
                else if (camera.Velocity.Y > 0) //if rising/jumping
                {
                    if (b.Type.Material.Hardness >= 1 && camera.UpRay.Intersects(b.BoundingBox) == 0)
                    {
                        //Y is set to .0001f below the block to avoid getting "stuck", with a small value to avoid any potential visual jitter.
                        camera.Position = new Vec3(camera.Position.X, b.BoundingBox.Min.Y - .0001f, camera.Position.Z);
                        airTime = TimeSpan.Zero;
                        camera.Velocity.Y = 0;
                    }
                }
            }
            #endregion

            #region Friction & Drag
            float friction = .03f;
            if (!jumping) friction = .1f;
            //slowly decelerate from drag or friction
            if (camera.Velocity.X != 0)
            {
                int SignX = Math.Sign(camera.Velocity.X);
                camera.Velocity.X -= SignX * friction;
                if (Math.Sign(camera.Velocity.X) != SignX) camera.Velocity.X = 0;
            }
            if (camera.Velocity.Z != 0)
            {
                int SignZ = Math.Sign(camera.Velocity.Z);
                camera.Velocity.Z -= SignZ * friction;
                if (Math.Sign(camera.Velocity.Z) != SignZ) camera.Velocity.Z = 0;
            }
            #endregion

            #region Determine Chunks To Generate
            Coordinate co = camera.Position.ToCoordinate();
            for (int z = -scene.ChunkViewDistance + co.CZ; z < scene.ChunkViewDistance + co.CZ; z++)
            {
                for (int x = -scene.ChunkViewDistance + co.CX; x < scene.ChunkViewDistance + co.CX; x++)
                {
                    if (x > -1 && z > -1) //Not Negative
                    {
                        Vec2 v = new Vec2(x, z);
                        if (Globals.Scene.World[x, z] == null)
                        {
                            if (!ChunkPositionsToGenerate.Contains(v)) lock(ChunkPositionsToGenerate) ChunkPositionsToGenerate.Add(v);
                        }
                    }
                }
            }
            #endregion

            surroundingChunks = scene.World.SurroundingChunks(camera.Position, 4);
            MouseHandler(gameTime); //mouse input
            //TouchHandler(gameTime); //touch input
            base.Update(gameTime);
        }

        bool DownRayCheck(RenderCamera a, ICollidable3 b)
        {
            Ray[] rays = a.DownRays;
            float? r1 = rays[0].Intersects(b.BoundingBox);
            float? r2 = rays[1].Intersects(b.BoundingBox);
            float? r3 = rays[2].Intersects(b.BoundingBox);
            float? r4 = rays[3].Intersects(b.BoundingBox);
            return (r1 == 0 || r2 == 0 || r3 == 0 || r4 == 0);
        }

        void SelectionRoutine(Block b)
        {
            float? bd = pickRay.Intersects(b.BoundingBox); //check if the block we're processing is in the selection ray
            if (bd != null && (bd < blockDistance || selectedBlock == null || blockDistance == null)) //if it is, and either there isn't something picked or it's closer than what has been
            {
                selectedBlock = b; //select the block
                blockDistance = bd; //store the distance to the block for future comparisons and display
                Vec3 n = (Vec3)pickRay.Position + ((Vec3)pickRay.Direction * (bd.Value - .001f)) - b.Position;
                float abyy = Math.Abs(n.Y);
                abyy *= abyy;
                float abx = Math.Abs(n.X);
                float abz = Math.Abs(n.Z);
                float abxz = abx * abx + abz * abz;
                pickNormal = Vec3.Zero;
                if (abyy > abxz)
                {
                    if (n.Y < 0) pickNormal.Y = -1;
                    if (n.Y > 0) pickNormal.Y = 1;
                }
                else
                {
                    if (abx > abz)
                    {
                        if (n.X < 0) pickNormal.X = -1;
                        if (n.X > 0) pickNormal.X = 1;
                    }
                    else
                    {
                        if (n.Z < 0) pickNormal.Z = -1;
                        if (n.Z > 0) pickNormal.Z = 1;
                    }
                }
            }
        }

        //"a" is the object that has moved, 
        //"b" is what it may have collided with (we're checking), 
        //oldPos is the position of "a" prior to the movement, 
        //and depth can be ignored and is used for limiting recursive checking.
        //note that this was designed for "b" to be a static object - if it's being used for another moving one, you might need to do something special like run resolve in both directions..
        void Resolve(ICollidable3 a, ICollidable3 b, Vec3 oldPos, int depth = 0)
        {
            //Positional difference between the two provides directional vector between them
            Vec3 diff = b.Position - a.Position;
            //Absolute values for comparison purposes (amount of change, disregard direction)
            float adx = (float)Math.Abs(diff.X);
            float adz = (float)Math.Abs(diff.Z);
            if (b is Block && ((Block)b).Type.Material.Hardness < 1) return;
            if (camera.BoundingBox.Intersects(b.BoundingBox)) //if there's a collision
            {
                if (adx > adz) //if there's more X than Z
                {
                    a.Position = new Vec3(oldPos.X, a.Position.Y, a.Position.Z); //reset the X
                    if (camera.BoundingBox.Intersects(b.BoundingBox)) //if that didn't work
                        a.Position = new Vec3(a.Position.X, a.Position.Y, oldPos.Z); //reset the Z
                }
                else if (adx < adz) //if there's more Z than X
                {
                    a.Position = new Vec3(a.Position.X, a.Position.Y, oldPos.Z); //reset the Z
                    if (camera.BoundingBox.Intersects(b.BoundingBox)) //if that didn't work
                        a.Position = new Vec3(oldPos.X, a.Position.Y, a.Position.Z); //reset the X
                }
                else a.Position = new Vec3(oldPos.X, a.Position.Y, oldPos.Z); //reset X and Z
            }
            if (camera.BoundingBox.Intersects(b.BoundingBox)) //if we fail to resolve the collision
            {
                if (++depth < 25) //if we have more passes left
                    Resolve(a, b, oldPos, depth); //keep trying
                else a.Position = new Vec3(oldPos.X, a.Position.Y, oldPos.Z); //reset X and Z
            }
            return; //resolved or aborted
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            MouseState ms = Mouse.GetState();
            if (ms != previousMouseState)
            {
                Vec2 center = new Vec2(Window.ClientBounds.Width / 2 + Window.ClientBounds.X, Window.ClientBounds.Height / 2 + Window.ClientBounds.Y);
                float dx = ms.X - center.X;
                float dy = ms.Y - center.Y;
                mouseRotationBuffer.X -= camera.MouseSpeed * dx * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mouseRotationBuffer.Y -= camera.MouseSpeed * dy * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (mouseRotationBuffer.Y < NEG89RADS)
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - NEG89RADS);
                if (mouseRotationBuffer.Y > POS89RADS)
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - POS89RADS);
                camera.Rotation = new Vec3(-MathHelper.Clamp(mouseRotationBuffer.Y, NEG89RADS, POS89RADS), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);
                Mouse.SetPosition((int)center.X, (int)center.Y);

                if (ms.ButtonPressed(XNA.MouseButton.Left))
                {
                    //left click
                    if (selectedBlock != null && lastInteraction.TotalSeconds > .25)
                    {
                        selectedBlock.Health -= 10;
                        if (selectedBlock.Health <= 0)
                        {
                            Coordinate co = selectedBlock.Position.ToCoordinate();
                            Chunk c = scene.World.GetChunk(co.CX, co.CZ);
                            inv.Put(ItemManager.Get(selectedBlock.Type.Name));
                            c.Blocks[co.X, co.Y, co.Z] = null;
                            //c.UpdateLight(co.X, co.Z);
                            c.UpdateSurrounding(co.X, co.Y, co.Z);
                            c.Dirty = true;
                            c.Changes[selectedBlock.Position] = new Change(selectedBlock.Position, 0);
                        }
                        lastInteraction = TimeSpan.Zero;
                    }
                }

                if (ms.ButtonPressed(XNA.MouseButton.Middle))
                {
                    if (selectedBlock != null && lastInteraction.TotalSeconds > .25)
                    {
                        Coordinate co = selectedBlock.Position.ToCoordinate();
                        Chunk c = scene.World.GetChunk(co.CX, co.CZ);
                        c.UpdateSelfAndSurrounding(co.X, co.Y, co.Z);
                        c.Dirty = true;
                    }
                }

                if (ms.ButtonPressed(XNA.MouseButton.Right))
                {
                    //right click
                    if (selectedBlock != null && lastInteraction.TotalSeconds > .25)
                    {
                        if (!inv.UseSelected()) return;
                        Vec3 v = selectedBlock.Position + pickNormal;
                        Coordinate co = v.ToCoordinate();
                        Chunk c = scene.World.GetChunk(co.CX, co.CZ);
                        if (c.Blocks[co.X, co.Y, co.Z] == null)
                        {
                            c.Blocks[co.X, co.Y, co.Z] = new Block(v, inv.SelectedItem.Actual as BlockType) { Facing = Block.Faces.All };
                            //c.UpdateLight(co.X, co.Z);
                            c.UpdateSelfAndSurrounding(co.X, co.Y, co.Z);
                            c.Dirty = true;
                            c.Changes[v] = new Change(v, (inv.SelectedItem.Actual as BlockType).ID);
                        }
                        lastInteraction = TimeSpan.Zero;
                    }
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
            if (ks.IsKeyDown(Keys.Space))
            {
                if (camera.Velocity.Y == 0 && !jumping)
                {
                    camera.Velocity.Y = 1;
                    jumping = true;
                }
            }
            if (ks.IsKeyDown(Keys.W))
                camera.Velocity.Z = (jumping ? .5f : 1);
            if (ks.IsKeyDown(Keys.S))
                camera.Velocity.Z = (jumping ? -.5f : -1);
            if (ks.IsKeyDown(Keys.A))
                camera.Velocity.X = (jumping ? .5f : 1);
            if (ks.IsKeyDown(Keys.D))
                camera.Velocity.X = (jumping ? -.5f : -1);
            if (ks.IsKeyDown(Keys.D1))
                inv.Select(ItemManager.Get("Grass"));
            if (ks.IsKeyDown(Keys.D2))
                inv.Select(ItemManager.Get("Stone"));
            if (ks.IsKeyDown(Keys.D3))
                inv.Select(ItemManager.Get("Iron"));
            if (ks.IsKeyDown(Keys.D4))
                inv.Select(ItemManager.Get("Log"));
            if (ks.IsKeyDown(Keys.D5))
                inv.Select(ItemManager.Get("Leaves"));
            if (ks.IsKeyDown(Keys.D6))
                inv.Select(ItemManager.Get("Water"));
            if (ks.IsKeyDown(Keys.O))
                VirtualTime = VirtualTime.AddHours(1);
            if (ks.IsKeyDown(Keys.P))
                VirtualTime = VirtualTime.AddMinutes(1);
            if (ks.IsKeyDown(Keys.G))
                GC.Collect();
            if (ks.IsKeyDown(Keys.Home))
                if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
            if (ks.KeyPressed(Keys.OemTilde))
                wireframe = !wireframe;
            if (ks.IsKeyDown(Keys.R))
                scene.World[Coordinate.From(camera.Position).Chunk].Save();
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
