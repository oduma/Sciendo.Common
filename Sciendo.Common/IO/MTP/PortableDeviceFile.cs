using System;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceTypesLib;
using IPortableDevicePropVariantCollection = PortableDeviceApiLib.IPortableDevicePropVariantCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;
using tag_inner_PROPVARIANT = PortableDeviceApiLib.tag_inner_PROPVARIANT;

namespace Sciendo.Common.IO.MTP
{
    internal class PortableDeviceFile : PortableDeviceObject
    {
        public PortableDeviceFile(string id, string name) : base(id, name)
        {
        }

        public static void Create(PortableDevice portableDevice,
            PortableDeviceFolder parentFolder,
            string fileName,
            byte[] body)
        {
            if(portableDevice==null || !portableDevice.IsConnected)
                throw new ArgumentNullException(nameof(portableDevice));
            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if(body==null)
                throw new ArgumentNullException(nameof(body));
            IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(fileName, parentFolder.Id, (ulong) body.LongLength);
            PortableDeviceApiLib.IStream tempStream;
            uint optimalTransferSizeBytes = 0;

            portableDevice.Content.CreateObjectWithPropertiesAndData(
                values,
                out tempStream,
                ref optimalTransferSizeBytes,
                null);
            var targetStream = tempStream as IStream;
            try
            {
                using (var sourceStream = new MemoryStream(body))
                {
                    var buffer = new byte[body.Length];
                    if (sourceStream.Read(buffer, 0, body.Length) <= 0)
                        return;
                    IntPtr pcbWritten = IntPtr.Zero;
                    targetStream.Write(buffer, body.Length, pcbWritten);
                    targetStream.Commit(0);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(tempStream);
            }
        }

        private static IPortableDeviceValues GetRequiredPropertiesForContentType(
            string fileName,
            string parentFolderId,
            ulong contentLength)
        {
            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(fileName);
            if( string.IsNullOrEmpty(parentFolderId))
                throw new ArgumentNullException(nameof(parentFolderId));
            IPortableDeviceValues values = new PortableDeviceValues() as IPortableDeviceValues;

            values.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_PARENT_ID, parentFolderId);

            values.SetUnsignedLargeIntegerValue(ref PortableDevicePKeys.WPD_OBJECT_SIZE, contentLength);

            values.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, fileName);

            values.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_NAME, fileName);

            return values;
        }

        public void Delete(PortableDevice portableDevice)
        {
            if(portableDevice==null || !portableDevice.IsConnected)
                throw new ArgumentNullException(nameof(portableDevice));

            var variant = new tag_inner_PROPVARIANT();
            StringToPropVariant(Id, out variant);

            IPortableDevicePropVariantCollection objectIds =
                new PortableDevicePropVariantCollection()
                    as IPortableDevicePropVariantCollection;
            objectIds.Add(variant);

            portableDevice.Content.Delete(0, objectIds, null);


        }
    }
}