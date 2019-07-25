using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebCamFrames
{
    class Program
    {
        static void Main(string[] args)
        {
            // enumerate video devices
            var videoDevices = new FilterInfoCollection(
                    FilterCategory.VideoInputDevice);
            // create video source
            VideoCaptureDevice videoSource = new VideoCaptureDevice(
                    videoDevices[0].MonikerString);
            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            videoSource.ProvideSnapshots = true;

            // start the video source
            videoSource.Start();
            
            //videoSource.SignalToStop();


            Console.ReadLine();
        }

        private static void video_NewFrame(object sender,
                    NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;

            var fileName = @"C:\temp\camera\File_Frame.jpg";

            //bitmap.Save(string.Format(fileName));

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bitmap.Save(fileName, GetEncoder(ImageFormat.Jpeg), encoderParameters);

            var bytes = bitmap.ToByteArray(ImageFormat.Bmp);

            SendVarData(, bytes);

            Thread.Sleep(1000);
            // process the frame
        }

        private static int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}
