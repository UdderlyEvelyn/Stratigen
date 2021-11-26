using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX;
/*using Jitter;
using Jitter.Collision;
using Jitter.LinearMath;
using Jitter.Dynamics;*/
using Stratigen.Libraries;
/*using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities.Prefabs;*/
using System.IO;

namespace Stratigen.Datatypes
{
    public class Mesh
    {
        public string Name;
        public Buffer<VertexPositionColor> VertexBuffer;
        public Buffer<int> IndexBuffer;
        public VertexInputLayout VertexInputLayout;
        public GraphicsDevice GraphicsDevice;
        public List<BoundingBox> CollisionBoxes;
        public Vector3 Position;
        public object Tag;

        public void Draw()
        {
            Draw(GraphicsDevice);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);
            graphicsDevice.Draw(PrimitiveType.TriangleList, VertexBuffer.ElementCount);
        }

        public void DrawOrderless()
        {
            DrawOrderless(GraphicsDevice);
        }

        public void DrawOrderless(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);
            graphicsDevice.Draw(PrimitiveType.TriangleStrip, VertexBuffer.ElementCount);
        }

        public void DrawRay()
        {
            DrawRay(GraphicsDevice);
        }

        public void DrawRay(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);
            graphicsDevice.Draw(PrimitiveType.LineList, VertexBuffer.ElementCount);
        }

        public void DrawIndexed()
        {
            DrawIndexed(GraphicsDevice);
        }

        public void DrawIndexed(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetIndexBuffer(IndexBuffer, true);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);
            graphicsDevice.DrawIndexed(PrimitiveType.TriangleList, IndexBuffer.ElementCount);
        }

        public void Draw(Buffer<VertexPositionColor> vertexBuffer)
        {
            Draw(GraphicsDevice, vertexBuffer, null);
        }

        public void Draw(Buffer<VertexPositionColor> vertexBuffer, Buffer<int> indexBuffer)
        {
            Draw(GraphicsDevice, vertexBuffer, indexBuffer);
        }

        public void Draw(GraphicsDevice graphicsDevice, Buffer<VertexPositionColor> vertexBuffer, Buffer<int> indexBuffer)
        {
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            if (indexBuffer != null)
            {
                graphicsDevice.SetIndexBuffer(indexBuffer, true);
                graphicsDevice.DrawIndexed(PrimitiveType.TriangleList, indexBuffer.ElementCount);
            }
            else graphicsDevice.Draw(PrimitiveType.TriangleStrip, VertexBuffer.ElementCount);
        }

        public void Scale(float f)
        {
            VertexPositionColor[] vpcs = VertexBuffer.GetData();
            VertexPositionColor[] nvpcs = new VertexPositionColor[VertexBuffer.ElementCount];
            for (int i = 0; i < VertexBuffer.ElementCount; i++)
            {
                nvpcs[i] = new VertexPositionColor(vpcs[i].Position * f, vpcs[i].Color);
            }
            if (IndexBuffer != null && IndexBuffer.ElementCount > 0)
            {
                int[] indices = IndexBuffer.GetData();
                int[] nindices = new int[IndexBuffer.ElementCount];
                for (int i = 0; i < IndexBuffer.ElementCount; i++)
                {
                    nindices[i] = (int)(indices[i] * f);
                }
                IndexBuffer.SetData(nindices);
            }
            VertexBuffer.SetData(nvpcs);
        }

        public void DrawAt(Vector3 v)
        {
            Matrix m = Matrix.Translation(v);
            VertexPositionColor[] vpcs = VertexBuffer.GetData();
            VertexPositionColor[] nvpcs = new VertexPositionColor[VertexBuffer.ElementCount];
            Buffer<int> indexbuffer = Buffer<int>.New<int>(GraphicsDevice, IndexBuffer == null ? 1 : IndexBuffer.ElementCount, BufferFlags.IndexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            for (int i = 0; i < VertexBuffer.ElementCount; i++)
            {
                if (Tag != null)
                {
                    if (Tag.GetType() == typeof(Color))
                        nvpcs[i] = new VertexPositionColor(Vector3.Transform(vpcs[i].Position, m).ToVector3(), (Color)Tag);
                    else
                        nvpcs[i] = new VertexPositionColor(Vector3.Transform(vpcs[i].Position, m).ToVector3(), vpcs[i].Color);
                }
                else
                    nvpcs[i] = new VertexPositionColor(Vector3.Transform(vpcs[i].Position, m).ToVector3(), vpcs[i].Color);
            }

            if (indexbuffer.ElementCount > 1)
            {
                if (IndexBuffer != null && IndexBuffer.ElementCount > 0)
                {
                    int[] indices = IndexBuffer.GetData();
                    int[] nindices = new int[IndexBuffer.ElementCount];
                    for (int i = 0; i + 3 <= IndexBuffer.ElementCount; i += 3)
                    {
                        nindices[i] = (int)(indices[i] * m.TranslationVector.X);
                        nindices[i + 1] = (int)(indices[i + 1] * m.TranslationVector.Y);
                        nindices[i + 2] = (int)(indices[i + 2] * m.TranslationVector.Z);
                    }
                    indexbuffer.SetData(nindices);
                }
            }
            Buffer<VertexPositionColor> vertexbuffer = Buffer<VertexPositionColor>.New<VertexPositionColor>(GraphicsDevice, nvpcs.Count(), BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            vertexbuffer.SetData(nvpcs);

            if (indexbuffer.ElementCount > 1)
                Draw(vertexbuffer, indexbuffer);
            else Draw(vertexbuffer);
        }

        public void DrawTransformed(Vector3 v)
        {
            VertexPositionColor[] vpcs = VertexBuffer.GetData();
            VertexPositionColor[] nvpcs = new VertexPositionColor[VertexBuffer.ElementCount];
            Buffer<int> indexbuffer = Buffer<int>.New<int>(GraphicsDevice, IndexBuffer == null ? 1 : IndexBuffer.ElementCount, BufferFlags.IndexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            for (int i = 0; i < VertexBuffer.ElementCount; i++)
            {
                if (Tag != null)
                {
                    if (Tag.GetType() == typeof(Color))
                        nvpcs[i] = new VertexPositionColor(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), (Color)Tag);
                    else
                        nvpcs[i] = new VertexPositionColor(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), vpcs[i].Color);
                }
                else
                    nvpcs[i] = new VertexPositionColor(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), vpcs[i].Color);
            }

            if (indexbuffer.ElementCount > 1)
            {
                if (IndexBuffer != null && IndexBuffer.ElementCount > 0)
                {
                    int[] indices = IndexBuffer.GetData();
                    int[] nindices = new int[IndexBuffer.ElementCount];
                    for (int i = 0; i + 3 <= IndexBuffer.ElementCount; i += 3)
                    {
                        nindices[i] = (int)(indices[i] + v.X);
                        nindices[i + 1] = (int)(indices[i + 1] + v.Y);
                        nindices[i + 2] = (int)(indices[i + 2] + v.Z);
                    }
                    indexbuffer.SetData(nindices);
                }
            }
            Buffer<VertexPositionColor> vertexbuffer = Buffer<VertexPositionColor>.New<VertexPositionColor>(GraphicsDevice, nvpcs.Count(), BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            vertexbuffer.SetData(nvpcs);

            if (indexbuffer.ElementCount > 1)
                Draw(vertexbuffer, indexbuffer);
            else Draw(vertexbuffer);
        }

        public void DrawTransformedWithMatrix(Vector3 v, Matrix m)
        {
            VertexPositionColor[] vpcs = VertexBuffer.GetData();
            VertexPositionColor[] nvpcs = new VertexPositionColor[VertexBuffer.ElementCount];
            Buffer<int> indexbuffer = Buffer<int>.New<int>(GraphicsDevice, IndexBuffer == null ? 1 : IndexBuffer.ElementCount, BufferFlags.IndexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            for (int i = 0; i < VertexBuffer.ElementCount; i++)
            {
                if (Tag != null)
                {
                    if (Tag.GetType() == typeof(Color))
                        nvpcs[i] = new VertexPositionColor(Vector3.Transform(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), m).ToVector3(), (Color)Tag);
                    else
                        nvpcs[i] = new VertexPositionColor(Vector3.Transform(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), m).ToVector3(), vpcs[i].Color);
                }
                else
                    nvpcs[i] = new VertexPositionColor(Vector3.Transform(new Vector3(vpcs[i].Position.X + v.X, vpcs[i].Position.Y + v.Y, vpcs[i].Position.Z + v.Z), m).ToVector3(), vpcs[i].Color);
            }

            if (indexbuffer.ElementCount > 1)
            {
                if (IndexBuffer != null && IndexBuffer.ElementCount > 0)
                {
                    int[] indices = IndexBuffer.GetData();
                    int[] nindices = new int[IndexBuffer.ElementCount];
                    for (int i = 0; i + 3 <= IndexBuffer.ElementCount; i += 3)
                    {
                        nindices[i] = (int)(indices[i] + v.X);
                        nindices[i + 1] = (int)(indices[i + 1] + v.Y);
                        nindices[i + 2] = (int)(indices[i + 2] + v.Z);
                    }
                    indexbuffer.SetData(nindices);
                }
            }
            Buffer<VertexPositionColor> vertexbuffer = Buffer<VertexPositionColor>.New<VertexPositionColor>(GraphicsDevice, nvpcs.Count(), BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            vertexbuffer.SetData(nvpcs);

            if (indexbuffer.ElementCount > 1)
                Draw(vertexbuffer, indexbuffer);
            else Draw(vertexbuffer);
        }

        public void AssignMeshData(VertexPositionColor[] vertices, int[] indices)//, BEPUutilities.Quaternion orientation)
        {
            SetVertexBuffer(vertices);
            SetIndexBuffer(indices);
            //PhysicsObject = new MobileMesh(vertices.Select(v => new BEPUutilities.Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray(), indices, new BEPUutilities.AffineTransform(new BEPUutilities.Vector3(Position.X, Position.Y, Position.Z)), MobileMeshSolidity.DoubleSided);
        }

        public void SetVertexBuffer(VertexPositionColor[] vertices)
        {
            if (VertexBuffer == null)
                VertexBuffer = Buffer<VertexPositionColor>.New<VertexPositionColor>(GraphicsDevice, vertices.Count(), BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            VertexBuffer.SetData(vertices);
        }

        public void SetIndexBuffer(int[] indices)
        {
            if (IndexBuffer == null)
                IndexBuffer = Buffer<int>.New<int>(GraphicsDevice, indices.Count(), BufferFlags.IndexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            IndexBuffer.SetData(indices);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            /*if (PhysicsObject != null)
                (PhysicsObject as MobileMesh).Position = new BEPUutilities.Vector3(position.X, position.Y, position.Z);*/
        }

        public static Mesh FromVTX(string filename)
        {
            Mesh m = new Mesh() { GraphicsDevice = Globals.GraphicsDevice, Position = Vector3.Zero };
            List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new VertexPositionColor(new Vector3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])), new Color(new Vector3(float.Parse(fields[3]), float.Parse(fields[4]), float.Parse(fields[5])))));
            }
            VertexPositionColor[] vertices = vertexList.ToArray();
            m.VertexBuffer = Buffer<VertexPositionColor>.New<VertexPositionColor>(Globals.GraphicsDevice, vertices.Count(), BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            m.VertexBuffer.SetData(vertices);
            m.VertexInputLayout = VertexInputLayout.FromBuffer(0, m.VertexBuffer);
            return m;
        }

        public void LoadVTX(string filename)
        {
            List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new VertexPositionColor(new Vector3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])), new Color(new Vector3(float.Parse(fields[3]), float.Parse(fields[4]), float.Parse(fields[5])))));
            }
            VertexPositionColor[] vertices = vertexList.ToArray();
            VertexBuffer.SetData(vertices);
            VertexInputLayout = VertexInputLayout.FromBuffer(0, VertexBuffer);
        }

        public static VertexPositionColor[] VerticesFromVTX(string filename)
        {
            List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
            TextReader tr = File.OpenText(filename);
            for (string s = tr.ReadLine(); s != "" && s != null; s = tr.ReadLine())
            {
                string[] fields = s.Split(',');
                vertexList.Add(new VertexPositionColor(new Vector3(float.Parse(fields[0]), float.Parse(fields[1]), float.Parse(fields[2])), new Color(new Vector3(float.Parse(fields[3]), float.Parse(fields[4]), float.Parse(fields[5])))));
            }
            return vertexList.ToArray();
        }
    }
}
