using System;
using System.IO;
using PortableDeviceApiLib;

namespace Sciendo.Common.IO.MTP
{
    internal class MtpDeviceManager
    {
        private readonly PortableDeviceManager _deviceManager;

        private MtpDeviceManager()
        {
            _deviceManager = new PortableDeviceManager();
        }


        private static MtpDeviceManager _mtpDeviceManager;
        private static MtpDeviceManager CreateInstance()
        {
            return _mtpDeviceManager ?? (_mtpDeviceManager = new MtpDeviceManager());
        }

        public static PortableDevice GetPortableDevice(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!MtpPathInterpreter.IsMtpDevice(path))
            {
                throw new IOException("Wrong path for this adapter.");
            }
            PortableDevice mtpDevice;
            if (!CreateInstance().TryGet(MtpPathInterpreter.GetMtpDeviceName(path), out mtpDevice))
            {
                mtpDevice?.Disconnect();
                throw new IOException();
            }
            return mtpDevice;
        }


        private bool TryGet(string deviceName, out PortableDevice portableDevice)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                portableDevice = null;
                return false;
            }

            _deviceManager.RefreshDeviceList();

            // Determine how many WPD devices are connected
            var deviceIds = new string[1];
            uint count = 1;
            _deviceManager.GetDevices(ref deviceIds[0], ref count);

            // Retrieve the device id for each connected device
            deviceIds = new string[count];
            _deviceManager.GetDevices(ref deviceIds[0], ref count);
            foreach (var deviceId in deviceIds)
            {
                portableDevice= new PortableDevice(deviceId);
                portableDevice.Connect();
                if (portableDevice.Name == deviceName)
                    return true;
            }
            portableDevice = null;
            return false;
        }
    }
}