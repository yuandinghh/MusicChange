//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using LibVLCSharp.Shared;

//namespace MusicChange
//{
//	public partial class AdjustForm : Form
//	{
//		private MediaPlayer _mediaPlayer;
//		private TrackBar _brightnessBar, _contrastBar, _saturationBar;
//		private Label _brightnessLabel, _contrastLabel, _saturationLabel;

//		public AdjustForm(MediaPlayer mediaPlayer)
//		{
//			_mediaPlayer = mediaPlayer;
//			this.Text = "调节亮度/对比度/色饱和度";
//			this.Size = new Size( 350, 250 );
//			this.FormBorderStyle = FormBorderStyle.FixedDialog;
//			this.MaximizeBox = false;

//			_brightnessLabel = new Label { Text = "亮度", Location = new Point( 20, 20 ), Width = 60 };
//			_brightnessBar = new TrackBar { Minimum = 0, Maximum = 200, Value = 100, Location = new Point( 80, 15 ), Width = 200 };
//			_brightnessBar.Scroll += (s, e) => ApplyAdjustments();

//			_contrastLabel = new Label { Text = "对比度", Location = new Point( 20, 70 ), Width = 60 };
//			_contrastBar = new TrackBar { Minimum = 0, Maximum = 200, Value = 100, Location = new Point( 80, 65 ), Width = 200 };
//			_contrastBar.Scroll += (s, e) => ApplyAdjustments();

//			_saturationLabel = new Label { Text = "色饱和度", Location = new Point( 20, 120 ), Width = 60 };
//			_saturationBar = new TrackBar { Minimum = 0, Maximum = 300, Value = 100, Location = new Point( 80, 115 ), Width = 200 };
//			_saturationBar.Scroll += (s, e) => ApplyAdjustments();

//			this.Controls.AddRange( new Control[] {
//				_brightnessLabel, _brightnessBar,
//				_contrastLabel, _contrastBar,
//				_saturationLabel, _saturationBar
//			} );

//			// 初始化时应用一次
//			ApplyAdjustments();
//		}

//		private void ApplyAdjustments( )
//		{
//			if (_mediaPlayer == null)
//				return;
//			// 使用 SetAdjustFloat 方法设置视频参数
//			_mediaPlayer.SetAdjustInt( LibVLCSharp.Shared.VideoAdjustOption.Enable, 1 );
//			_mediaPlayer.SetAdjustFloat( LibVLCSharp.Shared.VideoAdjustOption.Brightness, _brightnessBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustFloat( LibVLCSharp.Shared.VideoAdjustOption.Contrast, _contrastBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustFloat( LibVLCSharp.Shared.VideoAdjustOption.Saturation, _saturationBar.Value / 100.0f );
//		}
//	}
//}

//扩展支持伽马、色调等参数,并且添加 确认 和 重置 存储 按钮

//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using LibVLCSharp.Shared;

//namespace MusicChange
//{
//	public partial class AdjustForm : Form
//	{
//		private MediaPlayer _mediaPlayer;
//		private TrackBar _brightnessBar, _contrastBar, _saturationBar, _gammaBar, _hueBar;
//		private Label _brightnessLabel, _contrastLabel, _saturationLabel, _gammaLabel, _hueLabel;
//		private Button _okButton, _resetButton, _saveButton;

//		// 存储设置（可替换为配置文件持久化）
//		private static float SavedBrightness = 1.0f;
//		private static float SavedContrast = 1.0f;
//		private static float SavedSaturation = 1.0f;
//		private static float SavedGamma = 1.0f;
//		private static int SavedHue = 0;

//		public AdjustForm(MediaPlayer mediaPlayer)
//		{
//			_mediaPlayer = mediaPlayer;
//			this.Text = "调节亮度/对比度/色饱和度/伽马/色调";
//			this.Size = new Size( 370, 370 );
//			this.FormBorderStyle = FormBorderStyle.FixedDialog;
//			this.MaximizeBox = false;

//			_brightnessLabel = new Label { Text = "亮度", Location = new Point( 20, 20 ), Width = 60 };
//			_brightnessBar = new TrackBar { Minimum = 0, Maximum = 200, Value = (int)(SavedBrightness * 100), Location = new Point( 80, 15 ), Width = 200 };
//			_brightnessBar.Scroll += (s, e) => ApplyAdjustments();

//			_contrastLabel = new Label { Text = "对比度", Location = new Point( 20, 70 ), Width = 60 };
//			_contrastBar = new TrackBar { Minimum = 0, Maximum = 200, Value = (int)(SavedContrast * 100), Location = new Point( 80, 65 ), Width = 200 };
//			_contrastBar.Scroll += (s, e) => ApplyAdjustments();

//			_saturationLabel = new Label { Text = "色饱和度", Location = new Point( 20, 120 ), Width = 60 };
//			_saturationBar = new TrackBar { Minimum = 0, Maximum = 300, Value = (int)(SavedSaturation * 100), Location = new Point( 80, 115 ), Width = 200 };
//			_saturationBar.Scroll += (s, e) => ApplyAdjustments();

//			_gammaLabel = new Label { Text = "伽马", Location = new Point( 20, 170 ), Width = 60 };
//			_gammaBar = new TrackBar { Minimum = 10, Maximum = 1000, Value = (int)(SavedGamma * 100), Location = new Point( 80, 165 ), Width = 200 };
//			_gammaBar.Scroll += (s, e) => ApplyAdjustments();

//			_hueLabel = new Label { Text = "色调", Location = new Point( 20, 220 ), Width = 60 };
//			_hueBar = new TrackBar { Minimum = 0, Maximum = 360, Value = SavedHue, Location = new Point( 80, 215 ), Width = 200 };
//			_hueBar.Scroll += (s, e) => ApplyAdjustments();

//			_okButton = new Button { Text = "确认", Location = new Point( 40, 270 ), Size = new Size( 75, 30 ) };
//			_okButton.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

//			_resetButton = new Button { Text = "重置", Location = new Point( 140, 270 ), Size = new Size( 75, 30 ) };
//			_resetButton.Click += (s, e) => ResetAdjustments();

//			_saveButton = new Button { Text = "存储", Location = new Point( 240, 270 ), Size = new Size( 75, 30 ) };
//			_saveButton.Click += (s, e) => SaveAdjustments();

//			this.Controls.AddRange( new Control[] {
//				_brightnessLabel, _brightnessBar,
//				_contrastLabel, _contrastBar,
//				_saturationLabel, _saturationBar,
//				_gammaLabel, _gammaBar,
//				_hueLabel, _hueBar,
//				_okButton, _resetButton, _saveButton
//			} );

//			ApplyAdjustments();
//		}

//		private void ApplyAdjustments( )
//		{
//			if (_mediaPlayer == null)
//				return;
//			_mediaPlayer.SetAdjustInt( VideoAdjustOption.Enable, 1 );
//			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Brightness, _brightnessBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Contrast, _contrastBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Saturation, _saturationBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Gamma, _gammaBar.Value / 100.0f );
//			_mediaPlayer.SetAdjustInt( VideoAdjustOption.Hue, _hueBar.Value );
//		}

//		private void ResetAdjustments( )
//		{
//			_brightnessBar.Value = 100;
//			_contrastBar.Value = 100;
//			_saturationBar.Value = 100;
//			_gammaBar.Value = 100;
//			_hueBar.Value = 0;
//			ApplyAdjustments();
//		}

//		private void SaveAdjustments( )
//		{
//			SavedBrightness = _brightnessBar.Value / 100.0f;
//			SavedContrast = _contrastBar.Value / 100.0f;
//			SavedSaturation = _saturationBar.Value / 100.0f;
//			SavedGamma = _gammaBar.Value / 100.0f;
//			SavedHue = _hueBar.Value;
//			MessageBox.Show( "参数已保存，下次打开窗口自动应用。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
//		}
//	}
//}

using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace MusicChange
{
	public partial class AdjustForm : Form
	{
		private MediaPlayer _mediaPlayer;
		private TrackBar _brightnessBar, _contrastBar, _saturationBar, _gammaBar, _hueBar;
		private Label _brightnessLabel, _contrastLabel, _saturationLabel, _gammaLabel, _hueLabel;
		private Label _brightnessValue, _contrastValue, _saturationValue, _gammaValue, _hueValue;
		private Button _okButton, _resetButton, _saveButton;

		private static float SavedBrightness = 1.0f;
		private static float SavedContrast = 1.0f;
		private static float SavedSaturation = 1.0f;
		private static float SavedGamma = 1.0f;
		private static int SavedHue = 0;

		public AdjustForm(MediaPlayer mediaPlayer)
		{
			_mediaPlayer = mediaPlayer;
			//this.Text = "调节亮度/对比度/色饱和度/伽马/色调";
			this.Text = "色彩调节";
			this.Size = new Size( 400, 370 );
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			//将窗口定位到本程序的中心位置
			this.StartPosition = FormStartPosition.CenterScreen;

			_brightnessLabel = new Label { Text = "亮度", Location = new Point( 20, 20 ), Width = 60 };
			_brightnessBar = new TrackBar { Minimum = 0, Maximum = 200, Value = (int)(SavedBrightness * 100), Location = new Point( 80, 15 ), Width = 200, TickFrequency = 10 };
			_brightnessBar.Scroll += (s, e) => { ApplyAdjustments(); UpdateValueLabels(); };
			_brightnessValue = new Label { Location = new Point( 290, 20 ), Width = 60 };

			_contrastLabel = new Label { Text = "对比度", Location = new Point( 20, 70 ), Width = 60 };
			_contrastBar = new TrackBar { Minimum = 0, Maximum = 200, Value = (int)(SavedContrast * 100), Location = new Point( 80, 65 ), Width = 200, TickFrequency = 10 };
			_contrastBar.Scroll += (s, e) => { ApplyAdjustments(); UpdateValueLabels(); };
			_contrastValue = new Label { Location = new Point( 290, 70 ), Width = 60 };

			_saturationLabel = new Label { Text = "色饱和度", Location = new Point( 20, 120 ), Width = 60 };
			_saturationBar = new TrackBar { Minimum = 0, Maximum = 300, Value = (int)(SavedSaturation * 100), Location = new Point( 80, 115 ), Width = 200, TickFrequency = 15 };
			_saturationBar.Scroll += (s, e) => { ApplyAdjustments(); UpdateValueLabels(); };
			_saturationValue = new Label { Location = new Point( 290, 120 ), Width = 60 };

			_gammaLabel = new Label { Text = "伽马", Location = new Point( 20, 170 ), Width = 60 };
			_gammaBar = new TrackBar { Minimum = 10, Maximum = 1000, Value = (int)(SavedGamma * 100), Location = new Point( 80, 165 ), Width = 200, TickFrequency = 50 };
			_gammaBar.Scroll += (s, e) => { ApplyAdjustments(); UpdateValueLabels(); };
			_gammaValue = new Label { Location = new Point( 290, 170 ), Width = 60 };

			_hueLabel = new Label { Text = "色调", Location = new Point( 20, 220 ), Width = 60 };
			_hueBar = new TrackBar { Minimum = 0, Maximum = 360, Value = SavedHue, Location = new Point( 80, 215 ), Width = 200, TickFrequency = 30 };
			_hueBar.Scroll += (s, e) => { ApplyAdjustments(); UpdateValueLabels(); };
			_hueValue = new Label { Location = new Point( 290, 220 ), Width = 60 };

			_okButton = new Button { Text = "确认", Location = new Point( 40, 270 ), Size = new Size( 75, 30 ) };
			_okButton.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

			_resetButton = new Button { Text = "重置", Location = new Point( 140, 270 ), Size = new Size( 75, 30 ) };
			_resetButton.Click += (s, e) => { ResetAdjustments(); UpdateValueLabels(); };

			_saveButton = new Button { Text = "存储", Location = new Point( 240, 270 ), Size = new Size( 75, 30 ) };
			_saveButton.Click += (s, e) => SaveAdjustments();

			this.Controls.AddRange( new Control[] {
				_brightnessLabel, _brightnessBar, _brightnessValue,
				_contrastLabel, _contrastBar, _contrastValue,
				_saturationLabel, _saturationBar, _saturationValue,
				_gammaLabel, _gammaBar, _gammaValue,
				_hueLabel, _hueBar, _hueValue,
				_okButton, _resetButton, _saveButton
			} );

			ApplyAdjustments();
			UpdateValueLabels();
		}

		private void ApplyAdjustments( )
		{
			if (_mediaPlayer == null)
				return;
			_mediaPlayer.SetAdjustInt( VideoAdjustOption.Enable, 1 );
			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Brightness, _brightnessBar.Value / 100.0f );
			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Contrast, _contrastBar.Value / 100.0f );
			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Saturation, _saturationBar.Value / 100.0f );
			_mediaPlayer.SetAdjustFloat( VideoAdjustOption.Gamma, _gammaBar.Value / 100.0f );
			_mediaPlayer.SetAdjustInt( VideoAdjustOption.Hue, _hueBar.Value );
		}

		private void ResetAdjustments( )
		{
			_brightnessBar.Value = 100;
			_contrastBar.Value = 100;
			_saturationBar.Value = 100;
			_gammaBar.Value = 100;
			_hueBar.Value = 0;
			ApplyAdjustments();
		}

		private void SaveAdjustments( )
		{
			SavedBrightness = _brightnessBar.Value / 100.0f;
			SavedContrast = _contrastBar.Value / 100.0f;
			SavedSaturation = _saturationBar.Value / 100.0f;
			SavedGamma = _gammaBar.Value / 100.0f;
			SavedHue = _hueBar.Value;
			MessageBox.Show( "参数已保存，下次打开窗口自动应用。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void UpdateValueLabels( )
		{
			_brightnessValue.Text = $"{_brightnessBar.Value / 100.0f:F2}";
			_contrastValue.Text = $"{_contrastBar.Value / 100.0f:F2}";
			_saturationValue.Text = $"{_saturationBar.Value / 100.0f:F2}";
			_gammaValue.Text = $"{_gammaBar.Value / 100.0f:F2}";
			_hueValue.Text = $"{_hueBar.Value}";
		}
	}
}