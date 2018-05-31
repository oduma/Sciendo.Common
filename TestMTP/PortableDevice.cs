using System;
using System.IO;
using System.Collections.Generic;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using IPortableDeviceKeyCollection = PortableDeviceApiLib.IPortableDeviceKeyCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using System.Runtime.InteropServices;


namespace Sciendo.Common.IO.MTP
{
    public class PortableDevice
    {
        private bool _isConnected;
        private readonly PortableDeviceClass _device;

        public PortableDevice(string deviceId)
        {
            this._device = new PortableDeviceClass();
            this.DeviceId = deviceId;
        }

        public string DeviceId { get; set; }
        
    }
}
