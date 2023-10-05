
using System.Drawing;
using System.Drawing.Imaging;
using Infinite_module_test;
using static Infinite_module_test.tag_structs;

namespace InfiniteTextureCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello, World!");
        }
        public class tagfiles{
            public byte[] bytes;
            public List<KeyValuePair<byte[], bool>> resource_list;
        }
        static public tagfiles get_tagbytes_with_resources(string tag_path){
            tagfiles result = new();

            if (!File.Exists(tag_path)) throw new Exception("file does not exist");

            using (FileStream fs = new FileStream(tag_path, FileMode.Open)){
                byte[] bytes = new byte[4];
                fs.Read(bytes, 0, 4);
                if (!bytes.SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 }))// fail via file not a tag file
                    throw new Exception("not a tag file");
            }
            // check to see if the name is a resource file name
            if (tag_path.Contains("_res_")) throw new Exception("tag file appears to be resource");
            
            // lets first get a list of all the resource file that this guy probably owns
            var folder = Path.GetDirectoryName(tag_path);
            string tag_file_name = Path.GetFileName(tag_path);
            result.resource_list = new();

            foreach (var item in Directory.GetFiles(folder)){
                string file_name = Path.GetFileName(item);
                if (file_name.Length > tag_file_name.Length + 4 && file_name.StartsWith(tag_file_name)){
                    // get index of file, just incase the function that retrives all the files doesn't do it alphabetically
                    // then either insert or add 
                    try{string resource_number = file_name.Substring(tag_file_name.Length + 5);
                        int resource_index = Convert.ToInt32(resource_number);
                        byte[] resource_bytes = File.ReadAllBytes(item);  
                        // test whether the first 4 bytes are the 'hscu' magic (or whatever its supposed to be)
                        bool is_standalone_resource = resource_bytes[0..4].SequenceEqual(new byte[] { 0x75, 0x63, 0x73, 0x68 });
                        // pop it at the end if the currently index is too high (this is probably a terrible idea)
                        if (resource_index >= result.resource_list.Count) result.resource_list.Add(new KeyValuePair<byte[], bool>(resource_bytes, is_standalone_resource));
                        else result.resource_list.Insert(resource_index, new KeyValuePair<byte[], bool>(resource_bytes, is_standalone_resource));
                    }catch {} // skip bad resources
                }
            }
            // anomaly check // make sure all entries are of either type, else this will become very difficult to manage
            if (result.resource_list.Count > 0){
                bool inital = result.resource_list[0].Value;
                foreach (var item in result.resource_list)
                    if (item.Value != inital) throw new Exception("mismatching resource types");
            }
            
            result.bytes = File.ReadAllBytes(tag_path);
            return result;
        }
        static tag load_tag(string file){
            tagfiles files = get_tagbytes_with_resources(file);
            tag test = new tag(files.resource_list);
            if (!test.Load_tag_file(files.bytes)) throw new Exception("failed to load tag");
            return test;
        }

        static void convert_image()
        {
            System.Drawing.Image image1 = System.Drawing.Image.FromFile(file);
            var v = image1.PixelFormat;


        }
    }
}