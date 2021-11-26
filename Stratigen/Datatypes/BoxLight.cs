using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Framework;
using Stratigen.Libraries;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Stratigen.Datatypes
{
    public class BoxLight : IVisualObject
    {
        public Box Bounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Color4 LightColor = Color4.White;
        public long ID;
        public Vec3[] Vertices = new Vec3[8]
        {
            Vec3.ZZZ,
            Vec3.ZZP,
            Vec3.ZPZ,
            Vec3.ZPP,
            Vec3.PZZ,
            Vec3.PZP,
            Vec3.PPZ,
            Vec3.PPP,
        };        
        public Vec3[] PositionedVertices = new Vec3[8]
        {
            Vec3.ZZZ,
            Vec3.ZZP,
            Vec3.ZPZ,
            Vec3.ZPP,
            Vec3.PZZ,
            Vec3.PZP,
            Vec3.PPZ,
            Vec3.PPP,
        };
        public byte[] Indices = new byte[36]
        {
            0, 4, 6,
            2, 0, 6,

            1, 0, 2,
            3, 1, 2,

            5, 1, 3,
            0, 5, 3,

            4, 5, 7,
            6, 4, 7,

            1, 5, 4,
            0, 1, 4,

            3, 7, 6,
            2, 3, 6,

        };
        //public PrimitiveType? DrawMode = null;
        public Vec3 Position { get; set; }

        public void SetPosition(Vec3 v)
        {
            Position = v;
            for (int i = 0; i < Vertices.Count(); i++)
            {
                PositionedVertices[i] = Vertices[i] + v;
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vec3(x, y, z);
            for (int i = 0; i < Vertices.Count(); i++)
            {
                PositionedVertices[i] = Vertices[i] + Position;
            }
        }

        public void Scale(float f)
        {
            for (int i = 0; i < Vertices.Count(); i++)
            {
                Vertices[i] *= f;
                PositionedVertices[i] = Vertices[i] + Position;
            }
        }

        public void PreDraw()
        {
            GL.Enable(EnableCap.DepthTest); 
        }

        public void Draw()
        {
            PreDraw();
            if (!Globals.VertexGroups)
            {
                GL.Begin(DrawMode == null ? Globals.DrawMode : DrawMode.Value);
                foreach (byte b in Indices)
                {
                    GL.Vertex3(Vertices[b]);
                    GL.Normal3(Vertices[b].Normalize());
                    //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Color4.White);
                    GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Emission);
                    GL.Color4(LightColor);
                }
                GL.End();
            }
            else
            {
                int vertexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Count() * 3 * sizeof(float)), Vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);
                int indexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<byte>(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count()), Indices, BufferUsageHint.StaticDraw);
                GL.DrawElements(DrawMode == null ? Globals.DrawMode : DrawMode.Value, Indices.Count(), DrawElementsType.UnsignedByte, 0);
                GL.DisableVertexAttribArray(0);
                GL.DeleteBuffer(vertexBuffer);
            }

            PostDraw();
        }

        public void PostDraw()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public void DrawAtPosition()
        {
            PreDraw();
            if (!Globals.VertexGroups)
            {
                GL.Begin(DrawMode == null ? Globals.DrawMode : DrawMode.Value);
                foreach (byte b in Indices)
                {
                    GL.Normal3(Vertices[b].Normalize());
                    //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new Color4(1, 1, 1, 1));
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, LightColor);
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, LightColor);
                    //GL.Color4(LightColor);
                    GL.Vertex3(PositionedVertices[b]);
                }
                GL.End();
            }
            else
            {
                int vertexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(PositionedVertices.Count() * 3 * sizeof(float)), PositionedVertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);
                int indexBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
                GL.BufferData<byte>(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Count()), Indices, BufferUsageHint.StaticDraw);
                GL.DrawElements(DrawMode == null ? Globals.DrawMode : DrawMode.Value, Indices.Count(), DrawElementsType.UnsignedByte, 0);
                GL.DisableVertexAttribArray(0);
                GL.DeleteBuffer(vertexBuffer);
            }

            PostDraw();
        }

        public void DrawPositioned()
        {
            DrawAtPosition();
        }

        public void DrawAt(Vec3 v)
        {
            throw new NotImplementedException();
        }
    }
}
