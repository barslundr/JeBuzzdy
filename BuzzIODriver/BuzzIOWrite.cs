using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericHid;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace BuzzIOWrite
{
    public class BuzzIOWrite
    {
        private Debugging MyDebugging = new Debugging(); //  For viewing results of API calls via Debug.Write.
        private DeviceManagement MyDeviceManagement = new DeviceManagement();
        private Hid MyHid = new Hid(); 
        private IntPtr deviceNotificationHandle;
        private Boolean exclusiveAccess;
        private SafeFileHandle hidHandle;
        private String hidUsage;
        private Boolean myDeviceDetected;
        private String myDevicePathName;
        private SafeFileHandle readHandle;
        private SafeFileHandle writeHandle;

        public Boolean FindTheHid()
        {
            Boolean deviceFound = false;
            String[] devicePathName = new String[128];
            String functionName = "";
            Guid hidGuid = Guid.Empty;
            Int32 memberIndex = 0;
            Int16 myProductID = 1356;
            Int16 myVendorID = 2;
            Boolean success = false;

            try
            {
                myDeviceDetected = false;

                //  Get the device's Vendor ID and Product ID from the form's text boxes.

                //GetVendorAndProductIDsFromTextBoxes( ref myVendorID, ref myProductID ); 

                //  ***
                //  API function: 'HidD_GetHidGuid

                //  Purpose: Retrieves the interface class GUID for the HID class.

                //  Accepts: 'A System.Guid object for storing the GUID.
                //  ***

                Hid.HidD_GetHidGuid(ref hidGuid);

                functionName = "GetHidGuid";
                
                //Debug.WriteLine(MyDebugging.ResultOfAPICall(functionName));
                //Debug.WriteLine("  GUID for system HIDs: " + hidGuid.ToString());

                //  Fill an array with the device path names of all attached HIDs.

                deviceFound = MyDeviceManagement.FindDeviceFromGuid(hidGuid, ref devicePathName);

                //  If there is at least one HID, attempt to read the Vendor ID and Product ID
                //  of each device until there is a match or all devices have been examined.

                if (deviceFound)
                {
                    memberIndex = 0;

                    do
                    {
                        //  ***
                        //  API function:
                        //  CreateFile

                        //  Purpose:
                        //  Retrieves a handle to a device.

                        //  Accepts:
                        //  A device path name returned by SetupDiGetDeviceInterfaceDetail
                        //  The type of access requested (read/write).
                        //  FILE_SHARE attributes to allow other processes to access the device while this handle is open.
                        //  A Security structure or IntPtr.Zero. 
                        //  A creation disposition value. Use OPEN_EXISTING for devices.
                        //  Flags and attributes for files. Not used for devices.
                        //  Handle to a template file. Not used.

                        //  Returns: a handle without read or write access.
                        //  This enables obtaining information about all HIDs, even system
                        //  keyboards and mice. 
                        //  Separate handles are used for reading and writing.
                        //  ***

                        hidHandle = FileIO.CreateFile(devicePathName[memberIndex], 0, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);

                        functionName = "CreateFile";
                        //Debug.WriteLine(MyDebugging.ResultOfAPICall(functionName));
                        //Debug.WriteLine("  Returned handle: " + hidHandle.ToString());

                        if (!hidHandle.IsInvalid)
                        {
                            //  The returned handle is valid, 
                            //  so find out if this is the device we're looking for.

                            //  Set the Size property of DeviceAttributes to the number of bytes in the structure.

                            MyHid.DeviceAttributes.Size = Marshal.SizeOf(MyHid.DeviceAttributes);

                            //  ***
                            //  API function:
                            //  HidD_GetAttributes

                            //  Purpose:
                            //  Retrieves a HIDD_ATTRIBUTES structure containing the Vendor ID, 
                            //  Product ID, and Product Version Number for a device.

                            //  Accepts:
                            //  A handle returned by CreateFile.
                            //  A pointer to receive a HIDD_ATTRIBUTES structure.

                            //  Returns:
                            //  True on success, False on failure.
                            //  ***                            

                            success = Hid.HidD_GetAttributes(hidHandle, ref MyHid.DeviceAttributes);

                            if (success)
                            {
                                Debug.WriteLine("  HIDD_ATTRIBUTES structure filled without error.");
                                Debug.WriteLine("  Structure size: " + MyHid.DeviceAttributes.Size);
                                Debug.WriteLine("  Vendor ID: " + Convert.ToString(MyHid.DeviceAttributes.VendorID, 16));
                                Debug.WriteLine("  Product ID: " + Convert.ToString(MyHid.DeviceAttributes.ProductID, 16));
                                Debug.WriteLine("  Version Number: " + Convert.ToString(MyHid.DeviceAttributes.VersionNumber, 16));

                                //  Find out if the device matches the one we're looking for.

                                if ((MyHid.DeviceAttributes.VendorID == myVendorID) && (MyHid.DeviceAttributes.ProductID == myProductID))
                                {

                                    Debug.WriteLine("  My device detected");

                                    //  Display the information in form's list box.

                                    //lstResults.Items.Add("Device detected:");
                                    //lstResults.Items.Add("  Vendor ID= " + Convert.ToString(MyHid.DeviceAttributes.VendorID, 16));
                                    //lstResults.Items.Add("  Product ID = " + Convert.ToString(MyHid.DeviceAttributes.ProductID, 16));

                                    //ScrollToBottomOfListBox();

                                    myDeviceDetected = true;

                                    //  Save the DevicePathName for OnDeviceChange().

                                    myDevicePathName = devicePathName[memberIndex];
                                }
                                else
                                {
                                    //  It's not a match, so close the handle.

                                    myDeviceDetected = false;
                                    hidHandle.Close();
                                }
                            }
                            else
                            {
                                //  There was a problem in retrieving the information.

                                Debug.WriteLine("  Error in filling HIDD_ATTRIBUTES structure.");
                                myDeviceDetected = false;
                                hidHandle.Close();
                            }
                        }

                        //  Keep looking until we find the device or there are no devices left to examine.

                        memberIndex = memberIndex + 1;
                    }
                    while (!((myDeviceDetected || (memberIndex == devicePathName.Length))));
                }

                if (myDeviceDetected)
                {
                    //  The device was detected.
                    //  Register to receive notifications if the device is removed or attached.

                    success = MyDeviceManagement.RegisterForDeviceNotifications(myDevicePathName, new System.Windows.Forms.Form().Handle, hidGuid, ref deviceNotificationHandle);

                    Debug.WriteLine("RegisterForDeviceNotifications = " + success);

                    //  Learn the capabilities of the device.

                    MyHid.Capabilities = MyHid.GetDeviceCapabilities(hidHandle);

                    if (success)
                    {
                        //  Find out if the device is a system mouse or keyboard.

                        hidUsage = MyHid.GetHidUsage(MyHid.Capabilities);

                        //  Get the Input report buffer size.

                        //GetInputReportBufferSize();
                        //cmdInputReportBufferSize.Enabled = true;

                        //  Get handles to use in requesting Input and Output reports.

                        readHandle = FileIO.CreateFile(myDevicePathName, FileIO.GENERIC_READ, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, FileIO.FILE_FLAG_OVERLAPPED, 0);

                        functionName = "CreateFile, ReadHandle";
                        Debug.WriteLine(MyDebugging.ResultOfAPICall(functionName));
                        Debug.WriteLine("  Returned handle: " + readHandle.ToString());

                        if (readHandle.IsInvalid)
                        {
                            exclusiveAccess = true;
                            //lstResults.Items.Add("The device is a system " + hidUsage + ".");
                            //lstResults.Items.Add("Windows 2000 and Windows XP obtain exclusive access to Input and Output reports for this devices.");
                            //lstResults.Items.Add("Applications can access Feature reports only.");
                            //ScrollToBottomOfListBox();
                        }
                        else
                        {
                            writeHandle = FileIO.CreateFile(myDevicePathName, FileIO.GENERIC_WRITE, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);

                            functionName = "CreateFile, WriteHandle";
                            Debug.WriteLine(MyDebugging.ResultOfAPICall(functionName));
                            Debug.WriteLine("  Returned handle: " + writeHandle.ToString());

                            //  Flush any waiting reports in the input buffer. (optional)

                            MyHid.FlushQueue(readHandle);
                        }
                    }
                }
                else
                {
                    //  The device wasn't detected.

                    //lstResults.Items.Add("Device not found.");
                    //cmdInputReportBufferSize.Enabled = false;
                    //cmdOnce.Enabled = true;

                    Debug.WriteLine(" Device not found.");

                    //ScrollToBottomOfListBox();
                }
                return myDeviceDetected;
            }
            catch (Exception ex)
            {
                //DisplayException(this.Name, ex);
                throw;
            }
        }         
    }
}