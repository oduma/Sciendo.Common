using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sciendo.Common.IO.MTP;

namespace Sciendo.Common.Tests
{
    [TestFixture]
    public class MTPDeviceManagerTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPortableDeviceNullPath()
        {
            MtpDeviceManager.GetPortableDevice("");
        }

        [Test]
        [ExpectedException(typeof(IOException))]
        public void GetPortableDeviceFSPath()
        {
            MtpDeviceManager.GetPortableDevice(@"C:\abc\abc1");
        }

        [Test]
        [Ignore("Neeeds device linked to run")]
        public void GetPortableDeviceOk()
        {

            Assert.AreEqual("Xperia XA", MtpDeviceManager.GetPortableDevice("Xperia XA").Name);
            Assert.True(MtpDeviceManager.GetPortableDevice("Xperia XA").IsConnected);
            Assert.IsNotNullOrEmpty(MtpDeviceManager.GetPortableDevice("Xperia XA").DeviceId);
        }
    }
}
