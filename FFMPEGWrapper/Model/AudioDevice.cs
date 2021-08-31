namespace FFMPEGWrapper
{
    public class AudioDevice
    {
        public DeviceType DeviceType { get; }
        public string Name { get; }
        public bool IsDefault { get; }

        public AudioDevice(DeviceType deviceType, string name, bool isDefault)
        {
            DeviceType = deviceType;
            Name = name;
            IsDefault = isDefault;
        }
    }

    public enum DeviceType
    {
        Playback = 1,
        Capture = 2
    }
}
