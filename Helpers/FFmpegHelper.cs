using System;
using System.Diagnostics;
using System.IO;

namespace MusicChange
{
    public static class FFmpegHelper
    {
        /// <summary>
        /// 使用 ffmpeg 提取单帧图片。timeSeconds 为秒（小数）。
        /// </summary>
        public static bool ExtractFrame(string ffmpegPath, string inputFile, double timeSeconds, string outFile, int width = 320, int height = 180)
        {
            try
            {
                if (!File.Exists(ffmpegPath)) return false;
                if (!File.Exists(inputFile)) return false;
                var psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-ss {timeSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)} -i \"{inputFile}\" -frames:v 1 -vf \"scale={width}:{height}:force_original_aspect_ratio=decrease,pad={width}:{height}:(ow-iw)/2:(oh-ih)/2\" -q:v 2 -y \"{outFile}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var p = Process.Start(psi);
                if (p == null) return false;
                // 等待但不阻塞太久
                p.WaitForExit(5000);
                // 如果进程还在运行，杀掉
                if (!p.HasExited)
                {
                    try { p.Kill(); } catch { }
                }
                return File.Exists(outFile) && new FileInfo(outFile).Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}