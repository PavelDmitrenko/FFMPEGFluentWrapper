using System;

namespace FFMPEGWrapper
{
    public class FFMPEGEvent
    {

        /// <summary> Current audio level (as a percentage of the maximum recorded) </summary>
        public decimal CurrentAudioLevel { get; set; }
        
        /// <summary> Current bitrate in kb/s </summary>
        public decimal Bitrate { get; set; }

        /// <summary> File Size in bytes </summary>
        public long Size { get; set; }
       
        /// <summary> Recording duration </summary>
        public TimeSpan Duration { get; set; }
    }
}
