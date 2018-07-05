using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;
using Sciendo.Common.Logging;

namespace Sciendo.Common.IO.MTP
{
    public class MtpFile:IFile
    {
        public void WriteAllText(string fileName, string content)
        {
            byte[] byteContent = Encoding.UTF8.GetBytes(content);
            Create(fileName,byteContent);
        }

        public void Create(string path, byte[] body)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var mtpDevice = MtpDeviceManager.GetPortableDevice(path);
            if (Exists(path))
                return;
            new MtpDirectory().CreateDirectory(Path.GetDirectoryName(path));
            var folderOnlyPath = Path.GetDirectoryName(path.Replace($"{MtpPathInterpreter.GetMtpDeviceName(path)}{Path.DirectorySeparatorChar}",
                string.Empty));
            string lastExistentFolderPath = folderOnlyPath;
            var mtpFolder =
                mtpDevice.GetLastExistentFolder(ref lastExistentFolderPath);
            if (lastExistentFolderPath != folderOnlyPath)
                throw new IOException();
            if (mtpFolder == null || string.IsNullOrEmpty(lastExistentFolderPath))
            {
                mtpDevice.Disconnect();
                throw new IOException();
            }
            try
            {
                PortableDeviceFile.Create(mtpDevice, mtpFolder, Path.GetFileName(path), body);

            }
            catch (Exception)
            {
                mtpDevice.Disconnect();
                throw;
            }
        }

        public bool Exists(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            PortableDevice mtpDevice;
            try
            {
                mtpDevice = MtpDeviceManager.GetPortableDevice(path);
            }
            catch (Exception e)
            {
                LoggingManager.LogSciendoSystemError(e);
                return false;
            }
            var mtpFile = mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as PortableDeviceFile;
            if (mtpFile == null)
            {
                mtpDevice.Disconnect();
                return false;
            }
            return true;
        }

        public void Delete(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            var mtpDevice = MtpDeviceManager.GetPortableDevice(path);

            var mtpFile = mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as PortableDeviceFile;
            if (mtpFile == null)
            {
                mtpDevice.Disconnect();
                return;
            }

            mtpFile.Delete(mtpDevice);
        }

        public byte[] Read(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var mtpDevice = MtpDeviceManager.GetPortableDevice(path);
            if (!Exists(path))
                throw new ArgumentException("Path does not exist.");

            var mtpFile = mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as PortableDeviceFile;
            if (mtpFile == null)
            {
                mtpDevice.Disconnect();
                throw new IOException("Path not accessible.");
            }


            IPortableDeviceResources resources;
            mtpDevice.Content.Transfer(out resources);

            PortableDeviceApiLib.IStream wpdStream;
            uint optimalTransferSize = 0;

            resources.GetStream(mtpFile.Id, ref PortableDevicePKeys.WPD_RESOURCE_DEFAULT, 0, ref optimalTransferSize, out wpdStream);

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

            MemoryStream targetStream = new MemoryStream();

            unsafe
            {
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
                    targetStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }
            var len = targetStream.Length;
            byte[] result= new byte[len];
            targetStream.Read(result, 0, (int)len);
            targetStream.Close();
            resources.Cancel();
            mtpDevice.Disconnect();
            return result;
        }

        public string ReadAllText(string path)
        {
            var bytes = Read(path);
            var len = bytes.Length;
            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetString(bytes, 0, len);
        }
    }
}
