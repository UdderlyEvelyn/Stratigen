using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stratigen.Datatypes;
using Stratigen.Framework;
//using BoundingBox = Stratigen.Datatypes.BoundingBox;
using Model = Stratigen.Datatypes.Model;
using Ray = Microsoft.Xna.Framework.Ray;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.Drawing.Imaging;
using Color = Microsoft.Xna.Framework.Color;
using Font = Stratigen.Datatypes.Font;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.IO;

namespace Stratigen.Libraries
{
    public static class XNA
    {
        #region Controls
        public static KeyboardState lastKeyboardState;
        public static MouseState lastMouseState;
        public static GamePadState lastGamePadState;

        public static bool KeyPressed(this KeyboardState ks, Keys key)
        {
            bool result = ks.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
            lastKeyboardState = ks;
            return result;
        }

        public static bool ButtonPressed(this MouseState ms, MouseButton button)
        {
            bool result = false;
            switch (button)
            {
                case MouseButton.Left:
                    result = ms.LeftButton.HasFlag(ButtonState.Pressed) && lastMouseState.LeftButton.HasFlag(ButtonState.Released);
                    break;
                case MouseButton.Right:
                    result = ms.RightButton.HasFlag(ButtonState.Pressed) && lastMouseState.RightButton.HasFlag(ButtonState.Released);
                    break;
                case MouseButton.Middle:
                    result = ms.MiddleButton.HasFlag(ButtonState.Pressed) && lastMouseState.MiddleButton.HasFlag(ButtonState.Released);
                    break;
                case MouseButton.ScrollUp:
                    result = ms.ScrollWheelValue < 0;
                    break;
                case MouseButton.ScrollDown:
                    result = ms.ScrollWheelValue > 0;
                    break;
            }
            lastMouseState = ms;
            return result;            
        }

        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            ScrollUp,
            ScrollDown,
        }

        #endregion

        #region 2D
        public static Texture2D TemporaryTexture;
        public static Texture2D RedTexture;
        public static Texture2D BlueTexture;
        public static Texture2D GreenTexture;
        public static Texture2D WhiteTexture;
        public static Texture2D BlackTexture;
        public static Color DrawColor;

        public static void Initialize2D(GraphicsDevice graphicsDevice)
        {
            RedTexture = new Texture2D(graphicsDevice, 1, 1);
            GreenTexture = new Texture2D(graphicsDevice, 1, 1);
            BlueTexture = new Texture2D(graphicsDevice, 1, 1);
            WhiteTexture = new Texture2D(graphicsDevice, 1, 1);
            BlackTexture = new Texture2D(graphicsDevice, 1, 1);
            TemporaryTexture = new Texture2D(graphicsDevice, 1, 1);
            RedTexture.SetColor(Color.Red);
            GreenTexture.SetColor(Color.Green);
            BlueTexture.SetColor(Color.Blue);
            WhiteTexture.SetColor(Color.White);
            BlackTexture.SetColor(Color.Black);
        }

        public static void SetColor(this Texture2D target, Color color)
        {
            lock (target) target.SetData(new Color[] { color });
        }

        public static void DrawImage(this SpriteBatch spriteBatch, Vec2 position, string path)
        {
            if (path == null) return; //ABORT!
            lock (TemporaryTexture)
            {
                TemporaryTexture = Globals.Kernel.Content.Load<Texture2D>(path);
                spriteBatch.Draw(TemporaryTexture, position.ToXNA(), Color.White);
            }
        }

        public static void DrawTexture(this SpriteBatch spriteBatch, Vec2 position, Texture2D texture)
        {
            spriteBatch.Draw(texture, position.ToXNA(), Color.White);
        }

        public static void DrawSprite(this SpriteBatch spriteBatch, Sprite sprite)
        {
            if (sprite.Scale != 1)
            {
                spriteBatch.Draw(sprite.Data, new Rectangle(sprite.Position.Xi, sprite.Position.Yi, (int)(sprite.Data.Width * sprite.Scale), (int)(sprite.Data.Height * sprite.Scale)), Color.White);
            }
            else spriteBatch.Draw(sprite.Data, sprite.Position.ToXNA(), Color.White);
        }

        public static void DrawPixel(this SpriteBatch spriteBatch, Vec2 position, Color c)
        {
                if (c == Color.Red) spriteBatch.Draw(RedTexture, position.ToXNA(), c);
                else if (c == Color.Green) spriteBatch.Draw(GreenTexture, position.ToXNA(), c);
                else if (c == Color.Blue) spriteBatch.Draw(BlueTexture, position.ToXNA(), c);
                else if (c == Color.White) spriteBatch.Draw(WhiteTexture, position.ToXNA(), c);
                else if (c == Color.Black) spriteBatch.Draw(BlackTexture, position.ToXNA(), c);
        }

        public static Bitmap AsImage(this Chunk c)
        {
            Bitmap b = new Bitmap(16, 16);
            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Color co;
                    BlockType bt = c.TopBlock(x, z).Type;
                    if (bt == BlockType.Get("Grass")) co = Color.Green;
                    else if (bt == BlockType.Get("Leaves")) co = Color.DarkGreen;
                    else if (bt == BlockType.Get("Log")) co = Color.Brown;
                    else co = Color.White;
                    b.SetPixel(x, z, System.Drawing.Color.FromArgb(co.R, co.G, co.B));
                }
            }
            return b;
        }

        /// <summary>
        /// (expects a list of chunks in a 5x5 arrangement)
        /// </summary>
        /// <param name="chunks"></param>
        /// <returns></returns>
        public static Bitmap AsImage(this List<Chunk> chunks, Vec2 origin)
        {
            Bitmap b = new Bitmap(48, 48);
            Chunk c00 = null;
            try { c00 = chunks.Single(c => c.WorldPosition == origin - 1); }
            catch { /*Pass*/ }
            Chunk c10 = null;
            try { c10 = chunks.Single(c => c.WorldPosition == origin - new Vec2(0, 1)); }
            catch { /*Pass*/ }
            Chunk c01 = null;
            try { c01 = chunks.Single(c => c.WorldPosition == origin - new Vec2(1, 0)); }
            catch { /*Pass*/ }
            Chunk c11 = null;
            try { c11 = chunks.Single(c => c.WorldPosition == origin); }
            catch { /*Pass*/ }
            Chunk c20 = null;
            try { c20 = chunks.Single(c => c.WorldPosition == origin + new Vec2(1, -1)); }
            catch { /*Pass*/ }
            Chunk c02 = null;
            try { c02 = chunks.Single(c => c.WorldPosition == origin + new Vec2(-1, 1)); }
            catch { /*Pass*/ }
            Chunk c21 = null;
            try { c21 = chunks.Single(c => c.WorldPosition == origin + new Vec2(1, 0)); }
            catch { /*Pass*/ }
            Chunk c12 = null;
            try { c12 = chunks.Single(c => c.WorldPosition == origin + new Vec2(0, 1)); }
            catch { /*Pass*/ }
            Chunk c22 = null;
            try { c22 = chunks.Single(c => c.WorldPosition == origin + 1); }
            catch { /*Pass*/ }

            for (int z = 0; z < 16; z++)
            {
                for (int x = 0; x < 16; x++)
                {
                    System.Drawing.Color co = c00.GetTopColor(x, z);
                    b.SetPixel(x, z, co);
                    co = c10.GetTopColor(x, z);
                    b.SetPixel(x + 16, z, co);
                    co = c20.GetTopColor(x, z);
                    b.SetPixel(x + 32, z, co);
                    co = c01.GetTopColor(x, z);
                    b.SetPixel(x, z + 16, co);
                    co = c11.GetTopColor(x, z);
                    b.SetPixel(x + 16, z + 16, co);
                    co = c21.GetTopColor(x, z);
                    b.SetPixel(x + 32, z + 16, co);
                    co = c02.GetTopColor(x, z);
                    b.SetPixel(x, z + 32, co);
                    co = c12.GetTopColor(x, z);
                    b.SetPixel(x + 16, z + 32, co);
                    co = c22.GetTopColor(x, z);
                    b.SetPixel(x + 32, z + 32, co);
                }
            }

            return b;
        }

        public static Texture2D ToTexture(this List<Chunk> chunks, GraphicsDevice graphicsDevice, Vec2 origin)
        {
            Bitmap b = chunks.AsImage(origin);
            MemoryStream s = new MemoryStream();
            b.Save(s, ImageFormat.Bmp);
            s.Flush();
            s.Seek(0, SeekOrigin.Begin);
            Texture2D ret = Texture2D.FromStream(graphicsDevice, s);
            s.Close();
            return ret;
        }

        public static System.Drawing.Color GetTopColor(this Chunk c, int x, int z)
        {
            if (c == null) return System.Drawing.Color.Black;
            System.Drawing.Color co;
            Block b = c.TopBlock(x, z);
            if (b.Type == BlockType.Get("Grass")) co = System.Drawing.Color.Green;
            else if (b.Type == BlockType.Get("Leaves")) co = System.Drawing.Color.DarkGreen;
            else if (b.Type == BlockType.Get("Log")) co = System.Drawing.Color.Brown;
            else co = System.Drawing.Color.White;
            byte ao = (byte)(b.PosYVertices.Sum(v => v.AmbientOcclusion) * 4);
            co = System.Drawing.Color.FromArgb(co.R + ao, co.G + ao, co.B + ao);
            return co;
        }

        #endregion

        public static bool Collides(this BoundingBox thisBox, BoundingBox otherBox)
        {
            if (thisBox.Min.X > otherBox.Max.X ||
                thisBox.Min.Z > otherBox.Max.Z ||
                thisBox.Max.X < otherBox.Min.X ||
                thisBox.Max.Z < otherBox.Max.Z)
                return false; //Can't be colliding if any of these pass.
            return true; //If none of these range checks pass, it's colliding.
        }

        public static void DrawLine(this GraphicsDevice graphicsDevice, Vec3 start, Vec3 end, Color color)
        {
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.LineList, new ColorVertex[2]
            {
                new ColorVertex(start, color),
                new ColorVertex(end, color),
            }, 0, 1, ColorVertex.VertexDeclaration);
        }

        public static void DrawLine2(this GraphicsDevice graphicsDevice, Camera camera, List<Vec2> points, Color color, int primitiveCountOverride = 0)
        {
            List<ColorVertex> line = new List<ColorVertex>();
            for (int i = 0; i < points.Count; i++) line.Add(new ColorVertex(camera.UnprojectPoint(points[i].ToXNA(), graphicsDevice.Viewport).Position, color));
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.LineStrip, line.ToArray(), 0, primitiveCountOverride == 0 ? points.Count / 2 : primitiveCountOverride);
        }

        public static void DrawLine2(this GraphicsDevice graphicsDevice, Camera camera, Vec2 start, Vec2 end, Color color)
        {
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.LineList, new ColorVertex[2]
            {
                new ColorVertex(camera.UnprojectPoint(start.ToXNA(), graphicsDevice.Viewport).Position, color),
                new ColorVertex(camera.UnprojectPoint(end.ToXNA(), graphicsDevice.Viewport).Position, color),
            }, 0, 1);
        }

        public static void DrawRectangle2(this GraphicsDevice graphicsDevice, Camera camera, Vec2 position, Vec2 extent, Color color)
        {
            graphicsDevice.DrawLine2(camera, new List<Vec2> 
            {
                position, 
                new Vec2(extent.X, position.Y),
                extent,
                new Vec2(position.X, extent.Y),
                position, 
            }, color, 4);
                //position, color);
        }

        public static void DrawRectangle2(this GraphicsDevice graphicsDevice, Camera camera, Vec2 position, int width, int height, Color color)
        {
            DrawRectangle2(graphicsDevice, camera, position, new Vec2(width - 2, height - 2), color);
        }

        public static void DrawFilledRectangle(this GraphicsDevice graphicsDevice, Camera camera, Vec2 position, int width, int height, Color color, Color fillColor)
        {
            DrawRectangle2(graphicsDevice, camera, position, width + 1, position.Yi + height + 1, color);
            for (int y = 1; y < height - 1; y++) DrawLine2(graphicsDevice, camera, new Vec2(position.Xi + 1, position.Yi + y), new Vec2(width - 1, position.Yi + y), fillColor);
        }

        public static void DrawSun(this GraphicsDevice graphicsDevice, Vec3 pos, float size, Color color)
        {
            float halfSize = size / 2;
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.TriangleList,
                    new ColorVertex[6] 
                    {                  
                        new ColorVertex(pos + halfSize * Vec3.PNP, color), //5 7
                        new ColorVertex(pos + halfSize * Vec3.NNN, color), //0 8
                        new ColorVertex(pos + halfSize * Vec3.PNN, color), //4 9
                        new ColorVertex(pos + halfSize * Vec3.PNP, color), //5 16
                        new ColorVertex(pos + halfSize * Vec3.NNP, color), //1 17
                        new ColorVertex(pos + halfSize * Vec3.NNN, color), //0 18
                    },
                    0, 2);
        }

        public static void DrawCube(this GraphicsDevice graphicsDevice, Vec3 min, Vec3 max, Color color)
        {
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.LineList, new ColorVertex[24]
            {
                new ColorVertex(new Vec3(min.X, min.Y, min.Z), color), //NNN
                new ColorVertex(new Vec3(max.X, min.Y, min.Z), color), //PNN
                new ColorVertex(new Vec3(min.X, min.Y, min.Z), color), //NNN
                new ColorVertex(new Vec3(min.X, min.Y, max.Z), color), //NNP
                new ColorVertex(new Vec3(min.X, min.Y, min.Z), color), //NNN
                new ColorVertex(new Vec3(min.X, max.Y, min.Z), color), //NPN
                new ColorVertex(new Vec3(max.X, max.Y, max.Z), color), //PPP
                new ColorVertex(new Vec3(min.X, max.Y, max.Z), color), //NPP
                new ColorVertex(new Vec3(max.X, max.Y, max.Z), color), //PPP
                new ColorVertex(new Vec3(max.X, max.Y, min.Z), color), //PPN
                new ColorVertex(new Vec3(max.X, max.Y, max.Z), color), //PPP
                new ColorVertex(new Vec3(max.X, min.Y, max.Z), color), //PNP
                new ColorVertex(new Vec3(min.X, max.Y, min.Z), color), //NPN
                new ColorVertex(new Vec3(min.X, max.Y, max.Z), color), //NPP
                new ColorVertex(new Vec3(min.X, max.Y, min.Z), color), //NPN
                new ColorVertex(new Vec3(max.X, max.Y, min.Z), color), //PPN
                new ColorVertex(new Vec3(min.X, max.Y, max.Z), color), //NPP
                new ColorVertex(new Vec3(min.X, min.Y, max.Z), color), //NNP
                new ColorVertex(new Vec3(min.X, min.Y, max.Z), color), //NNP
                new ColorVertex(new Vec3(max.X, min.Y, max.Z), color), //PNP
                new ColorVertex(new Vec3(max.X, min.Y, max.Z), color), //PNP
                new ColorVertex(new Vec3(max.X, min.Y, min.Z), color), //PNN
                new ColorVertex(new Vec3(max.X, min.Y, min.Z), color), //PNN
                new ColorVertex(new Vec3(max.X, max.Y, min.Z), color), //PPN
            }, 0, 12, ColorVertex.VertexDeclaration);
        }

        public static void Draw(this GraphicsDevice graphicsDevice, Ray r, float length, Vec3 offset)
        {
            graphicsDevice.DrawUserPrimitives<ColorVertex>(PrimitiveType.LineList, new ColorVertex[2]
            {
                new ColorVertex(r.Position + (Vector3)offset, Color.White),
                new ColorVertex(r.Position + (Vector3)offset + r.Direction * length, Color.Red),
            }, 0, 1, ColorVertex.VertexDeclaration);
        }

        public static void Draw(this GraphicsDevice graphicsDevice, Ray r, float length)
        {
            graphicsDevice.Draw(r, length, Vec3.Zero);
        }

        public static void Draw(this GraphicsDevice graphicsDevice, Box box, Color color)
        {
            graphicsDevice.DrawCube(box.Min, box.Max, color);
        }

        public static Vec3 GetNormal(IVertex a, IVertex b, IVertex c)
        {
            return Vec3.Cross(b.Position - a.Position, c.Position - a.Position).Normalize();
        }

        #region Text

        public static void DrawString(this SpriteBatch batch, Font font, string s, Vec2 position, Color color)
        {
            int offset = 0;
            int line = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\n')
                {
                    line++;
                    offset = 0;
                }
                else
                {
                    Rectangle r = font[s[i]].ToXNA();
                    batch.Draw(font.Texture, new Rectangle((int)(position.X + offset), (int)(position.Y + (line * r.Height * font.Scale)), (int)(r.Width * font.Scale), (int)(r.Height * font.Scale)), r, color);
                    offset += (int)(r.Width * font.Scale + (font.Spacing * r.Width * font.Scale));
                }
            }
        }

        public static void SetColor(this Texture2D target, byte x, byte y, Color color)
        {
            lock (target) target.SetData(new Color[] { color }, Maths.LinearAddress(x, y, target.Bounds.Size.X), 1);
        }

        public static Color[] GetColor(this Texture2D target)
        {
            Color[] data = new Color[target.Bounds.Size.X * target.Bounds.Size.Y];
            lock (target) target.GetData(data);
            return data;
        }
        public static void ReplaceColor(this Texture2D target, Color original, Color desired)
        {
            var colors = target.GetColor();
            var originalCopy = colors.ToArray();
            for (int i = 0; i < colors.Length; i++)
                if (colors[i] == original)
                    colors[i] = desired;
            target.SetData(colors);
        }

        public static void DrawString(this SpriteBatch batch, Font font, string s, Vec2 position)
        {
            DrawString(batch, font, s, position, Color.White);
        }

        public static void DrawShadowedString(this SpriteBatch batch, Font font, string s, Vec2 position, Color color, Vec2 shadowOffset, Color shadowColor)
        {
            DrawString(batch, font, s, shadowOffset, shadowColor);
            DrawString(batch, font, s, position, color);
        }

        public static void DrawShadowedString(this SpriteBatch batch, Font font, string s, Vec2 position, Color color)
        {
            DrawShadowedString(batch, font, s, position, color, position + new Vec2(2, 2), Color.Black);
        }

        public static void DrawShadowedString(this SpriteBatch batch, Font font, string s, Vec2 position)
        {
            DrawShadowedString(batch, font, s, position, Color.White, position + new Vec2(2, 2), Color.Black);
        }

        #endregion

        #region Miscellaneous

        public static Color ToColor(this float f)
        {
            return new Color(f, f, f);
        }

        public static uint[] ToUintArray(this Index[] ixs)
        {
            List<uint> uints = new List<uint>();
            foreach (Index i in ixs)
            {
                uints.Add(i.A);
                uints.Add(i.B);
                uints.Add(i.C);
            }
            return uints.ToArray();
        }

        public static int[] ToIntArray(this Index[] ixs)
        {
            List<int> ints = new List<int>();
            foreach (Index i in ixs)
            {
                ints.Add((int)i.A);
                ints.Add((int)i.B);
                ints.Add((int)i.C);
            }
            return ints.ToArray();
        }

        public static Index[] ToIndexArray(this int[] ints)
        {
            List<Index> ixs = new List<Index>();
            for (int i = 0; i + 3 < ints.Count(); i+=3)
            {
                ixs.Add(new Index((uint)ints[i], (uint)ints[i + 1], (uint)ints[i + 2]));
            }
            return ixs.ToArray();
        }

        #endregion

        public static VertexPositionColor[] Offset(this VertexPositionColor[] vpcs, Vec3 v)
        {
            for (int i = 0; i < vpcs.Count(); i++)
            {
                vpcs[i].Position += (Vector3)v;
            }
            return vpcs;
        }

        /*public static Chunk ToChunk(this Array2<byte> data)
        {
            Chunk c = new Chunk { Blocks = new Block[16, 256, 16] };
            for (int x = 0; x < data.Width; x++)
            {
                if (x > c.Size.X) continue;
                for (int z = 0; z < data.Height; z++)
                {
                    if (z > c.Size.Z) continue;
                    byte y = data.Get(x, z);
                    List<Block> blocks = new List<Block>();
                    Block b = new Block(new Vec3(x, y, z), Globals.Random.NextDouble() < .5 ? BlockTypeCollection.Get("Grass") : BlockTypeCollection.Get("Stone"));
                    c.Blocks[x, y, z] = b;
                    int interval = (int)(Math.Max(1, b.Size));
                    for (int h = y - interval; h > -1; h -= interval)
                    {
                        b = new Block(new Vec3(x, h, z), Globals.Random.NextDouble() < .5 ? BlockTypeCollection.Get("Grass") : BlockTypeCollection.Get("Stone"));
                        c.Blocks[x, h, z] = b;
                    }
                }
                c.Position = c.Blocks[0, 0, 0].Position;
            }
            return c;
        }*/

        //unoptimized version
        /*public static Chunk ToChunk(this Cell data)
        {
            BlockType bt = BlockType.Get("Grass");
            float blockSize = 1;
            float halfSize = .5f;
            Chunk c = new Chunk { Blocks = new Block[16, 256, 16], WorldPosition = new Vec2(data.X, data.Y) };
            for (int x = 0; x < data.Width; x++)
            {
                if (x > c.Size.X) continue;
                for (int z = 0; z < data.Height; z++)
                {
                    if (z > c.Size.Z) continue;
                    byte y = data.Get(x, z);
                    float ax = x + halfSize + data.X * data.Width;
                    float az = z + halfSize + data.Y * data.Height;
                    c.Blocks[x, y, z] = new Block(new Vec3(ax, y, az), bt, blockSize);
                    int interval = (int)(Math.Max(1, c.Blocks[x, y, z].Size));
                    for (int h = y - interval; h > -1; h -= interval)
                    {
                        c.Blocks[x, h, z] = new Block(new Vec3(ax, h, az), bt, blockSize);
                    }
                }
                c.Position = c.Blocks[0, 0, 0].Position;
                Coordinate co = c.Position.ToCoordinate();
                c.WorldPosition = new Vec2(co.CX, co.CZ);
            }
            return c;
        }*/

        public static Chunk ToChunk(this Cell data, World w)
        {
            Chunk c = new Chunk { Blocks = new Block[16, 256, 16], WorldPosition = new Vec2(data.X, data.Y), World = w };
            BlockType stone = BlockType.Get("Stone");
            BlockType dirt = BlockType.Get("Dirt");
            BlockType grass = BlockType.Get("Grass");
            BlockType log = BlockType.Get("Log");
            BlockType leaves = BlockType.Get("Leaves");
//            BlockType air = BlockType.Get("Air");
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    byte y = data.Get(x, z);
                    float ax = x + .5f + data.X * data.Width;
                    float az = z + .5f + data.Y * data.Height;
                    for (int h = 255; h > -1; h -= 1)
                    {
                        if (h == y) //The top block is grass, or .001% of the time it's a tree.
                        {
                            if (!File.Exists(c.WorldPosition.Xi + "-" + c.WorldPosition.Yi + ".ykch"))
                                if (x > 1 && x < 13 && z > 1 && z < 13 && Globals.Random.NextDouble() <= .001) //if it's at least two blocks into the chunk and passes .001% chance roll
                                {
                                    #region Tree Generation
                                    Vec3 v = Vec3.Zero;
                                    BlockType bt = BlockType.Get("Test");
                                    int treeHeight = Globals.Random.Next(1, 11) + y;
                                    if (treeHeight > 255) treeHeight = 255; //don't go out of bounds
                                    for (int i = y; i <= treeHeight; i++)
                                    {
                                        const int treeHeightForLeaves = 3;
                                        if (treeHeight > y + treeHeightForLeaves && i == treeHeight) //top of tree on a tree that's at least 3 tall
                                        {
                                            c.Blocks[x, i, z] = new Block(v = new Vec3(ax, i, az), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x - 1, i, z] = new Block(v = new Vec3(ax - 1, i, az), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x + 1, i, z] = new Block(v = new Vec3(ax + 1, i, az), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x, i, z - 1] = new Block(v = new Vec3(ax, i, az - 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x, i, z + 1] = new Block(v = new Vec3(ax, i, az + 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x - 1, i, z - 1] = new Block(v = new Vec3(ax - 1, i, az - 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x + 1, i, z - 1] = new Block(v = new Vec3(ax + 1, i, az - 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x + 1, i, z + 1] = new Block(v = new Vec3(ax + 1, i, az + 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x - 1, i, z + 1] = new Block(v = new Vec3(ax - 1, i, az + 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                        }
                                        else if (treeHeight > y + treeHeightForLeaves && i == treeHeight - 1) //layer below top of tree on a tree that's at least 3 tall
                                        {
                                            c.Blocks[x, i, z] = new Block(v = new Vec3(ax, i, az), log);
                                            c.Changes[v] = new Change(v, log.ID);
                                            c.Blocks[x - 1, i, z] = new Block(v = new Vec3(ax - 1, i, az), bt = (Globals.Random.NextDouble() <= .1 ? log : leaves));
                                            c.Changes[v] = new Change(v, bt.ID);
                                            if (c.Blocks[x - 1, i, z].Type == log)
                                            {
                                                c.Blocks[x - 2, i, z] = new Block(v = new Vec3(ax - 2, i, az), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x - 2, i, z + 1] = new Block(v = new Vec3(ax - 2, i, az + 1), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x - 2, i, z - 1] = new Block(v = new Vec3(ax - 2, i, az - 1), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                            }
                                            c.Blocks[x + 1, i, z] = new Block(v = new Vec3(ax + 1, i, az), bt = (Globals.Random.NextDouble() <= .1 ? log : leaves));
                                            c.Changes[v] = new Change(v, bt.ID);
                                            if (c.Blocks[x + 1, i, z].Type == log)
                                            {
                                                c.Blocks[x + 2, i, z] = new Block(v = new Vec3(ax + 2, i, az), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x + 2, i, z + 1] = new Block(v = new Vec3(ax + 2, i, az + 1), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x + 2, i, z - 1] = new Block(v = new Vec3(ax + 2, i, az - 1), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                            }
                                            c.Blocks[x, i, z - 1] = new Block(v = new Vec3(ax, i, az - 1), bt = (Globals.Random.NextDouble() <= .1 ? log : leaves));
                                            c.Changes[v] = new Change(v, bt.ID);
                                            if (c.Blocks[x, i, z - 1].Type == log)
                                            {
                                                c.Blocks[x, i, z - 2] = new Block(v = new Vec3(ax, i, az - 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x + 1, i, z - 2] = new Block(v = new Vec3(ax + 1, i, az - 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x - 1, i, z - 2] = new Block(v = new Vec3(ax - 1, i, az - 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                            }
                                            c.Blocks[x, i, z + 1] = new Block(v = new Vec3(ax, i, az + 1), bt = (Globals.Random.NextDouble() <= .1 ? log : leaves));
                                            c.Changes[v] = new Change(v, bt.ID);
                                            if (c.Blocks[x, i, z + 1].Type == log)
                                            {
                                                c.Blocks[x, i, z + 2] = new Block(v = new Vec3(ax, i, az + 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x + 1, i, z + 2] = new Block(v = new Vec3(ax + 1, i, az + 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                                c.Blocks[x - 1, i, z + 2] = new Block(v = new Vec3(ax - 1, i, az + 2), leaves);
                                                c.Changes[v] = new Change(v, leaves.ID);
                                            }
                                            c.Blocks[x - 1, i, z - 1] = new Block(v = new Vec3(ax - 1, i, az - 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x + 1, i, z - 1] = new Block(v = new Vec3(ax + 1, i, az - 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x + 1, i, z + 1] = new Block(v = new Vec3(ax + 1, i, az + 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                            c.Blocks[x - 1, i, z + 1] = new Block(v = new Vec3(ax - 1, i, az + 1), leaves);
                                            c.Changes[v] = new Change(v, leaves.ID);
                                        }
                                        else
                                        {
                                            c.Blocks[x, i, z] = new Block(new Vec3(ax, i, az), log);
                                            c.Changes[v] = new Change(v, log.ID);
                                            //Branches (10% chance per tree height increment)
                                            if (i > y + 1)
                                            {
                                                if (Globals.Random.NextDouble() <= .1)
                                                {
                                                    switch (Globals.Random.Next(1, 5))
                                                    {
                                                        case 1:
                                                            if (x > 0)
                                                            {
                                                                c.Blocks[x - 1, i, z] = new Block(v = new Vec3(ax - 1, i, az), log);
                                                                c.Changes[v] = new Change(v, log.ID);
                                                            }
                                                            break;
                                                        case 2:
                                                            if (x < 15)
                                                            {
                                                                c.Blocks[x + 1, i, z] = new Block(v = new Vec3(ax + 1, i, az), log);
                                                                c.Changes[v] = new Change(v, log.ID);
                                                            }
                                                            break;
                                                        case 3:
                                                            if (z > 0)
                                                            {
                                                                c.Blocks[x, i, z - 1] = new Block(v = new Vec3(ax, i, az - 1), log);
                                                                c.Changes[v] = new Change(v, log.ID);
                                                            }
                                                            break;
                                                        case 4:
                                                            if (z < 15)
                                                            {
                                                                c.Blocks[x, i, z + 1] = new Block(v = new Vec3(ax, i, az + 1), log);
                                                                c.Changes[v] = new Change(v, log.ID);
                                                            }
                                                            break;

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            c.Blocks[x, h, z] = new Block(new Vec3(ax, h, az), grass);
                        }
                        else if (h > y - 5 && h < y) //The first five blocks (besides the top) are dirt.
                            c.Blocks[x, h, z] = new Block(new Vec3(ax, h, az), dirt);
                        else if (h < y) //Everything after the first five blocks is stone.
                            c.Blocks[x, h, z] = new Block(new Vec3(ax, h, az), stone);
                        //else
                            //c.Blocks[x, h, z] = null;
                    }
                }
                c.Position = new Vec3(data.X, 0, data.Y);
                //c.Position = c.Blocks[0, 0, 0].Position;
            }
            return c;
        }

        /*public static Terrain ToInstances(this Array2<byte> data, Model m)
        {
            Globals.CubeCount = 0;
            Terrain t = new Terrain(data.Width, data.Height, m.Vertices, m.Indices);
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    List<Instance> instances = new List<Instance>();
                    Instance i = m.Instantiate(new Vec3(x, value, y));
                    //i.IsVisible = true;
                    instances.Add(i);
                    int interval = (int)(Math.Max(1, i.Size));
                    for (int h = value - interval; h > -1; h-= interval)
                    {
                        i = m.Instantiate(new Vec3(x, h, y));
                        //i.IsVisible = false;
                        instances.Add(i);
                    }
                    t.Positions.Set(x, y, instances.OrderBy(j => j.Position.Y).ToArray());
                    Globals.CubeCount += instances.Count;
                }
            }
            return t;
        }*/

        /*public static Vertex[] ToVertices(this Array2<byte> data)
        {
            List<Vertex> vertices = new List<Vertex>();
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    vertices.Add(new Vertex(x, value, y));
                    for (int h = value - 1; h > -1; h--)
                    {
                        vertices.Add(new Vertex(x, h, y));
                    }
                }
            }
            return vertices.ToArray();
        }*/

        /*public static void ToInstances(this Array2<byte> data, Model m)
        {
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    m.Instantiate(new Vec3(x, value, y));
                }
            }
        }*/
    }
}
