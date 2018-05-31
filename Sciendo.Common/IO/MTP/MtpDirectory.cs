using System;
using System.IO;
using Sciendo.Common.Logging;

namespace Sciendo.Common.IO.MTP
{
    public class MtpDirectory:IDirectory
    {

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
            var mtpFolder = mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as PortableDeviceFolder;
            if (mtpFolder==null)
            {
                mtpDevice.Disconnect();
                return false;
            }
            return true;
        }

        public void CreateDirectory(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var mtpDevice = MtpDeviceManager.GetPortableDevice(path);
            var folderOnlyPath = path.Replace($"{MtpPathInterpreter.GetMtpDeviceName(path)}{Path.DirectorySeparatorChar}",
                string.Empty);
            string lastExistentFolderPath =folderOnlyPath;
            var mtpFolder =
                mtpDevice.GetLastExistentFolder(ref lastExistentFolderPath);
            if (lastExistentFolderPath == folderOnlyPath)
                return;
            if (mtpFolder==null || string.IsNullOrEmpty(lastExistentFolderPath))
            {
                mtpDevice.Disconnect();
                throw new IOException();
            }
            var newMtpFolder = PortableDeviceFolder.Create(mtpDevice, mtpFolder, folderOnlyPath.Replace($"{lastExistentFolderPath}{Path.DirectorySeparatorChar}", String.Empty));
            if (newMtpFolder == null)
            {
                mtpDevice.Disconnect();
                throw new IOException();
            }
        }

        public void Delete(string path, bool contentsAlso)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            var mtpDevice = MtpDeviceManager.GetPortableDevice(path);

            var mtpFolder = mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as PortableDeviceFolder;
            if (mtpFolder == null)
            {
                mtpDevice.Disconnect();
                return;
            }

            mtpFolder.Delete(mtpDevice, contentsAlso);
        }
    }
}
