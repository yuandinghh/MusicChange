using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using Vlc.DotNet.Forms;
using Color = System.Drawing.Color;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using Point = System.Drawing.Point;

namespace MusicChange
{
    public partial class VideoSettingsForm : Form
    {
        private readonly MediaPlayer.LibVLCAudioCleanupCb _mediaPlayer;

        public VideoSettingsForm(MediaPlayer mediaPlayer)
        {
            InitializeComponent();
            MediaPlayer _mediaPlayer = mediaPlayer;
            //mediaPlayer.VideoAdjustments.Contrast = 0.5f;
            //mediaPlayer.VideoAdjustments.Brightness = 0.5f;

            //// 初始化滑块
            //trackBarBrightness.Value = (int)(LaserEditing.mediaPlayer.VideoAdjustments.Brightness * 100);
            //trackBarContrast.Value = (int)(_mediaPlayer.VideoAdjustments.Contrast * 100);
            //trackBarSaturation.Value = (int)(_mediaPlayer.VideoAdjustments.Saturation * 100);
            //trackBarHue.Value = (int)(_mediaPlayer.VideoAdjustments.Hue);

            // 事件绑定
            trackBarBrightness.Scroll += TrackBarBrightness_Scroll;
            trackBarContrast.Scroll += TrackBarContrast_Scroll;
            trackBarSaturation.Scroll += TrackBarSaturation_Scroll;
            trackBarHue.Scroll += TrackBarHue_Scroll;
        }

        private void TrackBarBrightness_Scroll(object sender, EventArgs e)
        {
            //_mediaPlayer.VideoAdjustments.Brightness = trackBarBrightness.Value / 100f;
        }

        private void TrackBarContrast_Scroll(object sender, EventArgs e)
        {
            //_mediaPlayer.VideoAdjustments.Contrast = trackBarContrast.Value / 100f;
        }

        private void TrackBarSaturation_Scroll(object sender, EventArgs e)
        {
            //_mediaPlayer.VideoAdjustments.Saturation = trackBarSaturation.Value / 100f;
        }

        private void TrackBarHue_Scroll(object sender, EventArgs e)
        {
            //_mediaPlayer.VideoAdjustments.Hue = trackBarHue.Value;
        }
    }
}