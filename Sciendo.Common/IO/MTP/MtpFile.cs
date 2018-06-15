using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    }
}
