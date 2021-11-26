using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Stratigen.Libraries;
using Stratigen.Datatypes;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Stratigen.Framework;
//using OpenTK.Extensions;

namespace Stratigen
{
    public static class Program
    {
        public static Random random = new Random();
        public static int width = 128;
        public static int height = 128;
        public static double seed = random.NextDouble(); // = Program.random.Next(int.MinValue, int.MaxValue)
        private static Camera camera;
        private static Point mouseDelta;
        private static int centerX;
        private static int centerY;
        private static CursorCube cursorCube = new CursorCube();
        private static ICollisionObject collisionCursor;
        private static Array2<byte> terrain;

        static void Main(string[] args)
        {
            /*string r = " ";
            if (args != null && args.Count() > 0)
            {
                if (!int.TryParse(args[0], out width)) throw new ArgumentException("The value passed for the width argument was invalid.");
                if (!int.TryParse(args[1], out height)) throw new ArgumentException("The value passed for the height argument was invalid.");
                if (!double.TryParse(args[2], out seed)) throw new ArgumentException("The value passed for the seed argument was invalid.");
                Console.WriteLine("Generating a " + width + "x" + height + " grid.");
            }
            else
            {
                Console.WriteLine("Using default arguments, generating a 256x256 grid.");
                r = " random ";
            }
            Console.WriteLine("Using" + r + "seed \"" + seed + "\".");*/
            /*Cell c0000 = Cell(0, 0).Simplex();
            Cell c0010 = Cell(1, 0).Simplex();
            Cell c0001 = Cell(0, 1).Simplex();
            Cell c0011 = Cell(1, 1).Simplex();
            Array2<byte> ab00 = Array2<byte>.Stitch(c0000, c0010, c0001, c0011);
            Cell c1000 = Cell(2, 0).Simplex();
            Cell c1010 = Cell(3, 0).Simplex();
            Cell c1001 = Cell(2, 1).Simplex();
            Cell c1011 = Cell(3, 1).Simplex();
            Array2<byte> ab10 = Array2<byte>.Stitch(c1000, c1010, c1001, c1011);
            Cell c0100 = Cell(0, 2).Simplex();
            Cell c0110 = Cell(1, 2).Simplex();
            Cell c0101 = Cell(0, 3).Simplex();
            Cell c0111 = Cell(1, 3).Simplex();
            Array2<byte> ab01 = Array2<byte>.Stitch(c0100, c0110, c0101, c0111);
            Cell c1100 = Cell(2, 2).Simplex();
            Cell c1110 = Cell(3, 2).Simplex();
            Cell c1101 = Cell(2, 3).Simplex();
            Cell c1111 = Cell(3, 3).Simplex();
            Array2<byte> ab11 = Array2<byte>.Stitch(c1100, c1110, c1101, c1111);
            Array2<byte> result = Array2<byte>.Stitch(ab00, ab10, ab01, ab11);
            result.SaveImage("StratigenStitched.png");*/
            /*int octaves = 11;
            Array2<byte> surface = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, octaves, .7f, .4));
            Array2<byte> cavemap = new Array2<byte>(width, height).RandomStatic().Automata(10, 100, 0, true).Swap(0, 255).BiasRange(200, 254, 100, true).BiasRange(150, 200, 75, true).BiasRange(75, 100, 50, true);
            //byte d1 = 255 / 5;
            //byte d2 = 255 / 5 * 2;
            //byte d3 = 255 / 5 * 3;
            //byte d4 = 255 / 5 * 4;
            Array2<byte> depth1 = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, --octaves, .7f, .4));
            Array2<byte> depth2 = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, --octaves, .7f, .4));
            Array2<byte> depth3 = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, --octaves, .7f, .4));
            Array2<byte> depth4 = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, --octaves, .7f, .4));
            Array2<byte> depth5 = new Array2<byte>(width, height).SimplexNoise(seed, new Noise.NoiseArgs(.5f, .6f, --octaves, .7f, .4));
            surface.SaveImage("surface.png");
            cavemap.SaveImage("cavemap.png");
            depth1.SaveImage("depth1.png");
            depth2.SaveImage("depth2.png");
            depth3.SaveImage("depth3.png");
            depth4.SaveImage("depth4.png");
            depth5.SaveImage("depth5.png");*/
            //((Func<double, double>)Interpolation.SCurve5).PlotFunctionToImage("SC5.png", 400, 400, Color.Red, .005, true);
            /*Dictionary<Func<double, double>, Color> functions = new Dictionary<Func<double, double>, Color>
            {
                { ((Func<double, double>)Interpolation.SCurve3), Color.Red },
                { ((Func<double, double>)Interpolation.SCurve5), Color.Blue },
            };
            functions.PlotFunctionsToImage(400, 400, "functionTest.png");*/
            //Console.WriteLine("Done.");

            //new TestGame().Run();

            //terrain = new Array2<byte>(128, 128).SimplexNoise(seed = Program.random.Next(int.MinValue, int.MaxValue));
            Scene scene = new Scene();

            using (var game = new GameWindow())
            {
                bool init = false;
                bool lighting = true;
                bool camlock = true;
                game.Load += (sender, e) =>
                    {
                        game.ClientSize = new Size(800, 600);
                        game.VSync = VSyncMode.On;

                        scene.AddVisual(Quadrant(0, 0));
                        scene.AddVisual(Quadrant(1, 0));
                        scene.AddVisual(Quadrant(0, 1));
                        scene.AddVisual(Quadrant(1, 1));

                        //scene.AddVisual(new Array2<byte>(128, 128).SimplexNoise(seed).ToMesh("terrain"));
                        //terrain = new Array2<byte>(128, 128).SimplexNoise(seed);
                        /*BoxLight bl = new BoxLight();
                        bl.Scale(10);
                        bl.SetPosition(-100, 200, -100);
                        scene.AddVisual(bl);*/
                        //scene.AddVisual(new Array2<byte>(128, 128).SimplexNoise(seed, null, 0, 0, 512, 512).ToCubes());
                        scene.AddVisual(cursorCube);

                        Globals.Camera = camera = new Camera(game) { FarClip = 500f, NearClip = 1f, Aspect = game.ClientSize.Width / game.ClientSize.Height, FOV = (float)Common.degToRad(60) };
                        game.CursorVisible = false;
                    };

                game.Resize += (sender, e) =>
                    {
                        game.ClientSize = new Size(800, 600);
                        GL.Viewport(0, 0, game.Width, game.Height);
                        centerX = game.X + game.Width / 2;
                        centerY = game.Y + game.Height / 2;
                    };

                //Things that need to run every single time there's an update loop, whether its inbetween frames or not.
                game.UpdateFrame += (sender, e) =>
                {
                    if (!init)
                    {
                        camera.CamY = -Collision.ClosestXZ(camera.Position, 10).Position.Y + Globals.CameraHeightOffset; //Set height so that you're on the land - only on the first loop.
                        init = true;
                    }
                    Vec3 oldPos = camera.Position;
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    if (game.Keyboard[Key.W])
                    {
                        camera.MoveForward();
                    }
                    if (game.Keyboard[Key.S])
                    {
                        camera.MoveBackward();
                    }
                    if (game.Keyboard[Key.A])
                    {
                        camera.MoveLeft();
                    }
                    if (game.Keyboard[Key.D])
                    {
                        camera.MoveRight();
                    }
                    if (game.Keyboard[Key.Space])
                    {
                        camera.MoveDown();
                    }
                    if (game.Keyboard[Key.LControl])
                    {
                        camera.MoveUp();
                    }
                    mouseDelta = (System.Windows.Forms.Cursor.Position - new Size(centerX, centerY)); //Windows Only
                    camera.Tilt((float)(mouseDelta.Y / game.RenderFrequency));
                    camera.Turn((float)(mouseDelta.X / game.RenderFrequency));
                    System.Windows.Forms.Cursor.Position = new Point(centerX, centerY); //Windows Only

                    if (camlock)
                    {
                        ICollisionObject co = Collision.ClosestXZ(camera.Position, 10);
                        if (co != null)
                        {
                            if (camera.CamY - Globals.CameraHeightOffset - co.Position.Y > 5) camera.Position = oldPos; //The player is trying to climb something far too steep, stop them.
                            else camera.CamY = -co.Position.Y + Globals.CameraHeightOffset; //Your Y movement is acceptable - either you're going down, straight, or up a reasonable incline, so go ahead.
                        }
                        else camera.Position = oldPos; //Prevent leaving the area where there are collision objects to walk on
                    }
                };

                game.KeyPress += (sender, e) =>
                    {
                        if (e.KeyChar == '`')
                        {
                            switch (Globals.DrawMode)
                            {
                                case PrimitiveType.Polygon:
                                    Globals.DrawMode = PrimitiveType.Lines;
                                    break;
                                case PrimitiveType.Lines:
                                    Globals.DrawMode = PrimitiveType.Points;
                                    break;
                                case PrimitiveType.Points:
                                    Globals.DrawMode = PrimitiveType.Triangles;
                                    break;
                                case PrimitiveType.Triangles:
                                    Globals.DrawMode = PrimitiveType.Polygon;
                                    break;
                                default:
                                    Globals.DrawMode = PrimitiveType.Polygon;
                                    break;
                            }
                            Console.WriteLine("Draw mode is now " + Globals.DrawMode);
                        }
                        if (e.KeyChar == '\\')
                        {
                            switch (Globals.PolygonMode)
                            {
                                case PolygonMode.Fill:
                                    Globals.PolygonMode = PolygonMode.Line;
                                    break;
                                case PolygonMode.Line:
                                    Globals.PolygonMode = PolygonMode.Point;
                                    break;
                                case PolygonMode.Point:
                                    Globals.PolygonMode = PolygonMode.Fill;
                                    break;
                                default:
                                    Globals.PolygonMode = PolygonMode.Fill;
                                    break;
                            }
                            Console.WriteLine("Polygon mode is now " + Globals.PolygonMode);
                        }
                        if (e.KeyChar == 'i')
                        {
                            Console.WriteLine("Refresh: " + game.RenderFrequency);
                            Console.WriteLine("Visual Groups: " + scene.Visuals.OfType<VisualGroup>().Count());
                            Console.WriteLine("Cubes: " + scene.Visuals.OfType<VisualGroup>().Select(vg => vg.Visuals.OfType<Cube>().Count()).Sum());
                            Console.WriteLine("Boxes: " + Collision.CollisionObjects.OfType<BoundingBox>().Count());
                            Console.WriteLine("Position: " + camera.Position);
                            Console.WriteLine("Draw Mode: " + Globals.DrawMode);
                            Console.WriteLine("Polygon Mode: " + Globals.PolygonMode);
                            Console.WriteLine("Vertex Grouping: " + (Globals.VertexGroups ? "enabled" : "disabled") + ".");
                        }
                        if (e.KeyChar == '/')
                        {
                            Globals.VertexGroups = !Globals.VertexGroups;
                            Console.WriteLine("Vertex grouping is now " + (Globals.VertexGroups ? "enabled" : "disabled") + ".");
                        }
                        if (e.KeyChar == 'l')
                        {
                            lighting = !lighting;
                            Console.WriteLine("Lighting is now " + (lighting ? "enabled" : "disabled") + ".");
                        }
                        if (e.KeyChar == 'f')
                        {
                            camlock = !camlock;
                            Console.WriteLine("Camera is now " + (camlock ? "locked" : "free") + ".");
                        }
                        /*if (e.KeyChar == 'l') //place a boxlight in front of the camera.
                        {
                            BoxLight bl = new BoxLight();
                            bl.Scale(10);
                            bl.SetPosition(-camera.Position + -camera.Look * 100);
                            scene.AddVisual(bl);
                        }*/
                        /*if (e.KeyChar == 'e')
                        {
                            ICollisionObject co = new Ray(camera.Position + camera.Look * 10, camera.Look).Cast(10f);
                            if (co != null)
                            {
                                Console.WriteLine("#" + co.ID + " - " + co.Position.ToString());
                                cursorCube.SetPosition(-co.Position);
                            }
                            else Console.WriteLine("No hit.");
                        }*/
                        if (e.KeyChar == 'e') //lower terrain at cursor
                        {
                            if (collisionCursor != null && collisionCursor.Owner != null && collisionCursor.Owner is Mesh)
                            {
                                Mesh tm = collisionCursor.Owner as Mesh;
                                for (int i = 0; i < tm.Vertices.Count(); i++)
                                {
                                    if (tm.Vertices[i].Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3))
                                    {
                                        tm.Vertices[i] = new Vec3(tm.Vertices[i].X, tm.Vertices[i].Y + camera.Look.Y, tm.Vertices[i].Z);
                                        tm.PositionedVertices[i] = new Vec3(tm.PositionedVertices[i].X, tm.PositionedVertices[i].Y + camera.Look.Y, tm.PositionedVertices[i].Z);
                                        List<BoundingBox> boxes = tm.CollisionBoxes.Where(b => b.OriginalPosition.Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3)).ToList();
                                        foreach (BoundingBox b in boxes)
                                        {
                                            b.OriginalPosition = new Vec3(b.OriginalPosition.X, b.OriginalPosition.Y + camera.Look.Y / 15, b.OriginalPosition.Z);
                                            b.Position = new Vec3(b.Position.X, b.Position.Y + camera.Look.Y / 15, b.Position.Z);
                                        }
                                    }
                                }
                            }
                        }
                        if (e.KeyChar == 'q') //raise terrain at cursor
                        {
                            if (collisionCursor != null && collisionCursor.Owner != null && collisionCursor.Owner is Mesh)
                            {
                                Mesh tm = collisionCursor.Owner as Mesh;
                                for (int i = 0; i < tm.Vertices.Count(); i++)
                                {
                                    if (tm.Vertices[i].Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3))
                                    {
                                        tm.Vertices[i] = new Vec3(tm.Vertices[i].X, tm.Vertices[i].Y - camera.Look.Y, tm.Vertices[i].Z);
                                        tm.PositionedVertices[i] = new Vec3(tm.PositionedVertices[i].X, tm.PositionedVertices[i].Y - camera.Look.Y, tm.PositionedVertices[i].Z);
                                        List<BoundingBox> boxes = tm.CollisionBoxes.Where(b => b.OriginalPosition.Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3)).ToList();
                                        foreach (BoundingBox b in boxes)
                                        {
                                            b.OriginalPosition = new Vec3(b.OriginalPosition.X, b.OriginalPosition.Y - camera.Look.Y / 15, b.OriginalPosition.Z);
                                            b.Position = new Vec3(b.Position.X, b.Position.Y - camera.Look.Y / 15, b.Position.Z);
                                        }
                                    }
                                }
                            }
                        }
                        if (e.KeyChar == 't') //tunnel into terrain at cursor
                        {
                            if (collisionCursor != null && collisionCursor.Owner != null && collisionCursor.Owner is Mesh)
                            {
                                Mesh tm = collisionCursor.Owner as Mesh;
                                List<Vec3> newVertices = new List<Vec3>();
                                List<Vec3> newPositionedVertices = new List<Vec3>();
                                List<uint> newIndicesOnly = new List<uint>();
                                int where = 0;
                                bool foundOne = false;
                                for (int i = 0; i < tm.Vertices.Count(); i++)
                                {
                                    if (tm.Vertices[i].Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3) && !foundOne)
                                    {
                                        where = i;
                                        foundOne = true;
                                        Vec3 temp;
                                        BoundingBox b;
                                        newVertices.Add(new Vec3(tm.Vertices[i].X + camera.Look.X, tm.Vertices[i].Y, tm.Vertices[i].Z + camera.Look.Z));
                                        newPositionedVertices.Add(temp = new Vec3(tm.PositionedVertices[i].X + camera.Look.X, tm.PositionedVertices[i].Y, tm.PositionedVertices[i].Z + camera.Look.Z));
                                        b = new BoundingBox(temp, tm.CollisionBoxes.First().Size);
                                        tm.CollisionBoxes.Add(b);
                                        Collision.CollisionObjects.Add(b);
                                        uint ll = (uint)(tm.Vertices[i].X + tm.Vertices[i].Z);
                                        uint lr = (uint)((tm.Vertices[i].X + camera.Look.X) + tm.Vertices[i].Z);
                                        uint tl = (uint)(tm.Vertices[i].X + (tm.Vertices[i].Z + camera.Look.Z));
                                        uint tr = (uint)((tm.Vertices[i].X + camera.Look.X) + (tm.Vertices[i].Z + camera.Look.Z));
                                        newIndicesOnly.Add(tl);
                                        newIndicesOnly.Add(lr);
                                        newIndicesOnly.Add(ll);
                                        newIndicesOnly.Add(tl);
                                        newIndicesOnly.Add(tr);
                                        newIndicesOnly.Add(lr);
                                    }
                                    else
                                    {
                                        newVertices.Add(tm.Vertices[i]);
                                        newPositionedVertices.Add(tm.PositionedVertices[i]);
                                    }
                                }
                                tm.Vertices = newVertices;
                                tm.PositionedVertices = newPositionedVertices;
                                List<uint> indices = tm.Indices.ToList();
                                indices.Insert(where, newIndicesOnly[5]);
                                indices.Insert(where, newIndicesOnly[4]);
                                indices.Insert(where, newIndicesOnly[3]);
                                indices.Insert(where, newIndicesOnly[2]);
                                indices.Insert(where, newIndicesOnly[1]);
                                indices.Insert(where, newIndicesOnly[0]);
                                tm.Indices = indices;
                            }
                        }
                    };

                #region collision command skeleton
                //Collision command skeleton
                /*
                            if (collisionCursor != null && collisionCursor.Owner != null && collisionCursor.Owner is Mesh)
                            {
                                Mesh tm = collisionCursor.Owner as Mesh;
                                for (int i = 0; i < tm.Vertices.Count(); i++)
                                {
                                    if (tm.Vertices[i].Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3))
                                    {
                                        //adjust vertices[i] and positionedvertices[i]
                                        List<Box> boxes = tm.CollisionBoxes.Where(b => b.OriginalPosition.Inside(collisionCursor.OriginalPosition - collisionCursor.CollisionRadius * 3, collisionCursor.OriginalPosition + collisionCursor.CollisionRadius * 3)).ToList();
                                        foreach (Box b in boxes)
                                        {
                                            //adjust box originalposition and position
                                        }
                                    }
                                }
                            }
                 */
                #endregion

                //Put things that need to run every frame or often, but not necessarily every update loop, in addition to drawing itself.
                game.RenderFrame += (sender, e) =>
                    {
                        Globals.RenderCount = 0;
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        GL.MatrixMode(MatrixMode.Modelview);
                        camera.RecalculateViewMatrix();
                        Matrix4 m = camera.View;
                        GL.LoadMatrix(ref m);
                        GL.MatrixMode(MatrixMode.Projection);
                        Matrix4 p = camera.Perspective;
                        GL.LoadMatrix(ref p);
                        if (Globals.DrawMode == PrimitiveType.Polygon) GL.PolygonMode(MaterialFace.FrontAndBack, Globals.PolygonMode);
                        GL.Enable(EnableCap.DepthTest);
                        if (lighting)
                        {
                            GL.ShadeModel(ShadingModel.Smooth);
                            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0, 0, 0, 1 });
                            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1, 1, 1, 1 });
                            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1, 1, 1, 1 });
                            //GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0, 6000, 0, 1 });
                            GL.Enable(EnableCap.Lighting);
                            GL.Enable(EnableCap.Light0);
                            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0, 0, 0, 1 });
                            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 1, 1, 1, 1 });
                            GL.Enable(EnableCap.ColorMaterial);
                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0, 0, 0, 1 });
                            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { .1f, .1f, .1f, 1 });
                        }
                        scene.Draw();
                        //cube.DrawAtPosition();
                        /*
                        List<Vec3> vertices = terrain.ToVertexData();
                        GL.Begin(Globals.DrawMode);
                        foreach (Vec3 v in vertices)
                        {
                            GL.Color4(((byte)v.Y).HeightColor(1));
                            GL.Vertex3(v);
                            GL.Normal3(v.Normalize());
                        }
                        GL.End();*/
                        game.SwapBuffers();
                        if (lighting)
                        {
                            GL.Disable(EnableCap.ColorMaterial);
                            GL.Disable(EnableCap.Light0);
                            GL.Disable(EnableCap.Lighting);
                        }
                        GL.Disable(EnableCap.DepthTest);
                        //Console.WriteLine("Cubes Rendered: " + Globals.RenderCount);
                        #region Cursor Update

                        collisionCursor = new Ray(camera.Position + camera.Look * 3, camera.Look).Cast(15f);
                        if (collisionCursor != null) cursorCube.SetPosition(collisionCursor.Position);
                        else cursorCube.SetPosition(Vec3.Zero);

                        #endregion
                    };

                game.Run(60);
            }
        }

        /// <summary>
        /// Produces a cell at the proper size/projection for a 2x2 stitched image with the provided offsets.
        /// </summary>
        /// <param name="x">X "world coordinate" of this cell within the assumed 2x2 stitched image.</param>
        /// <param name="y">Y "world coordinate" of this cell within the assumed 2x2 stitched image.</param>
        /// <returns></returns>
        static Cell Cell(int x, int y)
        {
            return new Cell(seed, width / 8, height / 8, x, y, width, height);
        }

        static Chunk Chunk(int x, int y)
        {
            Chunk c = new Chunk() { Size = ((width / 4) + (height / 4)) };
            c.SetPosition(new Vec3(x * width / 4 + (width / 8), 0, y * height / 4 + (height / 8)));
            c.CullChildren = false;
            c.AddVisual(Cell(x, y).Simplex().ToCubes());
            c.AddVisual(Cell(x + 1, y).Simplex().ToCubes());
            c.AddVisual(Cell(x, y + 1).Simplex().ToCubes());
            c.AddVisual(Cell(x + 1, y + 1).Simplex().ToCubes());
            return c;
        }

        static Chunk Quadrant(int x, int y)
        {
            Chunk c = new Chunk() { Size = ((width / 2) + (height / 2)) };
            c.SetPosition(new Vec3(x * width / 2 + (width / 4), 0, y * height / 2 + (height / 4)));
            c.AddVisual(Chunk(0, 0));
            c.AddVisual(Chunk(1, 0));
            c.AddVisual(Chunk(0, 1));
            c.AddVisual(Chunk(1, 1));
            return c;
        }
    }
}
