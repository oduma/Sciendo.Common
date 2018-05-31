using System;
using System.IO;
using System.Linq;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;


namespace Sciendo.Common.IO.MTP
{
    public class PortableDevice
    {
        public bool IsConnected { get; private set; }
        private readonly PortableDeviceClass _device;
        private readonly string _deviceStringId = "DEVICE";
        public PortableDeviceFolder PortableDeviceRootFolder { get; private set; }
        private IPortableDeviceContent _content;

        public IPortableDeviceContent Content => _content;

        public PortableDevice(string deviceId)
        {
            _device = new PortableDeviceClass();
            DeviceId = deviceId;
        }

        public string DeviceId { get; set; }

        public string Name
        {
            get
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("Not connected to device.");
                }

                // Retrieve the properties of the device
                IPortableDeviceContent content;
                IPortableDeviceProperties properties;
                _device.Content(out content);
                content.Properties(out properties);

                // Retrieve the values for the properties
                IPortableDeviceValues propertyValues;
                properties.GetValues(_deviceStringId, null, out propertyValues);

                // Identify the property to retrieve
                var property = PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME;

                // Retrieve the friendly name
                string propertyValue;
                propertyValues.GetStringValue(ref property, out propertyValue);

                return propertyValue;
            }

        }

        public void Connect()
        {
            if (IsConnected) { return; }

            var clientInfo = new PortableDeviceValuesClass() as IPortableDeviceValues;
            _device.Open(DeviceId, clientInfo);
            IsConnected = true;

            PortableDeviceRootFolder = new PortableDeviceFolder(_deviceStringId, _deviceStringId);

            _device.Content(out _content);
        }

        public void Disconnect()
        {
            if (!IsConnected) { return; }
            _device.Close();
            IsConnected = false;

        }


        public PortableDeviceObject GetObject(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            if (PortableDeviceRootFolder == null)
                return null;

            var pathParts = path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var currentObject = PortableDeviceRootFolder as PortableDeviceObject;
            foreach (var pathPart in pathParts)
            {
                var portableDeviceFolder = (PortableDeviceFolder)currentObject;
                if (portableDeviceFolder != null)
                {
                    var childObjects = portableDeviceFolder.GetChildren(this, pathPart).ToArray();
                    if (childObjects.Length != 1)
                        return null;
                    currentObject = childObjects.FirstOrDefault();
                }
            }
            return currentObject;
        }

        public PortableDeviceFolder GetLastExistentFolder(ref string lastExistentFolderPath)
        {
            var mtpFolder = GetObject(lastExistentFolderPath);

            if (mtpFolder != null) return mtpFolder as PortableDeviceFolder;

            lastExistentFolderPath = lastExistentFolderPath.Substring(0,
                lastExistentFolderPath.LastIndexOf(Path.DirectorySeparatorChar));
            return GetLastExistentFolder(ref lastExistentFolderPath);
        }

    }
}
