using System;
using System.Drawing;
using NAudio.Wave;

namespace MusicChange
{
    public static class WaveformRenderer
    {
        /// <summary>
        /// 读取音频并按峰值绘制简单波形（每像素采样块取最大值）。
        /// width/height 指定输出 Bitmap 大小。
        /// </summary>
        public static Bitmap RenderWaveform(string audioFilePath, int width, int height, Color background, Color waveColor)
        {
            try
            {
                using var afr = new AudioFileReader(audioFilePath);
                var format = afr.WaveFormat;
                int channels = format.Channels;
                // 每像素需要读取的样本帧数量（帧包含 channels 个样本）
                long totalFrames = afr.Length / format.BlockAlign;
                int framesPerPixel = Math.Max(1, (int)(totalFrames / width));
                float[] buffer = new float[framesPerPixel * channels];
                Bitmap bmp = new Bitmap(width, height);
                using var g = Graphics.FromImage(bmp);
                g.Clear(background);
                Pen pen = new Pen(waveColor);
                int mid = height / 2;
                for (int x = 0; x < width; x++)
                {
                    int read = afr.Read(buffer, 0, buffer.Length);
                    if (read == 0) break;
                    float max = 0f;
                    // read is samples (floats)
                    for (int i = 0; i < read; i += channels)
                    {
                        float v = Math.Abs(buffer[i]);
                        if (v > max) max = v;
                    }
                    int amp = (int)(max * mid);
                    g.DrawLine(pen, x, mid - amp, x, mid + amp);
                }
                return bmp;
            }
            catch
            {
                // 返回占位图
                var bmp = new Bitmap(width, height);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.DarkGray);
                return bmp;
            }
        }
    }
}