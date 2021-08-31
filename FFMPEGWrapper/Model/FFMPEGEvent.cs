using System;

namespace FFMPEGWrapper
{
    public class FFMPEGEvent
    {
        /// <summary> RMS max.level measured in dBFS </summary>
        public decimal MaxLevel { get; set; }
        
        /// <summary> RMS current level measured in dBFS </summary>
        public decimal CurrentLevel { get; set; }
        
        /// <summary> Current bitrate in kb/s </summary>
        public decimal Bitrate { get; set; }

        /// <summary> File Size in bytes </summary>
        public long Size { get; set; }
       
        /// <summary> Recording duration </summary>
        public TimeSpan Duration { get; set; }
    }
}
