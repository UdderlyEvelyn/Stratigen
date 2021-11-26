using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShaderAbstractionLayer
{
    class Program
    {
        /// <summary>
        /// This takes in an HLSL source file, accounts for the differences between shader models,
        /// and saves a copy of the source modified for different shader model levels (SM2 for GL and SM4 for DX11). 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string path = args[0];
            DateTime start = DateTime.Now;
            string sm2path = Path.GetDirectoryName(path) + @"\SM2" + Path.GetFileName(path);
            string sm4path = Path.GetDirectoryName(path) + @"\SM4" + Path.GetFileName(path);
            Console.WriteLine("Reading HLSL Source...");
            TextReader tr = File.OpenText(path);
            string source = tr.ReadToEnd();
            tr.Close();
            Console.WriteLine("Translating to SM2...");
            string sm2source = ToSM2(source);
            Console.WriteLine("Translating to SM4...");
            string sm4source = ToSM4(source);
            Console.WriteLine("Writing SM2 Source File to \"" + sm2path + "\".");
            if (File.Exists(sm2path)) File.Delete(sm2path);
            TextWriter tw = File.CreateText(sm2path);
            tw.Write(sm2source);
            tw.Flush();
            tw.Close();
            Console.WriteLine("Writing SM4 Source File to \"" + sm4path + "\".");
            if (File.Exists(sm4path)) File.Delete(sm4path);
            tw = File.CreateText(sm4path);
            tw.Write(sm4source);
            tw.Flush();
            tw.Close();
            Console.WriteLine("Done (" + Math.Round((DateTime.Now - start).TotalMilliseconds, 2) + "ms).");
        }

        static string ToSM2(string source)
        {
	        //float4 InternalPosition		: POSITION;
            //tex2D(TexSampler, PSIn.TexCoord);
            //VertexShader = compile vs_2_0 vShader();
            //PixelShader  = compile ps_2_0 pShader();
            return source
                .Replace("vs_4_0_level_9_1", "vs_2_0")
                .Replace("ps_4_0_level_9_1", "ps_2_0")
                .Replace("Texture.Sample", "tex2D")
                .Replace("SV_POSITION", "POSITION");
        }

        static string ToSM4(string source)
        {
            //float4 InternalPosition		: SV_POSITION;
            //float4 texColor = Texture.Sample(TexSampler, PSIn.TexCoord);
    	    //VertexShader = compile vs_4_0_level_9_1 vShader();
            //PixelShader  = compile ps_4_0_level_9_1 pShader();
            return source
                .Replace("vs_2_0", "vs_4_0_level_9_1")
                .Replace("ps_2_0", "ps_4_0_level_9_1")
                .Replace("tex2D", "Texture.Sample")
                .Replace(": POSITION;", ": SV_POSITION;")
                .Replace(":POSITION;", ":SV_POSITION;");
        }
    }
}
