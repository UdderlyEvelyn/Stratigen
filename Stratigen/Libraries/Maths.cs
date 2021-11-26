using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratigen.Datatypes;

namespace Stratigen.Libraries
{
    public static class Maths
    {
        public static float Saturate(float f)
        {
            if (f > 1) return 1;
            if (f < 0) return 0;
            return f;
        }

        #region Clamp

        public static double Clamp(double d, double min, double max)
        {
            return Math.Max(Math.Min(d, max), min);
        }

        public static float Clamp(float f, float min, float max)
        {
            return Math.Max(Math.Min(f, max), min);
        }

        public static int Clamp(int i, int min, int max)
        {
            return Math.Max(Math.Min(i, max), min);
        }

        public static byte Clamp(byte b, byte min, byte max)
        {
            return Math.Max(Math.Min(b, max), min);
        }

        public static Vec2 Clamp(Vec2 v, float min, float max)
        {
            return new Vec2(Clamp(v.X, min, max), Clamp(v.Y, min, max));
        }

        public static Vec3 Clamp(Vec3 v, float min, float max)
        {
            return new Vec3(Clamp(v.X, min, max), Clamp(v.Y, min, max), Clamp(v.Z, min, max));
        }

        #endregion

        #region LinearAddress

        public static double LinearAddress(double x, double y, int width)
        {
            return Math.Max((int)y * width + (int)x, 0);
        }

        public static float LinearAddress(float x, float y, int width)
        {
            return Math.Max((int)y * width + (int)x, 0);
        }

        public static int LinearAddress(int x, int y, int width)
        {
            return Math.Max(y * width + x, 0);
        }

        public static byte LinearAddress(byte x, byte y, int width)
        {
            return (byte)Math.Max(y * width + x, 0);
        }

        public static float LinearAddress(Vec2 v, int width)
        {
            return Math.Max(v.Y * width + v.X, 0);
        }

        #endregion

        #region EuclideanAddress

        public static Vec2 EuclideanAddress(int linearAddress, int width)
        {
            int x = linearAddress % width;
            int y = (linearAddress - x) / width;
            return new Vec2(x, y);
        }

        #endregion

        public static double DegToRad(this double angle)
        {
            return angle * (Math.PI / 180);
        }

        public static double RadToDeg(this double angle)
        {
            return angle * (180 / Math.PI);
        }

        #region Inside

        public static bool Inside(this float f, float min, float max)
        {
            return f > min && f < max;
        }

        public static bool Inside(this double d, double min, double max)
        {
            return d > min && d < max;
        }

        public static bool Inside(this int i, int min, int max)
        {
            return i > min && i < max;
        }

        public static bool Inside(this byte b, byte min, byte max)
        {
            return b > min && b < max;
        }

        public static bool Inside(this Vec2 v, float min, float max)
        {
            return v.X > min && v.X < max && v.Y > min && v.Y < max;
        }

        public static bool Inside(this Vec3 v, float min, float max)
        {
            return v.X > min && v.X < max && v.Y > min && v.Y < max && v.Z > min && v.Z < max;
        }

        public static bool Inside(this Vec2 v, Vec2 min, Vec2 max)
        {
            return v.X > min.X && v.X < max.X && v.Y > min.Y && v.Y < max.Y;
        }

        public static bool Inside(this Vec3 v, Vec3 min, Vec3 max)
        {
            return v.X > min.X && v.X < max.X && v.Y > min.Y && v.Y < max.Y && v.Z > min.Z && v.Z < max.Z;
        }

        #endregion

        #region Outside

        public static bool Outside(this float f, float min, float max)
        {
            return !f.Inside(min, max);
        }

        public static bool Outside(this double d, double min, double max)
        {
            return !d.Inside(min, max);
        }

        public static bool Outside(this int i, int min, int max)
        {
            return !i.Inside(min, max);
        }

        public static bool Outside(this byte b, byte min, byte max)
        {
            return !b.Inside(min, max);
        }

        public static bool Outside(this Vec2 v, float min, float max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec3 v, float min, float max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec2 v, Vec2 min, Vec2 max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec3 v, Vec3 min, Vec3 max)
        {
            return !v.Inside(min, max);
        }

        #endregion

        #region Circle Math

        public static float CirclePosition(float radius, float angle)
        {
            return (float)Math.Sin(angle) * radius;
        }

        public static float CircleRadialAngle(float radius, float x)
        {
            return (float)Math.Asin(x / radius);
        }

        #endregion
    }
}
