using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Libraries;
using System.IO;
using Stratigen.Framework;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Stratigen.Datatypes
{
    public class Mesh : IVisualObject
    {
        public Box Bounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name;
        public long ID;
        public List<Vec3> Vertices;
        public List<Vec3> PositionedVertices;
        public List<Vec3> Normals;
        public List<uint> Indices;
        public PrimitiveType? DrawMode = null;
        public List<BoundingBox> CollisionBoxes;
        public Vec3 Position { get; set; }
        public object Tag;
        public int Width;
        public int Height;

        protected Vec3 _lastPosition;

        public virtual void Draw()
        {
            PreDraw();
            if (!Globals.VertexGroups)
            {
                GL.Begin(DrawMode == null ? Globals.DrawMode : DrawMode.Value);
                foreach (uint u in Indices)
                {
                    GL.Color4(((byte)Vertices[(int)u].Y).HeightColor(1));
                    GL.Vertex3(Vertices[(int)u]);
                    GL.Normal3(Vertices[(int)u].Normalize());
                }
                GL.End();
            }
            else
            {
                int vertexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Count() * 3 * sizeof(float)), Vertices.ToArray(), BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

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
                GL.DeleteBuffer(indexBuffer);
            }
            PostDraw();
        }

        public virtual void PreDraw()
        {
            if (Vertices == null) throw new Exception("Vertices list null for mesh #" + ID + " with name \"" + Name + "\".");
        }

        public virtual void PostDraw()
        {

        }

        protected virtual void DrawAtPosition()
        {
            //if (PositionedVertices == null) throw new Exception("Positioned vertices list null for mesh #" + ID + " with name \"" + Name + "\".");

            PreDraw();
            if (!Globals.VertexGroups)
            {
                GL.Begin(DrawMode == null ? Globals.DrawMode : DrawMode.Value);
                foreach (uint u in Indices)
                {
                    GL.Vertex3(Vertices[(int)u]);
                    GL.Normal3(Vertices[(int)u].Normalize());
                    GL.Color4(((byte)Vertices[(int)u].Y).HeightColor(1));
                }
                GL.End();
            }
            else
            {
                int vertexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PositionedVertices.Count() * 3 * sizeof(float)), PositionedVertices.ToArray(), BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

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
                GL.DeleteBuffer(indexBuffer);
            }
            PostDraw();
        }

        public void Scale(float f)
        {
            for (int i = 0; i < Vertices.Count(); i++)
            {
                Vertices[i] *= f;
            }
            //Don't scale the indices, but if you wanted to the below would do it.
            /*for (int i = 0; i < Indices.Count(); i++)
            {
                Indices[i] = (uint)(Indices[i] * f);
            }*/
        }

        public virtual void DrawAt(Vec3 v)
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
            GL.DeleteBuffer(indexBuffer);

            PostDraw();
        }

        public virtual void DrawPositioned()
        {
            if (Position != _lastPosition || PositionedVertices == null) 
            {
                PositionedVertices = new List<Vec3>();
                _lastPosition = Position;
                Matrix4 m = Matrix4.CreateTranslation(Position);
                for (int i = 0; i < Vertices.Count(); i++)
                {
                    PositionedVertices.Add(Vector3.TransformPosition(Vertices[i], m));
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

        public void SetPosition(Vec3 position)
        {
            Position = position;
        }

        public static Mesh FromVTX(string filename)
        {
            List<Vec3> vertexList = new List<Vec3>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new Vec3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])));
            }
            return new Mesh() { Position = Vector3.Zero, Vertices = vertexList };
        }

        public void LoadVTX(string filename)
        {
            List<Vec3> vertexList = new List<Vec3>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new Vec3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])));
            }
            Vertices = vertexList;
        }

        public static Vec3[] VerticesFromVTX(string filename)
        {
            List<Vec3> vertexList = new List<Vec3>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new Vec3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])));
            }
            return vertexList.ToArray();
        }

        public Mesh Connect(Mesh m, Vec2 axis)
        {
            if ((Width != m.Width && axis.X == 1) || (Height != m.Height && axis.Y == 1)) throw new ArgumentOutOfRangeException("m");
            if (axis.X == 1 && axis.Y == 0)
            {
                int w = Width;
                int h = Height;
                Vec3[] sourceVertices = Vertices.Where(v => v.X == w - 1).ToArray();
                Vec3[] destVertices = m.Vertices.Where(v => v.X == 0).ToArray();
                Vec3[] vertices = new Vec3[w * 6];
                int counter = 0;
                for (int i = 0; i+1 < w; i++)
                {
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    vertices[counter++] = new Vec3(destVertices[i + 1].X, destVertices[i + 1].Y, destVertices[i + 1].Z); //di+1
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X - w, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z); //si+1
                    vertices[counter++] = new Vec3(sourceVertices[i].X - w, sourceVertices[i].Y, sourceVertices[i].Z); //si
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X - w, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z); //si+1
                } 
                Mesh nm = new Mesh { Vertices = vertices.ToList() };
                nm.SetPosition(m.Position);
                return nm;
            }
            else if (axis.Y == 1 && axis.X == 0)
            {
                int w = Width;
                int h = Height;
                Vec3[] sourceVertices = Vertices.Where(v => v.Z == 0).ToArray();
                Vec3[] destVertices = m.Vertices.Where(v => v.Z == h - 1).ToArray();
                Vec3[] vertices = new Vec3[h * 6];
                int counter = 0;
                for (int i = 0; i+1 < h; i++)
                {
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    vertices[counter++] = new Vec3(destVertices[i + 1].X, destVertices[i + 1].Y, destVertices[i + 1].Z); //di+1
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z + h); //si+1
                    vertices[counter++] = new Vec3(sourceVertices[i].X, sourceVertices[i].Y, sourceVertices[i].Z + h); //si
                    vertices[counter++] = new Vec3(destVertices[i].X, destVertices[i].Y, destVertices[i].Z); //di
                    vertices[counter++] = new Vec3(sourceVertices[i + 1].X, sourceVertices[i + 1].Y, sourceVertices[i + 1].Z + h); //si+1
                }
                Mesh nm = new Mesh { Vertices = vertices.ToList() };
                nm.SetPosition(m.Position);
                return nm;
            }
            else throw new ArgumentOutOfRangeException("axis");
        }

        private static Mesh Cap(Mesh m00, Mesh m10, Mesh m01, Mesh m11)
        {
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
            Mesh nm = new Mesh { Vertices = vertices.ToList() };
            nm.SetPosition(new Vec3(pos.X, 0, pos.Z));
            return nm;
        }

        public static VisualGroup Stitch(Mesh m00, Mesh m10, Mesh m01, Mesh m11)
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
            vg.AddVisual(Mesh.Cap(m00, m10, m01, m11));
            return vg;
        }

        public static VisualGroup Connect(Mesh m00, Mesh m10, Mesh m01, Mesh m11)
        {
            VisualGroup vg = new VisualGroup();
            vg.AddVisual(m00.Connect(m10, Vec2.UnitX));
            vg.AddVisual(m01.Connect(m11, Vec2.UnitX));
            vg.AddVisual(m01.Connect(m00, Vec2.UnitY));
            vg.AddVisual(m11.Connect(m10, Vec2.UnitY));
            vg.AddVisual(Mesh.Cap(m00, m10, m01, m11));
            return vg;
        }

        public static VisualGroup Stitch(Cell c00, Cell c10, Cell c01, Cell c11, string name)
        {
            Mesh m00 = c00.ToMesh(name + "00");
            Mesh m10 = c10.ToMesh(name + "10");
            Mesh m01 = c01.ToMesh(name + "01");
            Mesh m11 = c11.ToMesh(name + "11");
            return Stitch(m00, m10, m01, m11);
        }
    }
}
