
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Drawing;
using System.Drawing.Imaging;


namespace InfiniteTextureCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello, World!");
        }



        static void convert_image(string file)
        {
            System.Drawing.Image image1 = System.Drawing.Image.FromFile(file);
            var v = image1.PixelFormat;


        }
    }
}