# .NET FFMPEG Fluent Wrapper
### A fluent wrapper to FFMPEG

* Enumerate DirectShow capture devices;
* Capture audio from DirectShow device;

## Dependencies
+ FFMPEG executable file

## Usage
```csharp
// Enumerate DirectShow capture devices
FFMPEG ffmpeg = new FFMPEG();
List<AudioDevice> captureDevices = ffmpeg.FFMPEGPath(<path_to_ffmpeg_executable>)
                                    .EnumerateCaptureDevices();
 
// Capture
FFMPEG ffmpeg = new FFMPEG();
ffmpeg.FFMPEGPath(<path_to_ffmpeg_executable>)
        .EncodingFormat(EncodingFormat.MP3)
        .Bitrate(128)
        .Sampling(44100)
        .Channels(Channels.Mono)
        .CaptureDevice(<name_of_directshow_device>)
        .Destination(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3"))
        .OnEvent(data =>
            {
                /// MaxLevel: RMS max.level
                /// CurrentLevel: RMS current level
                /// Bitrate: Current bitrate in kb/s
                /// Size: File Size in bytes
                /// Duration: Recording duration (TimeSpan)
            })
        .CaptureStart(); // Start capture using selected DirectShow capture device

// Stop capture
ffmpeg.CaptureStop();
```
