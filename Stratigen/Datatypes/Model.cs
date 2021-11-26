using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stratigen.Datatypes
{
    public class Model
    {
        private static long _id;

        public long ID { get; set; }
        public string Name { get; set; }
        public LitTextureVertex[] Vertices { get; set; }
        public Index[] Indices { get; set; }
        //public VertexPositionColor[] Vertices { get; set; }
        //public int[] Indices { get; set; }
        public List<Instance> Instances { get; set; }
        public string TexturePath { get; set; }

        public Model(LitTextureVertex[] vertices, Index[] indices, string name = "")
        //public Model(VertexPositionColor[] vertices, int[] indices)
        {
            ID = Interlocked.Increment(ref _id);
            Name = name;
            Vertices = vertices;
            Indices = indices;
            Instances = new List<Instance>();
        }

        public Instance Instantiate(Vec3 position, float size = 1f, float yaw = 0, float pitch = 0, float roll = 0)
        {
            Instance i = new Instance(this, position, size, yaw, pitch, roll);
            Instances.Add(i);
            return i;
        }

        /*public void Update()
        {
            foreach (Instance i in Instances)
            {
                //i.Update();
            }
        }*/
    }
}
