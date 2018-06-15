using System;
using System.Collections.Generic;
using System.IO;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using IPortableDeviceKeyCollection = PortableDeviceApiLib.IPortableDeviceKeyCollection;
using IPortableDevicePropVariantCollection = PortableDeviceApiLib.IPortableDevicePropVariantCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using tag_inner_PROPVARIANT = PortableDeviceApiLib.tag_inner_PROPVARIANT;

namespace Sciendo.Common.IO.MTP
{
    public class PortableDeviceFolder : PortableDeviceObject
    {
        public PortableDeviceFolder(string id, string name) : base(id, name)
        {
        }
        public static PortableDeviceFolder Create(PortableDevice portableDevice, PortableDeviceFolder parentFolder, string folderName)
        {
            if(portableDevice==null || !portableDevice.IsConnected)
                throw new ArgumentNullException(nameof(portableDevice));
            if(parentFolder==null)
                throw new ArgumentNullException(nameof(parentFolder));
            if(string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));
            var folderNameParts = folderName.Split(new[] {Path.DirectorySeparatorChar},
                StringSplitOptions.RemoveEmptyEntries);

            PortableDeviceFolder newDeviceFolder = null;
            PortableDeviceFolder currentParentFolder = parentFolder;
            foreach (var folderNamePart in folderNameParts)
            {
                newDeviceFolder = CreateOneFolder(portableDevice, currentParentFolder, folderNamePart);
                if (newDeviceFolder == null)
                    return null;
                currentParentFolder = newDeviceFolder;
            }
            return newDeviceFolder;
        }

        private static PortableDeviceFolder CreateOneFolder(PortableDevice portableDevice, PortableDeviceFolder parentFolder,
            string folderName)
        {
            if(portableDevice==null || !portableDevice.IsConnected)
                throw new ArgumentNullException(nameof(portableDevice));
            if(parentFolder==null)
                throw new ArgumentNullException(nameof(parentFolder));
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(folderName, parentFolder.Id);
            if(values==null)
                throw new Exception($"Could not set values for creation of folder:{folderName}");
            var newFolderId = parentFolder.Id;
            portableDevice.Content.CreateObjectWithPropertiesOnly(
                values, ref newFolderId);
            return new PortableDeviceFolder(newFolderId, folderName);
        }

        private static IPortableDeviceValues GetRequiredPropertiesForContentType(string folderName, string parentFolderId)
        {
            if(string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));
            if(string.IsNullOrEmpty(parentFolderId))
                throw new ArgumentNullException(nameof(parentFolderId));
            IPortableDeviceValues values =
                new PortableDeviceValues() as IPortableDeviceValues;
            values.SetGuidValue(ref PortableDevicePKeys.WPD_OBJECT_FORMAT, PortableDeviceGuids.WPD_OBJECT_FORMAT_PROPERTIES_ONLY);

            values.SetGuidValue(ref PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER);

            values.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_PARENT_ID, parentFolderId);
            
            values.SetStringValue(PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME,folderName);

            values.SetStringValue(PortableDevicePKeys.WPD_OBJECT_NAME, folderName);


            return values;
        }

        public void Delete(PortableDevice mtpDevice, bool contentsAlso)
        {
            if(mtpDevice==null || !mtpDevice.IsConnected)
                throw new ArgumentNullException(nameof(mtpDevice));
            foreach (var child in GetChildren(mtpDevice))
            {
                if (contentsAlso)
                {
                    ((PortableDeviceFolder)child).Delete(mtpDevice, true);
                }
                else
                {
                    throw new IOException();
                }
            }

            tag_inner_PROPVARIANT variant;
            StringToPropVariant(Id, out variant);
            
            IPortableDevicePropVariantCollection objectIds =
                new PortableDevicePropVariantCollection()
                as IPortableDevicePropVariantCollection;
            objectIds.Add(variant);

            mtpDevice.Content.Delete(0, objectIds, null);

        }

        public IEnumerable<PortableDeviceObject> GetChildren(PortableDevice portableDevice, string filterPath="")
        {
            if(portableDevice==null || !portableDevice.IsConnected)
                throw new ArgumentNullException(nameof(portableDevice));
            IPortableDeviceProperties properties;
            portableDevice.Content.Properties(out properties);

            IEnumPortableDeviceObjectIDs objectIds;
            portableDevice.Content.EnumObjects(0, Id, null, out objectIds);
            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    var currentObject = WrapObject(properties, objectId);
                    if (string.IsNullOrEmpty(filterPath) || currentObject.Name.ToLower() == Path.GetFileName(filterPath).ToLower())
                        if(currentObject is PortableDeviceFolder)
                            yield return currentObject as PortableDeviceFolder;
                        else if (currentObject is PortableDeviceFile)
                            yield return currentObject as PortableDeviceFile;
                }
            } while (fetched > 0);
        }


        private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties,
            string objectId)
        {
            if(string.IsNullOrEmpty(objectId))
                throw new ArgumentNullException(nameof(objectId));

            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            string name;
            // Get the name of the object
            values.GetStringValue(PortableDevicePKeys.WPD_OBJECT_NAME, out name);

            // Get the type of the object
            Guid contentType;
            values.GetGuidValue(PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, out contentType);

            if (contentType == PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER
                || contentType == PortableDeviceGuids.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT)
            {
                return new PortableDeviceFolder(objectId, name);
            }
            string fileName;
            values.GetStringValue(PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, out fileName);
            return new PortableDeviceFile(objectId, fileName);
        }

    }
}
