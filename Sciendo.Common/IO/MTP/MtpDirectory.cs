using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sciendo.Common.Logging;

namespace Sciendo.Common.IO.MTP
{
    public class MtpDirectory:IDirectory
    {

        public bool Exists(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var mtpFolder = GetMtpFolder(GetMtpDevice(path),path);
            if (mtpFolder==null)
            {
                return false;
            }
            return true;
        }

        private static PortableDeviceFolder GetMtpFolder(PortableDevice mtpDevice, string path)
        {
            if (mtpDevice == null) return null;
            var mtpFolder =
                mtpDevice.GetObject(path.Replace(MtpPathInterpreter.GetMtpDeviceName(path), string.Empty)) as
                    PortableDeviceFolder;
            if(mtpFolder==null)
                mtpDevice.Disconnect();
            return mtpFolder;
        }

        private static PortableDevice GetMtpDevice(string path)
        {
            PortableDevice mtpDevice;

            try
            {
                mtpDevice = MtpDeviceManager.GetPortableDevice(path);
            }
            catch (Exception e)
            {
                LoggingManager.LogSciendoSystemError(e);
                return null;
            }
            return mtpDevice;
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

        public IEnumerable<string> GetTopLevel(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));

            PortableDevice mtpDevice = GetMtpDevice(path);
            if (mtpDevice == null)
                return null;

            var mtpFolder = GetMtpFolder(mtpDevice,path);
            if (mtpFolder == null)
            {
                throw new IOException("Folder does not exist.");
            }
            return mtpFolder.GetChildren(mtpDevice).Select(f => f.Name);
        }


        public IEnumerable<string> GetFiles(string path, SearchOption searchOption, string[] extensions = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));
            var mtpDevice = GetMtpDevice(path);
            if (mtpDevice != null)
            {
                var mtpFolder = GetMtpFolder(mtpDevice, path);
                if (mtpFolder != null)
                {
                    DirectoryRead?.Invoke(this, new DirectoryReadEventArgs(path));
                    return GetContents(mtpDevice, mtpFolder, path, searchOption, extensions);
                }
            }
            return null;
        }

        private IEnumerable<string> GetContents(
            PortableDevice mtpDevice, 
            PortableDeviceFolder mtpFolder,
            string path, 
            SearchOption searchOption,
            string[] extensions=null)
        {
            if (extensions == null)
                extensions = new[] {"*.*"};
            foreach (var extension in extensions)
            {
                ExtensionsRead?.Invoke(this, new ExtensionsReadEventArgs(extension));
                foreach (var file in EnumerateFiles(mtpDevice, mtpFolder,path, searchOption)
                    .Where(s => s.ToLower().EndsWith(extension.ToLower()) || extension=="*.*"))
                {
                    yield return file;
                }

            }
        }

        private IEnumerable<string> EnumerateFiles(PortableDevice mtpDevice, 
            PortableDeviceFolder mtpFolder, string path, SearchOption searchOption)
        {
            foreach (var deviceObject in mtpFolder.GetChildren(mtpDevice))
            {
                if (deviceObject is PortableDeviceFile)
                    yield return $"{path}{Path.DirectorySeparatorChar}{deviceObject.Name}";
                else if (deviceObject is PortableDeviceFolder && searchOption==SearchOption.AllDirectories)
                    foreach (var fileName in EnumerateFiles(mtpDevice,deviceObject as PortableDeviceFolder, $"{path}{Path.DirectorySeparatorChar}{deviceObject.Name}",SearchOption.AllDirectories))
                    {
                        yield return $"{fileName}";
                    }
            }
        }

        public event EventHandler<DirectoryReadEventArgs> DirectoryRead;
        public event EventHandler<ExtensionsReadEventArgs> ExtensionsRead;

    }
}
