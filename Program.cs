
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Infinite_module_test;
using static Infinite_module_test.tag_structs;

namespace InfiniteTextureCompiler
{
    internal class Program
    {
        static void Main(string[] args) { 
            Console.WriteLine("Input source image path");
            string? input_path = Console.ReadLine();
            if (string.IsNullOrEmpty(input_path)) throw new Exception("bad input");

            Console.WriteLine("Input destination bitm path");
            string? output_path = Console.ReadLine();
            if (string.IsNullOrEmpty(output_path)) throw new Exception("bad input");

            Console.WriteLine("Input desired TagID (unsigned integer)");
            string? tagID = Console.ReadLine();
            if (string.IsNullOrEmpty(tagID)) throw new Exception("bad input");
            uint new_tagid = Convert.ToUInt32(tagID);

            Console.WriteLine("beginning conversion process...");
            convert_image(input_path, output_path, new_tagid);
            Console.WriteLine("process complete.");
        }
        static tag load_tag(string file){
            if (!File.Exists(file)) throw new Exception("file does not exist");
            byte[] file_bytes = File.ReadAllBytes(file);

            tag test = new tag(new List <KeyValuePair<byte[], bool>> ()); // apparently we do NOT support null, despite declaring it as nullable (this is for compiling at least)
            if (!test.Load_tag_file(file_bytes)) throw new Exception("failed to load tag");
            return test;
        }

        static void convert_image(string image_path, string output_path, uint tagid){
            //System.Drawing.Image image1 = System.Drawing.Image.FromFile(image_path);
            //var v = image1.PixelFormat;
            //var pixels = image1.;

            Bitmap bmp = new Bitmap(image_path);
            //Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData rawOriginal = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            int origByteCount = rawOriginal.Stride * rawOriginal.Height;
            byte[] origBytes = new byte[origByteCount];
            System.Runtime.InteropServices.Marshal.Copy(rawOriginal.Scan0, origBytes, 0, origByteCount);

            // we now copy over this information to the loaded tag
            tag bitmap_tag = load_tag("template\\template.bitm");
            bitmap_tag.set_tagID(tagid); // we have to update the tagID, as we're going to have the template set to -1
            bitmap_tag.set_number("bitmaps[0].width", bmp.Width.ToString());
            bitmap_tag.set_number("bitmaps[0].sourceWidth", bmp.Width.ToString());
            bitmap_tag.set_number("bitmaps[0].height", bmp.Height.ToString());
            bitmap_tag.set_number("bitmaps[0].sourceHeight", bmp.Height.ToString());
            bitmap_tag.set_datablock("bitmaps[0].bitmap resource handle.pixels", origBytes);
            tag.compiled_tag output = bitmap_tag.compile();
            // we aren't going to output resources with this, so we only need to worry about spitting out the main file
            File.WriteAllBytes(output_path, output.tag_bytes);
        }
    }
}