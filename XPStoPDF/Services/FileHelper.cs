using Ascon.Pilot.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IDataObject = Ascon.Pilot.SDK.IDataObject;

namespace XPStoPDF.Services
{
    internal static class FileHelper
    {
        internal static string GetJpgFilePath(FrameworkElement fe, double scale)
        {
            string output = "";

            var bmp = new RenderTargetBitmap((int)(fe.ActualWidth * scale),
                                             (int)(fe.ActualHeight * scale),
                                             96d * scale,
                                             96d * scale,
                                             PixelFormats.Default);

            bmp.Render(fe);

            var encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            output = Path.GetTempFileName();

            using (Stream stream = File.Create(output))
            {
                encoder.Save(stream);
            }

            return output;
        }

        public static bool MoveFile(string oldPath, string newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);

                    return true;
                }
            }
            catch (Exception ex)
            {}

            return false;
        }

        public static string GetTempFilePathWitExtention(string newExt, string suffix = null)
        {
            var path = Path.GetTempFileName();
            var dir = Path.GetDirectoryName(path);
            var tmpShortfileName = Path.GetFileNameWithoutExtension(path);

            if (suffix == null)
            {
                suffix = "";
            }

            var xpsPath = Path.Combine(dir, tmpShortfileName + suffix + "." + newExt);

            if (File.Exists(xpsPath))
            {
                File.Delete(xpsPath);
            }

            if (File.Exists(path))
            {
                File.Move(path, xpsPath);

                return xpsPath;
            }

            return "";
        }

        internal static bool CheckOnXps(IEnumerable<IDataObject> selectedObjects)
        {
            return selectedObjects.Any(p => CheckOnXps(p));
        }


        internal static bool CheckOnXps(IDataObject obj)
        {
            if (!obj.Files.Any())
            {
                return false;
            }

            var ext = Path.GetExtension(obj.Files[0].Name);

            if (ext != ".xps")
            {
                return false;
            }

            return true;
        }


   

    }
}