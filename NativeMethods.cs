using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ViGEm_360
{
    public enum VigemTargetType
    {
        Xbox360Wired,
        XboxOneWired,
        DualShock4Wired
    }

    internal static partial class ViGemUm
    {
        #region Enums

        public enum VigemError : uint
        {
            VIGEM_ERROR_NONE = 0x20000000,
            VIGEM_ERROR_BUS_NOT_FOUND = 0xE0000001,
            VIGEM_ERROR_NO_FREE_SLOT = 0xE0000002,
            VIGEM_ERROR_INVALID_TARGET = 0xE0000003,
            VIGEM_ERROR_REMOVAL_FAILED = 0xE0000004,
            VIGEM_ERROR_ALREADY_CONNECTED = 0xE0000005,
            VIGEM_ERROR_TARGET_UNINITIALIZED = 0xE0000006,
            VIGEM_ERROR_TARGET_NOT_PLUGGED_IN = 0xE0000007,
            VIGEM_ERROR_BUS_VERSION_MISMATCH = 0xE0000008,
            VIGEM_ERROR_BUS_ACCESS_FAILED = 0xE0000009
        }

        public enum VigemTargetState
        {
            VigemTargetNew,
            VigemTargetInitialized,
            VigemTargetConnected,
            VigemTargetDisconnected
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct VigemTarget
        {
            public uint Size;
            public uint SerialNo;
            public VigemTargetState State;
            public ushort VendorId;
            public ushort ProductId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Ds4LightbarColor
        {
            public byte Red;
            public byte Green;
            public byte Blue;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XusbReport
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Ds4Report
        {
            byte bThumbLX;
            byte bThumbLY;
            byte bThumbRX;
            byte bThumbRY;
            ushort wButtons;
            byte bSpecial;
            byte bTriggerL;
            byte bTriggerR;
        }

        #endregion

        public delegate void VigemXusbNotification(
            VigemTarget target,
            byte largeMotor,
            byte smallMotor,
            byte ledNumber);

        public delegate void VigemDs4Notification(
            VigemTarget target,
            byte largeMotor,
            byte smallMotor,
            Ds4LightbarColor lightbarColor);

        public static void VIGEM_TARGET_INIT(
            [In, Out] ref VigemTarget target)
        {
            target.Size = (uint)Marshal.SizeOf(typeof(VigemTarget));
            target.State = VigemTargetState.VigemTargetInitialized;
        }

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_init();

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void vigem_shutdown();

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_target_plugin(
            [In] VigemTargetType type,
            [In, Out] ref VigemTarget target);

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void vigem_target_unplug(
            [In, Out] ref VigemTarget target);

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_register_xusb_notification(
            [In, MarshalAs(UnmanagedType.FunctionPtr)] VigemXusbNotification notification,
            [In] VigemTarget target);

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_register_ds4_notification(
            [In, MarshalAs(UnmanagedType.FunctionPtr)] VigemDs4Notification notification,
            [In] VigemTarget target);

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_xusb_submit_report(
            [In] VigemTarget target,
            [In] XusbReport report);

        [DllImport("ViGEmUM.dll", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VigemError vigem_ds4_submit_report(
            [In] VigemTarget target,
            [In] Ds4Report report);
    }
}
