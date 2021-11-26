using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Stratigen.Libraries;
using Stratigen.Framework;

namespace Stratigen.Datatypes
{
    public class TexturedMesh : Mesh
    {
        public Texture Texture;
        public Vec2[] TextureCoordinates;

        public override void Draw()
        {
            PreDraw();

            int vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Count() * 3 * sizeof(float)), Vertices.ToArray(), BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            int texture = GL.GenTexture();
            int texCoordBuffer = GL.GenBuffer();

            if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, Texture.Bitmap.Width, Texture.Bitmap.Height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, Texture.Lock());

                GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TextureCoordinates.Count() * 2 * sizeof(float)), TextureCoordinates, BufferUsageHint.StreamRead);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            }

            int indexBuffer = GL.GenBuffer();

            if (Indices == null || Indices.Count() == 0) GL.DrawArrays(DrawMode == null ? Globals.DrawMode : DrawMode.Value, 0, Vertices.Count());
            else
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count() * sizeof(uint)), Indices.ToArray(), BufferUsageHint.StreamDraw);

                GL.DrawElements(DrawMode == null ? Globals.DrawMode : DrawMode.Value, Indices.Count(), DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);

            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteTexture(texture);
            GL.DeleteBuffer(texCoordBuffer);
            GL.DeleteBuffer(indexBuffer);

            PostDraw();
        }

        public override void PreDraw()
        {
            if (Vertices == null) throw new Exception("Vertices list null for mesh #" + ID + " with name \"" + Name + "\".");
            GL.Enable(EnableCap.DepthTest);
            if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.EnableClientState(ArrayCap.TextureCoordArray);
            }
        }

        public override void PostDraw()
        {
            if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
            {
                GL.DisableClientState(ArrayCap.TextureCoordArray);
                GL.Disable(EnableCap.Texture2D);
            }
            GL.Disable(EnableCap.DepthTest);
        }

        public override void DrawAt(Vec3 v)
        {
            PreDraw();

            Vec3[] tempVertices = new Vec3[Vertices.Count()];

            Matrix4 m = Matrix4.CreateTranslation(v);
            for (int i = 0; i < Vertices.Count(); i++)
            {
                tempVertices[i] = Vector3.TransformPosition(Vertices[i], m);
            }

            int vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tempVertices.Count() * 3 * sizeof(float)), tempVertices, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            int texture = GL.GenTexture();
            int texCoordBuffer = GL.GenBuffer();

            if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, Texture.Bitmap.Width, Texture.Bitmap.Height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, Texture.Lock());

                GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TextureCoordinates.Count() * 2 * sizeof(float)), TextureCoordinates, BufferUsageHint.StreamRead);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            }

            int indexBuffer = GL.GenBuffer();

            if (Indices == null || Indices.Count() == 0) GL.DrawArrays(DrawMode == null ? Globals.DrawMode : DrawMode.Value, 0, Vertices.Count());
            else
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count() * sizeof(uint)), Indices.ToArray(), BufferUsageHint.StreamDraw);

                GL.DrawElements(DrawMode == null ? Globals.DrawMode : DrawMode.Value, Indices.Count(), DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);

            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteTexture(texture);
            GL.DeleteBuffer(texCoordBuffer);
            GL.DeleteBuffer(indexBuffer);

            PostDraw();
        }

        protected override void DrawAtPosition()
        {
            PreDraw();

            int vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PositionedVertices.Count() * 3 * sizeof(float)), PositionedVertices.ToArray(), BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            int texture = GL.GenTexture();
            int texCoordBuffer = GL.GenBuffer();

            if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, Texture.Bitmap.Width, Texture.Bitmap.Height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, Texture.Lock());

                GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TextureCoordinates.Count() * 2 * sizeof(float)), TextureCoordinates, BufferUsageHint.StreamRead);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            }

            int indexBuffer = GL.GenBuffer();

            if (Indices == null || Indices.Count() == 0) GL.DrawArrays(DrawMode == null ? Globals.DrawMode : DrawMode.Value, 0, Vertices.Count());
            else
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count() * sizeof(uint)), Indices.ToArray(), BufferUsageHint.StreamDraw);

                GL.DrawElements(DrawMode == null ? Globals.DrawMode : DrawMode.Value, Indices.Count(), DrawElementsType.UnsignedInt, 0);
            }

            GL.DisableVertexAttribArray(0);

            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteTexture(texture);
            GL.DeleteBuffer(texCoordBuffer);
            GL.DeleteBuffer(indexBuffer);

            PostDraw();
        }

        public override void DrawPositioned()
        {
            if (Position != _lastPosition || PositionedVertices == null)
            {
                if (PositionedVertices == null) PositionedVertices = new List<Vec3>();
                _lastPosition = Position;
                Matrix4 m = Matrix4.CreateTranslation(Position);
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    PositionedVertices[i] = Vector3.TransformPosition(Vertices[i], m);
                    if (TextureCoordinates != null && TextureCoordinates.Count() > 0)
                        TextureCoordinates[i] = new Vec2((float)((double)Vertices[i].X / Texture.Width), (float)((double)Vertices[i].Z / Texture.Height));
                }
                if (CollisionBoxes != null && CollisionBoxes.Count > 0)
                {
                    foreach (BoundingBox b in CollisionBoxes)
                    {
                        b.Position = Vector3.TransformPosition(b.Position, m);
                    }
                }
            }
            DrawAtPosition();
        }

        public TexturedMesh Connect(TexturedMesh tm, Vec2 axis)
        {
            if ((Width != tm.Width && axis.X == 1) || (Height != tm.Height && axis.Y == 1)) throw new ArgumentOutOfRangeException("tm");
            if (axis.X == 1 && axis.Y == 0)
            {
                int w = Width;
                int h = Height;
                Bitmap texture = new Bitmap(w - 1, 6);
                Vec3[] sourceVertices = Vertices.Where(v => v.X == w - 1).ToArray();
                Vec3[] destVertices = tm.Vertices.Where(v => v.X == 0).ToArray();
                Vec3[] vertices = new Vec3[w * 6];
                Vec2[] texcoords = new Vec2[w * 6];
                int counter = 0;
                for (int i = 0; i + 1 < w; i++)
                {
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)destVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(destVertices[i + 1].X, destVertices[i + 1].Y, destVertices[i + 1].Z); //di+1
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)destVertices[i + 1].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X - w, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z); //si+1
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)sourceVertices[i + 1].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i].X - w, sourceVertices[i].Y, sourceVertices[i].Z); //si
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)sourceVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)destVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X - w, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z); //si+1
                    texcoords[counter] = new Vec2(counter / w, 0);
                    texture.TrySetPixel(0, counter / h, ((byte)sourceVertices[i + 1].Y).HeightColor());
                }
                string texturePath = "Data/Texture-TEMP-conX-" + DateTime.Now.Ticks + "-" + Program.random.Next() + ".png";
                System.IO.File.Delete(texturePath);
                texture.Save(texturePath, ImageFormat.Png);
                //texture.Save(texturePath, ImageFormat.Png);
                TexturedMesh nm = new TexturedMesh { Vertices = vertices.ToList(), TextureCoordinates = texcoords, Texture = new Texture(texturePath) };
                nm.SetPosition(tm.Position);
                return nm;
            }
            else if (axis.Y == 1 && axis.X == 0)
            {
                int w = Width;
                int h = Height;
                Bitmap texture = new Bitmap(6, h - 1);
                Vec3[] sourceVertices = Vertices.Where(v => v.Z == 0).ToArray();
                Vec3[] destVertices = tm.Vertices.Where(v => v.Z == h - 1).ToArray();
                Vec3[] vertices = new Vec3[h * 6];
                Vec2[] texcoords = new Vec2[h * 6];
                int counter = 0;
                for (int i = 0; i + 1 < h; i++)
                {
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    //Vec2 c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)destVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(destVertices[i + 1].X, destVertices[i + 1].Y, destVertices[i + 1].Z); //di+1
                    //c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)destVertices[i + 1].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z + h); //si+1
                    //c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)sourceVertices[i + 1].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i].X, sourceVertices[i].Y, sourceVertices[i].Z + h); //si
                    //c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)sourceVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    //c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)destVertices[i].Y).HeightColor());
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z + h); //si+1
                    //c = Maths.EuclideanAddress(counter, h).Flip();
                    texcoords[counter] = new Vec2(0, counter / h);
                    texture.TrySetPixel(counter / w, 0, ((byte)sourceVertices[i + 1].Y).HeightColor());
                }
                string texturePath = "Data/Texture-TEMP-conY-" + DateTime.Now.Ticks + "-" + Program.random.Next() + ".png";
                System.IO.File.Delete(texturePath);
                texture.Save(texturePath, ImageFormat.Png);
                TexturedMesh nm = new TexturedMesh { Vertices = vertices.ToList(), TextureCoordinates = texcoords, Texture = new Texture(texturePath) };
                nm.SetPosition(tm.Position);
                return nm;
            }
            else throw new ArgumentOutOfRangeException("axis");
        }

        public new static VisualGroup Stitch(Cell c00, Cell c10, Cell c01, Cell c11, string name)
        {
            TexturedMesh m00 = c00.ToTexturedMesh(name + "00");
            m00.CollisionBoxes.ForEach(b => b.Position = new Vec3(m00.Position.X + b.OriginalPosition.X - c00.Width * c00.X, m00.Position.Y + b.OriginalPosition.Y, m00.Position.Z + b.OriginalPosition.Z - c00.Height * c00.Y));
            TexturedMesh m10 = c10.ToTexturedMesh(name + "10");
            m10.CollisionBoxes.ForEach(b => b.Position = new Vec3(m10.Position.X + b.OriginalPosition.X - c10.Width * c10.X, m10.Position.Y + b.OriginalPosition.Y, m10.Position.Z + b.OriginalPosition.Z - c10.Height * c10.Y));
            TexturedMesh m01 = c01.ToTexturedMesh(name + "01"); 
            m01.CollisionBoxes.ForEach(b => b.Position = new Vec3(m01.Position.X + b.OriginalPosition.X - c01.Width * c01.X, m01.Position.Y + b.OriginalPosition.Y, m01.Position.Z + b.OriginalPosition.Z - c01.Height * c01.Y));
            TexturedMesh m11 = c11.ToTexturedMesh(name + "11");
            m11.CollisionBoxes.ForEach(b => b.Position = new Vec3(m11.Position.X + b.OriginalPosition.X - c11.Width * c11.X, m11.Position.Y + b.OriginalPosition.Y, m11.Position.Z + b.OriginalPosition.Z - c11.Height * c11.Y));
            return Stitch(m00, m10, m01, m11);
        }

        public static VisualGroup Stitch(TexturedMesh m00, TexturedMesh m10, TexturedMesh m01, TexturedMesh m11)
        {
            VisualGroup vg = new VisualGroup();
            vg.AddVisual(m00);
            vg.AddVisual(m10);
            vg.AddVisual(m01);
            vg.AddVisual(m11);
            vg.AddVisual(m00.Connect(m10, Vec2.UnitX));
            vg.AddVisual(m01.Connect(m11, Vec2.UnitX));
            vg.AddVisual(m01.Connect(m00, Vec2.UnitY));
            vg.AddVisual(m11.Connect(m10, Vec2.UnitY));
            vg.AddVisual(TexturedMesh.Cap(m00, m10, m01, m11));
            return vg;
        }

        private static TexturedMesh Cap(TexturedMesh m00, TexturedMesh m10, TexturedMesh m01, TexturedMesh m11)
        {
            int w = 2;
            int h = 2;
            Vec3 pos = m00.Vertices.Single(v => v.X == m00.Width - 1 && v.Z == m00.Height - 1);
            Vec3 v00 = new Vec3(0, pos.Y, 0);
            Vec3 v01 = new Vec3(0, m01.Vertices.Single(v => v.X == m01.Width - 1 && v.Z == 0).Y, 1);
            Vec3 v10 = new Vec3(1, m10.Vertices.Single(v => v.X == 0 && v.Z == m10.Height - 1).Y, 0);
            Vec3 v11 = new Vec3(1, m11.Vertices.Single(v => v.X == 0 && v.Z == 0).Y, 1);
            Vec3[] vertices = new Vec3[6]
            {
                v01,
                v11,
                v10,
                v00,
                v01,
                v10,
            };
            Vec2[] texcoords = new Vec2[6]
            {
                new Vec2(v01.X / w, v01.Z / h),
                new Vec2(v11.X / w, v11.Z / h),
                new Vec2(v10.X / w, v10.Z / h),
                new Vec2(v00.X / w, v00.Z / h),
                new Vec2(v01.X / w, v01.Z / h),
                new Vec2(v10.X / w, v10.Z / h),
            };
            Bitmap texture = new Bitmap(2, 2);
            texture.SetPixel(0, 0, ((byte)v00.Y).HeightColor());
            texture.SetPixel(0, 1, ((byte)v01.Y).HeightColor());
            texture.SetPixel(1, 0, ((byte)v10.Y).HeightColor());
            texture.SetPixel(1, 1, ((byte)v11.Y).HeightColor());
            string texturePath = "Data/Texture-TEMP-cap-" + DateTime.Now.Ticks + ".png";
            texture.Save(texturePath, ImageFormat.Png);
            TexturedMesh nm = new TexturedMesh { Vertices = vertices.ToList(), TextureCoordinates = texcoords, Texture = new Texture(texturePath)  };
            nm.SetPosition(new Vec3(pos.X, 0, pos.Z));
            return nm;
        }
    }
}
