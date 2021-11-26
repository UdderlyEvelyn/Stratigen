using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Runtime.InteropServices;

namespace Stratigen.Libraries
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// The BGR to RGB color matrix used to switch the blue and red colors from an image
        /// </summary>
        private static ColorMatrix _BgrToRgbColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        /// <summary>
        /// My SaveAsPng function.
        /// </summary>
        /// <param name="stream">The stream to write the png to.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void MySaveAsPng(this Texture2D thisTexture2D, Stream stream, int width, int height)
        {
            var pixelData = new byte[thisTexture2D.Width * thisTexture2D.Height /** GraphicsExtensions.Size(thisTexture2D.Format)*/];
            thisTexture2D.GetData(pixelData);
            Bitmap bitmap = new Bitmap(thisTexture2D.Width, thisTexture2D.Height, PixelFormat.Format32bppArgb);
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, thisTexture2D.Width, thisTexture2D.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Marshal.Copy(pixelData, 0, bmData.Scan0, 4 * thisTexture2D.Width * thisTexture2D.Height);
            bitmap.UnlockBits(bmData);

            // Switch from BGR encoding to RGB
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrix(_BgrToRgbColorMatrix);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, ia);
                }
            }

            bitmap.Save(stream, ImageFormat.Png);
        }

    }
}
