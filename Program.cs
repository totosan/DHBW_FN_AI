using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.MetaData;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace Predict_console
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", true, true)
             .AddJsonFile("appsettings.development.json", true, true)
             .Build();

            var file = "WIN_20181201_19_01_35_Pro.jpg";
            var url = config["prediction-url"];
            if (args != null && args.Length == 2)
            {
                file = args[0];
                url = args[1];
            }
            var bytes = GetResizedImage(file);
            var response = UploadFileAsync(url, bytes, config["predict-key"]).GetAwaiter().GetResult();
            //var responseJson = (JObject)JsonConvert.DeserializeObject(response);
            Console.Write(response);
        }



        private static byte[] GetFileByteArray(string filename)
        {
            FileStream oFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file size.
            byte[] FileByteArrayData = new byte[oFileStream.Length];

            //Read file in bytes from stream into the byte array
            oFileStream.Read(FileByteArrayData, 0, System.Convert.ToInt32(oFileStream.Length));

            //Close the File Stream
            oFileStream.Close();

            return FileByteArrayData; //return the byte data
        }

        public static async Task<string> UploadFileAsync(string URL, byte[] fileData, string apiKey)
        {
            string Response = null;
            HttpWebRequest WebReq = null;
            HttpWebResponse WebRes = null;
            StreamReader StreamResponseReader = null;
            Stream requestStream = null;
            try
            {
                WebReq = (HttpWebRequest)WebRequest.Create(URL);
                WebReq.Method = "POST";
                WebReq.Accept = "*/*";
                //WebReq.Timeout = 50000;
                WebReq.KeepAlive = false;
                WebReq.AllowAutoRedirect = false;
                WebReq.AllowWriteStreamBuffering = true;
                WebReq.ContentType = "binary/octet-stream";
                WebReq.ContentLength = fileData.Length;
                WebReq.Headers.Add("Prediction-Key", apiKey);

                requestStream = WebReq.GetRequestStream();
                await requestStream.WriteAsync(fileData, 0, fileData.Length);

                requestStream.Close();

                WebRes = (HttpWebResponse)await WebReq.GetResponseAsync();
                StreamResponseReader = new StreamReader(WebRes.GetResponseStream(), Encoding.UTF8);
                Response = StreamResponseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (WebReq != null)
                {
                    WebReq.Abort();
                    WebReq = null;
                }
                if (WebRes != null)
                {
                    WebRes.Close();
                    WebRes = null;
                }
                if (StreamResponseReader != null)
                {
                    StreamResponseReader.Close();
                    StreamResponseReader = null;
                }
                if (requestStream != null)
                {
                    requestStream = null;
                }
            }

            return Response;
        }

        private static byte[] GetResizedImage(string inputPath)
        {
            using (Image<Rgba32> image = Image.Load(inputPath))
            {
                image.Mutate(x => x
                     .Resize(image.Width / 2, image.Height / 2)
                     .Grayscale());
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormats.Jpeg);
                    return ms.ToArray();
                }
            }
        }
    }
}
