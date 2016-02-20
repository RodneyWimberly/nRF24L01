﻿using System;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SimpleReceiverRole : RoleBase
    {
        public event EventHandler<byte[]> DataArrived;

        public override bool Start()
        {
            if (!base.Start()) return false;

            Reader = Radio.ReceivePipes[1];
            Reader.AutoAcknowledgementEnabled = true;
            Reader.DynamicPayloadLengthEnabled = true;
            Reader.Address = ReceiveAddress;
            Reader.Enabled = true;

            Reader.FlushBuffer();
            Radio.TransmitPipe.FlushBuffer();

            Radio.Interrupted += Radio_Interrupted;
            Radio.Status = DeviceStatus.StandBy;
            Radio.Status = DeviceStatus.ReceiveMode;
            IsRunning = true;

            return IsRunning;
        }

        protected override void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
            if (e.StatusRegister.ReceiveDataReady && DataArrived != null)
            {
                DataArrived(this, Reader.ReadBufferAll());
                Radio.Status = DeviceStatus.StandBy;
                e.StatusRegister.Save();
                Radio.Status = DeviceStatus.ReceiveMode;
            }
        }
    }
}
