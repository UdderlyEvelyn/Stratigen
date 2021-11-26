using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Libraries;
using Microsoft.Xna.Framework;
using Stratigen.Framework;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stratigen.Datatypes
{
    public class World : ICollidable3
    {
        public Dictionary<Vec2, Chunk> Chunks = new Dictionary<Vec2, Chunk>();
        public int Width = 0;
        public int Height = 0;
        public double Seed;
        public int ProjectionFactor = 1024;
        public BoundingBox BoundingBox { get; set; }
        public Vec3 Position { get; set; }
        public Dictionary<Vec2, WeakReference<Chunk>> ChunkCache = new Dictionary<Vec2, WeakReference<Chunk>>();
        public List<Chunk> LoadedChunks = new List<Chunk>();

        public World()
        {
            Position = Vec3.Zero;
        }

        public bool InRange(float x, float y, float z)
        {
            if (x < 0) return false;
            if (z < 0) return false;
            if (y < 0) return false;
            if (y > 255) return false;
            if (Math.Floor(x / 16) > Width - 1) return false;
            if (Math.Floor(z / 16) > Height - 1) return false;
            return true;
        }

        public Chunk GetChunk(int x, int z)
        {
            Vec2 wp = new Vec2(x, z);
            Chunk c = null;
            try
            {
                Chunks.TryGetValue(wp, out c);
            }
            catch (OverflowException oe)
            {
                Console.WriteLine(oe.Message);
                //overflow
            }
            return c;
        }

        public List<Block> Surrounding(Vec3 position, float range)
        {
            List<Block> blocks = new List<Block>();
            int r = (int)Math.Ceiling(range);
            for (float z = -r + position.Z; z < r + position.Z; z++)
            {
                for (float y = -r + position.Y; y < r + position.Y; y++)
                {
                    for (float x = -r + position.X; x < r + position.X; x++)
                    {
                        if (y < 0) continue;
                        if (y > 255) continue;
                        if (x < 0) continue;
                        if (z < 0) continue;
                        if (Math.Floor(x / 16) > Width - 1) continue;
                        if (Math.Floor(z / 16) > Height - 1) continue;
                        Block b = GetBlock(new Vec3((float)Math.Floor((double)x), (float)Math.Floor((double)y), (float)Math.Floor((double)z)));
                        if (b != null && b.IsVisible) blocks.Add(b);
                    }
                }
            }
            return blocks;
        }

        public List<Chunk> SurroundingChunks(Vec3 position, int range)
        {
            List<Chunk> chunks = new List<Chunk>();
            int posX = (int)Math.Floor(position.X / 16);
            int posZ = (int)Math.Floor(position.Z / 16);
            int r = range;
            for (int z = -r + posZ; z < r + posZ; z++)
            {
                for (int x = -r + posX; x < r + posX; x++)
                {
                    if (!InRange(x, 1, z)) continue;
                    Chunk c = GetChunk(x, z);
                    if (c != null) chunks.Add(c);
                }
            }
            return chunks;
        }

        public void LoadSurroundingChunks(Vec3 position, int range)
        {
            List<Chunk> surroundingChunks = SurroundingChunks(position, range);
            foreach (Chunk c in surroundingChunks)
            {
                c.World = this;
                c.Load();
            }
        }

        public void PurgeOutOfRangeChunks(Vec3 position, int range)
        {
            List<Chunk> surroundingChunks = SurroundingChunks(position, range);
            foreach (Chunk c in Chunks.Values) if (c != null) c.Unload();
        }

        public void UpdateLoadedChunks(Vec3 position, int range)
        {
            List<Chunk> surroundingChunks = SurroundingChunks(position, range);
            List<Chunk> allChunks = new List<Chunk>();
            lock (Chunks) allChunks = Chunks.Values.ToList();
            foreach (Chunk c in allChunks)
            {
                if (c != null)
                {
                    c.World = this;
                    if (surroundingChunks.Contains(c)) c.Load();
                    else c.Unload();
                }
            }
        }

        //Buggy? Didn't test extensively but buggy in tree gen cases.
        /*public void SetBlock(Vec3 v, BlockType type, bool generateNew = false)
        {
            if (!InRange(v.X, v.Y, v.Z)) return; //ABORT
            Coordinate co = v.ToCoordinate();
            if ((!Chunks.ContainsKey(co.Chunk) || Chunks[co.Chunk] == null) && generateNew) GenerateChunk(co.CX, co.CZ, Seed);
            else return; //ABORT
            Chunk c = Chunks[co.Chunk];
            c.Blocks[co.X, co.Y, co.Z] = new Block(v, type);
            UpdateSelfAndSurrounding(v, generateNew);
        }

        public void UpdateBlock(Vec3 v, bool generateNew = false)
        {
            if (!InRange(v.X, v.Y, v.Z)) return; //ABORT
            Coordinate co = v.ToCoordinate();
            if (!Chunks.ContainsKey(co.Chunk) || Chunks[co.Chunk] == null)
            {
                if (generateNew) GenerateChunk(co.CX, co.CZ, Seed);
                else return;
            }
            Chunk c = Chunks[co.Chunk];
            c.UpdateBlock(co.X, co.Y, co.Z);
        }

        public void UpdateSurrounding(Vec3 v, bool generateNew = false)
        {
            if (!InRange(v.X, v.Y, v.Z)) return; //ABORT
            Coordinate co = v.ToCoordinate();
            if (!Chunks.ContainsKey(co.Chunk) || Chunks[co.Chunk] == null)
            {
                if (generateNew) GenerateChunk(co.CX, co.CZ, Seed);
                else return;
            }
            Chunk c = Chunks[co.Chunk];
            if (co.X == 0) //have to do chunk to the left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, co.Z);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z);
            if (co.X == 15) //have to do chunk to the right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, co.Z);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z);
            if (co.Z == 0) //have to do chunk in front
            {
                Vec2 wp = new Vec2(co.CX, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(co.X, co.Y, 15);
            }
            else c.UpdateBlock(co.X, co.Y, co.Z - 1);
            if (co.Z == 15) //have to do chunk behind
            {
                Vec2 wp = new Vec2(co.CX, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(co.X, co.Y, 0);
            }
            else c.UpdateBlock(co.X, co.Y, co.Z + 1);

            if (co.X == 0 && co.Z == 0) //have to do chunk to the front/left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, 15);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z - 1);
            if (co.X == 15 && co.Z == 15) //have to do chunk to the back/right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, 0);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z + 1);
            if (co.X == 15 && co.Z == 0) //have to do chunk to the front/right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, 15);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z - 1);
            if (co.X == 0 && co.Z == 15) //have to do chunk to the back/left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, 0);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z + 1);
        }

        public void UpdateSelfAndSurrounding(Vec3 v, bool generateNew = false)
        {
            if (!InRange(v.X, v.Y, v.Z)) return; //ABORT
            Coordinate co = v.ToCoordinate();
            if (!Chunks.ContainsKey(co.Chunk) || Chunks[co.Chunk] == null)
            {
                if (generateNew) GenerateChunk(co.CX, co.CZ, Seed);
                else return;
            }
            Chunk c = Chunks[co.Chunk];
            c.UpdateBlock(co.X, co.Y, co.Z);
            if (co.X == 0) //have to do chunk to the left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, co.Z);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z);
            if (co.X == 15) //have to do chunk to the right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, co.Z);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z);
            if (co.Z == 0) //have to do chunk in front
            {
                Vec2 wp = new Vec2(co.CX, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(co.X, co.Y, 15);
            }
            else c.UpdateBlock(co.X, co.Y, co.Z - 1);
            if (co.Z == 15) //have to do chunk behind
            {
                Vec2 wp = new Vec2(co.CX, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(co.X, co.Y, 0);
            }
            else c.UpdateBlock(co.X, co.Y, co.Z + 1);

            if (co.X == 0 && co.Z == 0) //have to do chunk to the front/left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, 15);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z - 1);
            if (co.X == 15 && co.Z == 15) //have to do chunk to the back/right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, 0);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z + 1);
            if (co.X == 15 && co.Z == 0) //have to do chunk to the front/right
            {
                Vec2 wp = new Vec2(co.CX + 1, co.CZ - 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(0, co.Y, 15);
            }
            else c.UpdateBlock(co.X + 1, co.Y, co.Z - 1);
            if (co.X == 0 && co.Z == 15) //have to do chunk to the back/left
            {
                Vec2 wp = new Vec2(co.CX - 1, co.CZ + 1);
                if (!Chunks.ContainsKey(wp) || Chunks[wp] == null)
                {
                    if (generateNew) GenerateChunk(wp.Xi, wp.Yi, Seed);
                    else return;
                }
                Chunks[wp].UpdateBlock(15, co.Y, 0);
            }
            else c.UpdateBlock(co.X - 1, co.Y, co.Z + 1);
            c.UpdateBlock(co.X, co.Y, co.Z);
        }*/

        public Block GetBlock(Vec3 v)
        {
            return GetBlock(v.X, v.Y, v.Z);
        }

        public Block GetBlock(float x, float y, float z)
        {
            if (!InRange(x, y, z)) return null; //if block coordinates aren't in world, no block found
            int cx = (int)Math.Floor(x / 16);
            int cy = (int)Math.Floor(z / 16);
            if (!InRange(cx, 1, cy)) return null; //if chunk coordinates aren't valid, no block found
            Chunk c = GetChunk(cx, cy);
            if (c == null) return null; //if chunk is invalid, no block found
            int bx = (int)Math.Floor(x % 16);
            int by = (int)Math.Floor(y);
            int bz = (int)Math.Floor(z % 16);
            return c.InRange(bx, by, bz) ? c.Blocks[bx, by, bz] : null; //if block coordinates aren't in chunk, no block found
        }

        public Block GetBlock(int cx, int cy, int lx, int y, int lz)
        {
            if (lx < 0 || lz < 0 || lx > 15 || lz > 15 || y < 0 || y > 255) return null; //if local coordinates are invalid, no block found
            if (!InRange(cx, 1, cy)) return null; //if chunk cooridnates aren't valid, no block found
            Chunk c = GetChunk(cx, cy);
            if (c == null) return null; //if chunk is invalid, no block found
            return c.Blocks[lx, y, lz];
        }

        public bool BlockAt(Vec3 v)
        {
            if (!InRange(v.X, v.Y, v.Z)) return false;
            else return GetBlock(v.X, v.Y, v.Z) != null;
        }

        public bool BlockAt(float x, float y, float z)
        {
            if (!InRange(x, y, z)) return false;
            else return GetBlock(x, y, z) != null;
        }

        public void AddOrUpdate(Vec2 v, Chunk c)
        {
            lock(Chunks)
            {
                if (Chunks.ContainsKey(v)) Chunks[v] = c;
                else Chunks.Add(v, c);
                if (v.X > Width) Width = (int)Math.Ceiling(v.X);
                if (v.Y > Height) Height = (int)Math.Ceiling(v.Y);
            }
        }

        public void Remove(Vec2 v)
        {
            lock(Chunks) if (Chunks.ContainsKey(v)) Chunks.Remove(v);
        }

        public Chunk this[int x, int z]
        {
            get
            {
                return GetChunk(x, z);
            }
            set
            {
                this.Chunks[new Vec2(x, z)] = value;
            }
        }

        public Chunk this[float x, float z]
        {
            get
            {
                return GetChunk((int)Math.Floor(x), (int)Math.Floor(z));
            }
            set
            {
                this.Chunks[new Vec2((int)Math.Floor(x), (int)Math.Floor(z))] = value;
            }
        }

        public Chunk this[Vec2 v]
        {
            get
            {
                return GetChunk((int)v.X, (int)v.Y);
            }
            set
            {
                AddOrUpdate(v, value);
            }
        }

        public Chunk this[Vec3 v]
        {
            get
            {
                return GetChunk((int)Math.Floor(v.X / 16), (int)Math.Floor(v.Z / 16));
            }
            set
            {
                this.Chunks[new Vec2((int)Math.Floor(v.X / 16), (int)Math.Floor(v.Z / 16))] = value;
            }
        }

        public void GenerateChunk(int x, int z, double seed)
        {
            Chunk ch = null;
            Vec2 wp = new Vec2(x, z);
            bool reloaded = false;
            WeakReference<Chunk> wrc;
            if (ChunkCache.TryGetValue(wp, out wrc) && wrc != null) reloaded = wrc.TryGetTarget(out ch);
            if (!reloaded)
            {
                /*string file = x + "-" + z + ".ykch";
                if (File.Exists(file))
                {
                    ch = new Chunk() { Loading = true, Dirty = false, Blocks = new Block[16, 256, 16] };
                    using (GZipStream stream = new GZipStream(File.OpenRead(file), CompressionMode.Decompress))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        for (int cz = 0; cz < 16; cz++) //iterate Z
                            for (int cx = 0; cx < 16; cx++) //iterate X
                            {
                                for (int cy = 255; cy > 0; cy--) //iterate Y (top to bottom)
                                {
                                    Block b = (Block)bf.Deserialize(stream);
                                    if (b != Block.NullBlock) ch.Blocks[cx, cy, cz] = b;
                                }
                            }
                    }
                    Console.WriteLine("Chunk loaded from \"" + file + "\"..");
                }
                else*/ ch = new Cell(seed, 16, 16, x, z, ProjectionFactor, ProjectionFactor).Simplex().ToChunk(this);
                string file = x + "-" + z + ".ykch";
                if (File.Exists(file))
                {
                    using (TextReader tr = File.OpenText(file))
                    {
                        string[] lines = tr.ReadToEnd().Split('\n');
                        foreach (string line in lines)
                        {
                            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
                            Change c = new Change(line);
                            ch.Changes.Add(c.Position, c);
                            Block b = null;
                            try
                            {
                                b = ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi];
                            }
                            catch { /*Pass*/ }
                            if (b == null && c.BlockTypeID != 0)
                                ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi] = new Block(c.Position + Vec3.HZH, BlockType.Get(c.BlockTypeID));
                            else if (c.BlockTypeID != 0) ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi].Type = BlockType.Get(c.BlockTypeID);
                        }
                    }
                    //using (GZipStream stream = new GZipStream(File.OpenRead(file), CompressionMode.Decompress))
                    //{
                        //BinaryFormatter bf = new BinaryFormatter();
                        //bool more = true;
                        /*while (more)
                        {
                            try
                            {
                                Change c = (Change)bf.Deserialize(stream);
                                ch.Changes.Add(c.Position, c);
                                Block b = null;
                                try
                                {
                                    b = ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi];
                                }
                                catch { /*Pass*/ /*}
                                if (b == null)
                                    ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi] = new Block(c.Position, BlockType.Get(c.BlockTypeID));
                                else ch.Blocks[c.Position.Xi, c.Position.Yi, c.Position.Zi].Type = BlockType.Get(c.BlockTypeID);
                            }
                            catch
                            {
                                more = false;
                                break;
                            }
                        }
                    }*/
                }
            }
            ch.World = this;
            AddOrUpdate(ch.WorldPosition, ch);
            ch.Loading = false;
            ch.Dirty = true;
            ch.WorldPosition = wp;
            ch.Position = new Vec3(wp.X, 0, wp.Y);
            ch.Update();
        }

        public void Update()
        {
            /*Parallel.ForEach<Chunk>(Chunks.Values,
                delegate(Chunk c)
                {*/
                    foreach (Chunk c in Chunks.Values) c.Update();
                //}
            //);
        }
    }
}

/*
World Save Format
 World (Folder)
 * World.dat (File)
 * * Seed (double)
 * * Name (string)
 * SectorX.Y.dat (File, where X and Y are sector world position of 8x8 chunks of chunks)
 * * ChunkPosition (Vec2, world position)
 * * Change
 * * More Changes, etc.
 * * Magic Value marking the end of chunk
 * * More Chunks
 * Players (Folder)
 * * PlayerName.dat (where PlayerName is the player's name)
 * * * Position (Vec3, absolute)
 * * * Inventory (probably not in its class format, something specially packed up)
 * * * Stats (depends on how we implement these, probably its own serializable class to represent player status - position might get in there too) 
*/