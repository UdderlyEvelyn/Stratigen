using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratigen.Datatypes;
using Stratigen.Libraries;
using SharpDX.Toolkit.Input;
using SharpDX.XInput;
using Stratigen.Framework;
using Ray = Stratigen.Datatypes.Ray;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;

namespace Stratigen
{
    public class Engine : Game
    {
        private GraphicsDeviceManager gdm;
        private GraphicsDevice gd;
        private BasicEffect effect;
        private RasterizerState rasterizerState;
        private Camera camera;
        private MouseManager mm;
        private KeyboardManager km;
        private Vector2 center = new Vector2(.5f, .5f);
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private string info;
        private Scene scene;
        private SharpDX.Direct3D11.Device device;
        private SharpDX.Direct3D11.DeviceContext context;

        //TODO:
        /*
         * Create a drawing manager similar to the collision library.
         * Flesh out Vec2 functionality.
         * Malleable terrain.
         * Jump/Crouch.
         * Chunked terrain.
         * More primitives in VTX form.
         * Terrain depth (array of terrain "sheets").
         * Vec4 (dunno what we'll need it for, but why not).
         * More collision shapes (Sphere, Rectangle, Disk, Plane, etc.).
         * Cave generation applied to depth terrain.
         */

        public Engine()
        {
            scene = new Scene();
            //terrain = new Array2<byte>(128, 128).SimplexNoise(seed = Program.random.Next(int.MinValue, int.MaxValue));
            Globals.Game = this;
            mm = new MouseManager(this);
            km = new KeyboardManager(this);
            gdm = new GraphicsDeviceManager(this);
            gdm.DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport;
            gdm.PreferredBackBufferWidth = 800;
            gdm.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Data";
            IsMouseVisible = false;
            IsFixedTimeStep = true;
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Arial16.tkb");
            base.LoadContent();
        }

        protected override void Initialize()
        {
            Col3 white = new Col3(255, 255, 255);
            Globals.CubeModel = new Datatypes.Model(
                new Vertex[] {
                    new Vertex(Vec3.NNN, white),
                    new Vertex(Vec3.NNP, white),
                    new Vertex(Vec3.NPN, white),
                    new Vertex(Vec3.NPP, white),
                    new Vertex(Vec3.PNN, white),
                    new Vertex(Vec3.PNP, white),
                    new Vertex(Vec3.PPN, white),
                    new Vertex(Vec3.PPP, white),
                },
                new Index[12]
                {
                    new Index(3, 1, 0),
                    new Index(2, 1, 3),
                    new Index(0, 5, 4),
                    new Index(1, 5, 0),
                    new Index(3, 4, 7),
                    new Index(0, 4, 3),
                    new Index(1, 6, 5),
                    new Index(2, 6, 1),
                    new Index(2, 7, 6),
                    new Index(3, 7, 2),
                    new Index(6, 4, 5),
                    new Index(7, 4, 6),
                }
            );
            scene.Models.Add(Globals.CubeModel);
            //scene.AddVisual(Globals.CubeModel);

            /*      new Index(0, 2, 4),
                    new Index(2, 6, 4),
                    new Index(1, 5, 3),
                    new Index(3, 5, 7),
                    new Index(0, 5, 1),
                    new Index(0, 5, 4),
                    new Index(2, 3, 7),
                    new Index(2, 7, 6),
                    new Index(0, 3, 2),
                    new Index(1, 3, 0),
                    new Index(4, 6, 7),
                    new Index(5, 4, 7),*/
            Globals.GraphicsDevice = gd = gdm.GraphicsDevice;
            Globals.CubeIndexBuffer = Buffer.New<short>(gd, 36, BufferFlags.IndexBuffer);
            Globals.CubeVertexBuffer = Buffer.New<VertexPositionColor>(gd, 8, BufferFlags.VertexBuffer);
            Globals.CubeIndexBuffer.SetData<short>(Globals.CubeIndices);
            Globals.CubeVertexBuffer.SetData<VertexPositionColor>(Globals.CubeVertices);
            Globals.CubeVertexInputLayout = VertexInputLayout.FromBuffer(0, Globals.CubeVertexBuffer);
            Globals.Camera = camera = new Camera(Window);
            camera.FarClip = 9999f;
            camera.NearClip = 1f;
            camera.Aspect = 1.33f;
            camera.FOV = MathUtil.PiOverFour;
            spriteBatch = new SpriteBatch(gd);
            effect = new BasicEffect(gd) { VertexColorEnabled = true };
            //Globals.CubeVertices = Mesh.VerticesFromVTX("Data/Cube.vtx");
            //cube = new Mesh { GraphicsDevice = gd, Position = Vector3.Zero };
            //cube.SetVertexBuffer(Globals.CubeVertices);
            //mesh = terrain.ToMesh(gd, "Terrain");
            //scene.AddVisual(Quadrant(0, 0));
            //scene.AddVisual(Quadrant(1, 0));
            //scene.AddVisual(Quadrant(0, 1));
            //scene.AddVisual(Quadrant(1, 1));
            //scene.AddVisual(terrain.ToCubes());
            rasterizerState = RasterizerState.New(gd, new SharpDX.Direct3D11.RasterizerStateDescription { CullMode = SharpDX.Direct3D11.CullMode.Back, FillMode = SharpDX.Direct3D11.FillMode.Wireframe });     
            base.Initialize();
        }

        protected override bool BeginDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            gd.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            gd.SetRasterizerState(rasterizerState);
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Perspective;
            foreach (EffectPass ep in effect.CurrentTechnique.Passes)
            {
                ep.Apply();
            }
            scene.Draw();
            //mesh.DrawIndexed();
            //cube.DrawAt(cube.Position);
            spriteBatch.DrawString(font, "Stratigen Test" +
                                         "\nSeed: " + Program.seed +
                                         "\nX: " + Math.Round(camera.CamX, 2) +
                                         "\nY: " + Math.Round(camera.CamY, 2) +
                                         "\nZ: " + Math.Round(camera.CamZ, 2) +
                                         "\nPitch: " + Math.Round(camera.CamRotX, 2) +
                                         "\nFacing: " + Math.Round(camera.CamRotY, 2) +
                                         "\nGameTime: " + Math.Round(gameTime.TotalGameTime.TotalSeconds, 2) + "s" +
                                         "\nInfo: " + info +
                                         "\nCollision Objects: " + Stratigen.Libraries.Collision.CollisionObjects.Count
                                         , Vector2.Zero, Color.White);
            //Font Size + 8 * Line Count = Offset
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            spriteBatch.End();
            base.EndDraw();
        }

        protected override void Update(GameTime gameTime)
        {
            //Mouse Handling
            MouseState ms = mm.GetState();
            camera.Tilt(ms.Y - .5f);
            camera.Turn(ms.X - .5f);
            mm.SetPosition(center);
            //Keyboard Handling
            KeyboardState ks = km.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();
            if (ks.IsKeyDown(Keys.Space))
                camera.MoveUp();
            if (ks.IsKeyDown(Keys.Control))
                camera.MoveDown();
            if (ks.IsKeyDown(Keys.Left))
                camera.TurnLeft();
            if (ks.IsKeyDown(Keys.Right))
                camera.TurnRight();
            if (ks.IsKeyDown(Keys.Up))
                camera.TiltUp();
            if (ks.IsKeyDown(Keys.Down))
                camera.TiltDown();
            if (ks.IsKeyDown(Keys.W))
                camera.MoveForward();
            if (ks.IsKeyDown(Keys.S))
                camera.MoveBackward();
            if (ks.IsKeyDown(Keys.A))
                camera.MoveLeft();
            if (ks.IsKeyDown(Keys.D))
                camera.MoveRight();
            if (ks.IsKeyPressed(Keys.OemTilde))
            {
                if (rasterizerState.Description.FillMode == SharpDX.Direct3D11.FillMode.Wireframe)
                    rasterizerState = RasterizerState.New(gd, new SharpDX.Direct3D11.RasterizerStateDescription { CullMode = SharpDX.Direct3D11.CullMode.Back, FillMode = SharpDX.Direct3D11.FillMode.Solid });
                else
                    rasterizerState = RasterizerState.New(gd, new SharpDX.Direct3D11.RasterizerStateDescription { CullMode = SharpDX.Direct3D11.CullMode.Back, FillMode = SharpDX.Direct3D11.FillMode.Wireframe });     
            }
            /*if (ks.IsKeyPressed(Keys.E))
            {
                //Y is modified so that the ray doesn't start in any terrain you happen to be clipping - this could be changed later when the clipping isn't relevant.
                Ray r = new Ray(-camera.Position.ToVec3() + -camera.Look.ToVec3() * 2, -camera.Look.ToVec3());
                ICollisionObject co2 = r.Cast(100, 1);
                info = co2 == null ? "N/A" : co2.Position.ToString();
                cube.SetPosition(co2.Position.ToVector3()); //Show "selection" spot upon pick.
            }*/
            ICollisionObject co = (-camera.Position.ToVec3()).ClosestXZ(10);
            if (co != null) camera.CamY = -co.Position.Y + Globals.CameraHeightOffset; //Match ground height.
            base.Update(gameTime);
        }

        /// <summary>
        /// Produces a cell at the proper size/projection for a 2x2 stitched image with the provided offsets.
        /// </summary>
        /// <param name="x">X "world coordinate" of this cell within the assumed 2x2 stitched image.</param>
        /// <param name="y">Y "world coordinate" of this cell within the assumed 2x2 stitched image.</param>
        /// <returns></returns>
        static Cell Cell(int x, int y)
        {
            return new Cell(Program.seed, Program.width / 8, Program.height / 8, x, y, Program.width, Program.height);
        }

        static Chunk Chunk(int x, int y)
        {
            Chunk c = new Chunk() { Size = ((Program.width / 4) + (Program.height / 4)) };
            c.SetPosition(new Vec3(x * Program.width / 4 + (Program.width / 8), 0, y * Program.height / 4 + (Program.height / 8)));
            c.CullChildren = false;
            c.AddVisual(Cell(x, y).Simplex().ToCubes());
            c.AddVisual(Cell(x + 1, y).Simplex().ToCubes());
            c.AddVisual(Cell(x, y + 1).Simplex().ToCubes());
            c.AddVisual(Cell(x + 1, y + 1).Simplex().ToCubes());
            return c;
        }

        static Chunk Quadrant(int x, int y)
        {
            Chunk c = new Chunk() { Size = ((Program.width / 2) + (Program.height / 2)) };
            c.SetPosition(new Vec3(x * Program.width / 2 + (Program.width / 4), 0, y * Program.height / 2 + (Program.height / 4)));
            c.AddVisual(Chunk(0, 0));
            c.AddVisual(Chunk(1, 0));
            c.AddVisual(Chunk(0, 1));
            c.AddVisual(Chunk(1, 1));
            return c;
        }
    }
}