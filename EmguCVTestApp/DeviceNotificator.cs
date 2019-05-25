using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace EmguCVTestApp
{
    /// <summary>
    /// Класс, позволяющий получать оповещения об изменении набора USB-устройств.
    /// Генерирует невидимое окно Windows для получения сообщений.
    /// </summary>
    public class DeviceNotificator : NativeWindow
    {
        /// <summary>
        /// Событие вызывается при добавлении устройства.
        /// </summary>
        public event EventHandler DeviceAdded;
        /// <summary>
        /// Событие вызывается при удалении устройства.
        /// </summary>
        public event EventHandler DeviceRemoved;
        /// <summary>
        /// Событие вызывается при добавлении или удалении устройства.
        /// </summary>
        public event EventHandler DeviceListChanged;
        /// <summary>
        /// Отслеживает добавление и удаление USB устройств.
        /// </summary>
        public DeviceNotificator() : base()
        {
            subscriptionGuid = USB; // если не указано иное, нас интересуют USB устройства
        }
        /// <summary>
        /// Отслеживает добавление и удаление устройств, принадлежащих к классу с заданным GUID.
        /// </summary>
        /// <param name="guid">GUID класса интерфейса устройств, которые мы отслеживаем.</param>
        public DeviceNotificator(Guid guid) : base()
        {
            subscriptionGuid = guid;
        }
        /// <summary>
        /// Позволяет узнать текущий статус механизма уведомлений, и управлять им.
        /// </summary>
        public bool Enabled
        {
            get { return notificationHandle != IntPtr.Zero; }
            set
            {
                if (value && (notificationHandle == IntPtr.Zero))
                    Enable();
                else if (!value && (notificationHandle != IntPtr.Zero))
                    Disable();
            }
        }
        /// <summary>
        /// Активирует получение уведомлений.
        /// </summary>
        public void Enable()
        {
            if (notificationHandle != IntPtr.Zero) // если уведомления уже активны, ничего не делаем
                return;
            if (this.Handle == IntPtr.Zero) // если окно не было создано, создаём
                CreateHandle(new CreateParams());
            // Структура, описывающая желаемую категорию уведомлений.
            DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = subscriptionGuid,
                Name = 0
            };
            // Преобразуем структуру в указатель для передачи в WINAPI функцию
            dbi.Size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);
            // Вызываем WinAPI функцию для подписки на уведомления
            notificationHandle = RegisterDeviceNotification(this.Handle, buffer, 0);
        }
        /// <summary>
        /// Деактивирует получение уведомлений.
        /// </summary>
        public void Disable()
        {
            if (notificationHandle != IntPtr.Zero) // если мы уже подписаны на уведомления, отписываемся
            {
                UnregisterDeviceNotification(notificationHandle);
                notificationHandle = IntPtr.Zero;
            }
        }

        /// <summary>Устройства Bluetooth. Доступно начиная с Windows XP SP2.</summary>
        public static readonly Guid Bluetooth = new Guid("0850302A-B344-4fda-9BE9-90576B8D46F0");
        /// <summary>Видеоадаптеры.</summary>
        public static readonly Guid DisplayAdapter = new Guid("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");
        /// <summary>Устройства захвата одиночных изображений (камеры и сканеры).</summary>
        public static readonly Guid Imaging = new Guid("6BDD1FC6-810F-11D0-BEC7-08002BE2092F");
        /// <summary>Мониторы.</summary>
        public static readonly Guid Monitor = new Guid("E6F07B5F-EE97-4a90-B076-33F57BF4EAA7");
        /// <summary>COM порты.</summary>
        public static readonly Guid COMPort = new Guid("86E0D1E0-8089-11D0-9CE4-08003E301F73");
        /// <summary>LPT порты.</summary>
        public static readonly Guid LPTPort = new Guid("97F76EF0-F883-11D0-AF1F-0000F800845C");
        /// <summary>Дисковый раздел.</summary>
        public static readonly Guid Partition = new Guid("53F5630A-B6BF-11D0-94F2-00A0C91EFB8B");
        /// <summary>USB устройства.</summary>
        public static readonly Guid USB = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        #region Реализация
        const int DbtDevicearrival = 0x8000; // суб-сообщение - новое устройство
        const int DbtDeviceremovecomplete = 0x8004; // суб-сообщение - устройство удалено
        const int WmDevicechange = 0x0219; // оконное сообщение - изменился список устройств
        const int DbtDevtypDeviceinterface = 5; // Нас интересует класс устройств

        Guid subscriptionGuid; //GUID класса устройств, чьё появление/удаление мы отслеживаем
        IntPtr notificationHandle = IntPtr.Zero; // Дескриптор подписки.
        /// <summary>
        /// Процедура обработки оконных сообщений.
        /// </summary>
        /// <param name="m">Структура, описывающая сообщения.</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == DeviceNotificator.WmDevicechange) // сообщение - изменился состав устройств.
            {
                EventArgs e = new EventArgs();
                if ((int)m.WParam == DeviceNotificator.DbtDevicearrival) // добавилось новое?
                {
                    DeviceAdded?.Invoke(this, e);
                    DeviceListChanged?.Invoke(this, e);
                }
                else if ((int)m.WParam == DeviceNotificator.DbtDeviceremovecomplete) // удалили старое?
                {
                    DeviceRemoved?.Invoke(this, e);
                    DeviceListChanged?.Invoke(this, e);
                }
            }
        }
        // подключаем функции WinAPI
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);
        // необходимая структура данных
        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
        #endregion
    }
}
