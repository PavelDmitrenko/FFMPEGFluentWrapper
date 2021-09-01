# .NET FFMPEG Fluent Wrapper
### A fluent wrapper to FFMPEG

* Enumerate DirectShow capture devices;
* Capture stream from DirectShow device;
* .NET Standard 2.1 & .NET FrameWork 4.0 DLL;

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
        .Destination(<path_to_destination>)
        .OnEvent(data =>
            {
                /// CurrentLevel: Current audio level (as a percentage of the maximum recorded)
                /// Bitrate: Current bitrate in kb/s
                /// Size: File size in bytes
                /// Duration: Recording duration (TimeSpan)
            })
        .CaptureStart(); // Start capture using selected DirectShow capture device

// Stop capture
ffmpeg.CaptureStop();
```
