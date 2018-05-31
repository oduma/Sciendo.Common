using PortableDeviceTypesLib;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using tag_inner_PROPVARIANT = PortableDeviceApiLib.tag_inner_PROPVARIANT;

namespace Sciendo.Common.IO.MTP
{
    public abstract class PortableDeviceObject
    {

        protected PortableDeviceObject(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        protected static void StringToPropVariant(
            string value,
            out tag_inner_PROPVARIANT propvarValue)

        {
            IPortableDeviceValues pValues =
                (IPortableDeviceValues)
                new PortableDeviceValuesClass();

            pValues.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ID, value);

            pValues.GetValue(ref PortableDevicePKeys.WPD_OBJECT_ID, out propvarValue);
        }
    }
}
