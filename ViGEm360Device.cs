using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPluginInterface;
using InputMapperIcons;


namespace ViGEm_360
{
    public class ViGEm360Device : IOutputDevice, IFeedbackDevice
    {
        public InputMapperIcon.IMIcons icon { get { return InputMapperIcon.IMIcons.Xbox360; } }
        public double RumbleBig { get; set; }
        public double RumbleSmall { get; set; }
        public double RumbleAux1 { get; set; }
        public double RumbleAux2 { get; set; }

        private ViGemUm.VigemTarget _target;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler FeedbackReceived;
        ViGemUm.XusbReport OutputReport = new ViGemUm.XusbReport();

        static ViGEm360Device()
        {
            var error = ViGemUm.vigem_init();
            if (error == ViGemUm.VigemError.VIGEM_ERROR_NONE) return;
                throw new Exception(string.Format("Init error: {0}", ((ViGemUm.VigemError)error).ToString()));
        }

        public ViGEm360Device()
        {
            ViGemUm.VIGEM_TARGET_INIT(ref _target);

            var error = ViGemUm.vigem_target_plugin(VigemTargetType.Xbox360Wired, ref _target);

            if (error != ViGemUm.VigemError.VIGEM_ERROR_NONE)
                throw new Exception(string.Format("Init error: {0}", ((ViGemUm.VigemError)error).ToString()));

            ViGemUm.vigem_register_xusb_notification(XusbNotification, _target);
        }

        public void Dispose()
        {
            ViGemUm.vigem_target_unplug(ref _target);
        }

        private void XusbNotification(ViGemUm.VigemTarget target, byte largeMotor, byte smallMotor, byte ledNumber)
        {
            RumbleBig = (double)largeMotor / 255d;
            RumbleSmall = (double)smallMotor / 255d;

            FeedbackReceived?.Invoke(this, new EventArgs());
        }

        public bool sendInputs(DeviceState state)
        {
            OutputReport.sThumbLX = Convert.ToInt16(state.LX * 32767);
            OutputReport.sThumbLY = Convert.ToInt16(state.LY * 32767);
            OutputReport.sThumbRX = Convert.ToInt16(state.RX * 32767);
            OutputReport.sThumbRY = Convert.ToInt16(state.RY * 32767);
            OutputReport.bLeftTrigger = (byte)((int)(state.LeftTrigger * 255d));
            OutputReport.bRightTrigger = (byte)((int)(state.RightTrigger * 255d));

            OutputReport.wButtons = 0x0;
            //OutputReport.sSpecial = 0;

            if (state.FaceButton1) OutputReport.wButtons |= (ushort)(1 << 5);
            if (state.FaceButton2) OutputReport.wButtons |= (ushort)(1 << 6);
            if (state.FaceButton3) OutputReport.wButtons |= (ushort)(1 << 4);
            if (state.FaceButton4) OutputReport.wButtons |= (ushort)(1 << 7);

            if (state.LeftStickClick) OutputReport.wButtons |= (ushort)(1 << 14);
            if (state.RightStickClick) OutputReport.wButtons |= (ushort)(1 << 15);
            if (state.LeftBumper) OutputReport.wButtons |= (ushort)(1 << 8);
            if (state.RightBumper) OutputReport.wButtons |= (ushort)(1 << 9);
            if (state.SystemButton1) OutputReport.wButtons |= (ushort)(1 << 12);
            if (state.SystemButton2) OutputReport.wButtons |= (ushort)(1 << 13);
            //if (state.HomeButton) OutputReport.bSpecial |= (byte)(1 << 0);


            ushort dpad = 0x8;
            if (!state.DpadUp && !state.DpadDown && !state.DpadLeft && !state.DpadRight) dpad = 0x8;
            else if (state.DpadUp && !(state.DpadLeft || state.DpadRight)) dpad = 0x0;
            else if (state.DpadDown && !(state.DpadLeft || state.DpadRight)) dpad = 0x4;
            else if (state.DpadLeft && !(state.DpadUp || state.DpadDown)) dpad = 0x6;
            else if (state.DpadRight && !(state.DpadUp || state.DpadDown)) dpad = 0x2;
            else if (state.DpadUp && state.DpadRight) dpad = 0x1;
            else if (state.DpadDown && state.DpadRight) dpad = 0x3;
            else if (state.DpadUp && state.DpadLeft) dpad = 0x7;
            else if (state.DpadDown && state.DpadLeft) dpad = 0x5;

            OutputReport.wButtons |= dpad;

            try
            {
                var error = ViGemUm.vigem_xusb_submit_report(_target, OutputReport);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void init()
        {
            throw new NotImplementedException();
        }
    }
}
