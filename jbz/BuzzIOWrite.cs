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
            Int16 myProductID = 2;
            Int16 myVendorID = 1356;
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
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public void Write(bool c1, bool c2, bool c3, bool c4)
        {
            try
            {
                //  If the device hasn't been detected, was removed, or timed out on a previous attempt
                //  to access it, look for the device.

                if ((myDeviceDetected == false))
                {
                    myDeviceDetected = FindTheHid();
                }

                if ((myDeviceDetected == true))
                {
                    WriteToDevice(c1, c2, c3, c4);
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }             
        }

        private void WriteToDevice(bool c1, bool c2, bool c3, bool c4)
        {
            String byteValue = null;
            Int32 count = 0;            
            Byte[] outputReportBuffer = null;
            Boolean success = false;

            try
            {
                success = false;

                //  Don't attempt to exchange reports if valid handles aren't available
                //  (as for a mouse or keyboard under Windows 2000/XP.)

                if (!readHandle.IsInvalid && !writeHandle.IsInvalid)
                {
                    //  Don't attempt to send an Output report if the HID has no Output report.

                    if (MyHid.Capabilities.OutputReportByteLength > 0)
                    {
                        //  Set the size of the Output report buffer.   

                        outputReportBuffer = new Byte[MyHid.Capabilities.OutputReportByteLength];

                        //  Store the report ID in the first byte of the buffer:

                        outputReportBuffer[0] = 0;

                        //  Store the report data following the report ID.
                        //  Use the data in the combo boxes on the form.

                        //  Write a report.

                        outputReportBuffer[0] = 0;
                        outputReportBuffer[1] = 0;
                        outputReportBuffer[6] = 0;
                        outputReportBuffer[7] = 0;


                        if (c1==true)
                            outputReportBuffer[2] = 255;
                        else
                            outputReportBuffer[2] = 0;

                        if (c2 == true)
                            outputReportBuffer[3] = 255;
                        else
                            outputReportBuffer[3] = 0;

                        if (c3 == true)
                            outputReportBuffer[4] = 255;
                        else
                            outputReportBuffer[4] = 0;

                        if (c4 == true)
                            outputReportBuffer[5] = 255;
                        else
                            outputReportBuffer[5] = 0;

                        //if ((chkUseControlTransfersOnly.Checked) == true)
                        //{

                        //    //  Use a control transfer to send the report,
                        //    //  even if the HID has an interrupt OUT endpoint.

                        //    Hid.OutputReportViaControlTransfer myOutputReport = new Hid.OutputReportViaControlTransfer();
                        //    success = myOutputReport.Write(outputReportBuffer, writeHandle);
                        //}
                        //else
                        //{

                            //  Use WriteFile to send the report.
                            //  If the HID has an interrupt OUT endpoint, WriteFile uses an 
                            //  interrupt transfer to send the report. 
                            //  If not, WriteFile uses a control transfer.

                            Hid.OutputReportViaInterruptTransfer myOutputReport = new Hid.OutputReportViaInterruptTransfer();
                            success = myOutputReport.Write(outputReportBuffer, writeHandle);
                        //}

                        if (success)
                        {
                            //lstResults.Items.Add("An Output report has been written.");

                            //  Display the report data in the form's list box.

                            //lstResults.Items.Add(" Output Report ID: " + String.Format("{0:X2} ", outputReportBuffer[0]));
                            //lstResults.Items.Add(" Output Report Data:");

                            //txtBytesReceived.Text = "";
                            for (count = 0; count <= outputReportBuffer.Length - 1; count++)
                            {

                                //  Display bytes as 2-character hex strings.

                                byteValue = String.Format("{0:X2} ", outputReportBuffer[count]);
                                //lstResults.Items.Add(" " + byteValue);
                            }
                        }
                        else
                        {
                            //lstResults.Items.Add("The attempt to write an Output report has failed.");
                        }
                    }
                    else
                    {
                        //lstResults.Items.Add("The HID doesn't have an Output report.");
                    }

                    ////  Read an Input report.

                    //success = false;

                    ////  Don't attempt to send an Input report if the HID has no Input report.
                    ////  (The HID spec requires all HIDs to have an interrupt IN endpoint,
                    ////  which suggests that all HIDs must support Input reports.)

                    //if (MyHid.Capabilities.InputReportByteLength > 0)
                    //{
                    //    //  Set the size of the Input report buffer. 

                    //    inputReportBuffer = new Byte[MyHid.Capabilities.InputReportByteLength];

                    //    if (chkUseControlTransfersOnly.Checked)
                    //    {
                    //        //  Read a report using a control transfer.

                    //        Hid.InputReportViaControlTransfer myInputReport = new Hid.InputReportViaControlTransfer();

                    //        //  Read the report.

                    //        myInputReport.Read(hidHandle, readHandle, writeHandle, ref myDeviceDetected, ref inputReportBuffer, ref success);

                    //        if (success)
                    //        {
                    //            lstResults.Items.Add("An Input report has been read.");

                    //            //  Display the report data received in the form's list box.

                    //            lstResults.Items.Add(" Input Report ID: " + String.Format("{0:X2} ", inputReportBuffer[0]));
                    //            lstResults.Items.Add(" Input Report Data:");

                    //            txtBytesReceived.Text = "";

                    //            for (count = 0; count <= inputReportBuffer.Length - 1; count++)
                    //            {
                    //                //  Display bytes as 2-character Hex strings.

                    //                byteValue = String.Format("{0:X2} ", inputReportBuffer[count]);

                    //                lstResults.Items.Add(" " + byteValue);

                    //                //  Display the received bytes in the text box.

                    //                txtBytesReceived.SelectionStart = txtBytesReceived.Text.Length; ///* TRANSINFO: .NET Equivalent of Microsoft.VisualBasic NameSpace */ System.Runtime.InteropServices.Marshal.SizeOf(txtBytesReceived.Text); 
                    //                txtBytesReceived.SelectedText = byteValue + "\r\n";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            lstResults.Items.Add("The attempt to read an Input report has failed.");
                    //        }

                    //        ScrollToBottomOfListBox();

                    //        //  Enable requesting another transfer.

                    //        AccessForm("EnableCmdOnce", "");
                    //    }
                    //    else
                    //    {
                    //        //  Read a report using interrupt transfers.                
                    //        //  To enable reading a report without blocking the main thread, this
                    //        //  application uses an asynchronous delegate.

                    //        IAsyncResult ar = null;
                    //        Hid.InputReportViaInterruptTransfer myInputReport = new Hid.InputReportViaInterruptTransfer();

                    //        //  Define a delegate for the Read method of myInputReport.

                    //        ReadInputReportDelegate MyReadInputReportDelegate = new ReadInputReportDelegate(myInputReport.Read);

                    //        //  The BeginInvoke method calls myInputReport.Read to attempt to read a report.
                    //        //  The method has the same parameters as the Read function,
                    //        //  plus two additional parameters:
                    //        //  GetInputReportData is the callback procedure that executes when the Read function returns.
                    //        //  MyReadInputReportDelegate is the asynchronous delegate object.
                    //        //  The last parameter can optionally be an object passed to the callback.

                    //        ar = MyReadInputReportDelegate.BeginInvoke(hidHandle, readHandle, writeHandle, ref myDeviceDetected, ref inputReportBuffer, ref success, new AsyncCallback(GetInputReportData), MyReadInputReportDelegate);
                    //    }
                    //}
                    //else
                    //{
                    //    //lstResults.Items.Add("No attempt to read an Input report was made.");
                    //    //lstResults.Items.Add("The HID doesn't have an Input report.");
                    //}
                }
                else
                {
                    //lstResults.Items.Add("Invalid handle. The device is probably a system mouse or keyboard.");
                    //lstResults.Items.Add("No attempt to write an Output report or read an Input report was made.");
                }
                //ScrollToBottomOfListBox();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }             
        }
        
    }
}