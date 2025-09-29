//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace MusicChange
//{
//	public partial class VideoSettingsForm:Form
//	{
//		public VideoSettingsForm()
//		{
//			InitializeComponent();
//		}
//	}
//}


using System;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace MusicChange
{
    public partial class VideoSettingsForm:Form
    {
        private readonly MediaPlayer _mediaPlayer;

        public VideoSettingsForm(MediaPlayer mediaPlayer)
        {
            InitializeComponent();
            _mediaPlayer = mediaPlayer;

            // 初始化滑块
            trackBarBrightness.Value = (int)(_mediaPlayer.VideoAdjustments.Brightness * 100);
            trackBarContrast.Value = (int)(_mediaPlayer.VideoAdjustments.Contrast * 100);
            trackBarSaturation.Value = (int)(_mediaPlayer.VideoAdjustments.Saturation * 100);
            trackBarHue.Value = (int)(_mediaPlayer.VideoAdjustments.Hue);

            // 事件绑定
            trackBarBrightness.Scroll += TrackBarBrightness_Scroll;
            trackBarContrast.Scroll += TrackBarContrast_Scroll;
            trackBarSaturation.Scroll += TrackBarSaturation_Scroll;
            trackBarHue.Scroll += TrackBarHue_Scroll;
        }

        private void TrackBarBrightness_Scroll(object sender, EventArgs e)
        {
            _mediaPlayer.VideoAdjustments.Brightness = trackBarBrightness.Value / 100f;
        }

        private void TrackBarContrast_Scroll(object sender, EventArgs e)
        {
            _mediaPlayer.VideoAdjustments.Contrast = trackBarContrast.Value / 100f;
        }

        private void TrackBarSaturation_Scroll(object sender, EventArgs e)
        {
            _mediaPlayer.VideoAdjustments.Saturation = trackBarSaturation.Value / 100f;
        }

        private void TrackBarHue_Scroll(object sender, EventArgs e)
        {
            _mediaPlayer.VideoAdjustments.Hue = trackBarHue.Value;
        }
    }