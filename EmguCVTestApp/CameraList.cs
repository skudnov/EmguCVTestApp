using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DirectShowLib;

namespace EmguCVTestApp
{
    /// <summary>
    /// Cписок, содержащий список доступных камер. Умеет обнаруживать подключение/отключение устройств.
    /// Совместим с OpenCV - индекс камеры в списке соответствует параметру конструктора VideoCapture.
    /// </summary>
    public class CameraList : BindingList<CameraDeviceRecord>
    {
        private DeviceNotificator Notify; // отслеживатель состава USB устройств.
        public CameraList(bool track = true)
        {
            Notify = new DeviceNotificator(DeviceNotificator.USB);
            Notify.DeviceListChanged += (sender, e) => Update();
            // получаем текущий список камер
            foreach (DsDevice d in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                this.Add(new CameraDeviceRecord(d));
            // подписываемся на события, если нужно
            Track = track;
        }
        /// <summary>
        /// Должно ли производиться отслеживание добавления/удаления устройств.
        /// </summary>
        public bool Track
        {
            get { return Notify.Enabled; }
            set { Notify.Enabled = value; }
        }
        /// <summary>
        /// Получает текущий список камер и обновляет содержимое коллекции.
        /// </summary>
        public void Update()
        {
            DsDevice[] cams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            bool same = this.Count == cams.Length;
            for (int i = 0; same && (i < cams.Length); i++)
                same = this[i].Equals(cams[i]);
            if (!same)
            {
                this.RaiseListChangedEvents = false;
                this.Clear();
                foreach (DsDevice d in cams)
                    this.Add(new CameraDeviceRecord(d));
                this.RaiseListChangedEvents = true;
                this.ResetBindings();
            }
        }
    }
    /// <summary>
    /// Простая структура, позволяющая наглядно отображать список камер.
    /// </summary>
    [Serializable]
    public struct CameraDeviceRecord : IEquatable<DsDevice>, IEquatable<CameraDeviceRecord>
    {
        public readonly DsDevice Device;
        public string Name { get => Device.Name; }
        public override string ToString() => Device.Name;
        public CameraDeviceRecord(DsDevice d) { Device = d; }

        public override int GetHashCode() => this.Device.DevicePath.GetHashCode();
        public bool Equals(DsDevice other) => (this.Device.ClassID == other.ClassID) && (this.Device.DevicePath == other.DevicePath);
        public bool Equals(CameraDeviceRecord other) => this.Equals(other.Device);

        public override bool Equals(object other)
        {
            if (other is CameraDeviceRecord) return this.Equals((CameraDeviceRecord)other);
            else if (other is DsDevice) return this.Equals((DsDevice)other);
            else return false;
        }
        public static implicit operator CameraDeviceRecord(DsDevice d) => new CameraDeviceRecord(d);
        
    }
}
