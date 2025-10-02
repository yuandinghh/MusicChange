
//namespace MusicChange
//{
//	partial class VideoSettingsForm
//	{
//		/// <summary>
//		/// Required designer variable.
//		/// </summary>
//		private System.ComponentModel.IContainer components = null;

//		/// <summary>
//		/// Clean up any resources being used.
//		/// </summary>
//		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//		protected override void Dispose(bool disposing)
//		{
//			if(disposing && (components != null))
//			{
//				components.Dispose();
//			}
//			base.Dispose(disposing);
//		}

//		#region Windows Form Designer generated code

//		/// <summary>
//		/// Required method for Designer support - do not modify
//		/// the contents of this method with the code editor.
//		/// </summary>
//		private void InitializeComponent()
//		{
//			this.components = new System.ComponentModel.Container();
//			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//			this.ClientSize = new System.Drawing.Size(800, 450);
//			this.Text = "VideoSettingsForm";
//		}

//		#endregion
//	}
//}

using System.Windows.Forms;

namespace MusicChange
{
  
        partial class VideoSettingsForm
        {
            private System.ComponentModel.IContainer components = null;
            private TrackBar trackBarBrightness;
            private TrackBar trackBarContrast;
            private TrackBar trackBarSaturation;
            private TrackBar trackBarHue;
            private Label labelBrightness;
            private Label labelContrast;
            private Label labelSaturation;
            private Label labelHue;

            protected override void Dispose(bool disposing)
            {
                if(disposing && (components != null))
                    components.Dispose();
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
			this.trackBarBrightness = new System.Windows.Forms.TrackBar();
			this.trackBarContrast = new System.Windows.Forms.TrackBar();
			this.trackBarSaturation = new System.Windows.Forms.TrackBar();
			this.trackBarHue = new System.Windows.Forms.TrackBar();
			this.labelBrightness = new System.Windows.Forms.Label();
			this.labelContrast = new System.Windows.Forms.Label();
			this.labelSaturation = new System.Windows.Forms.Label();
			this.labelHue = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBarBrightness
			// 
			this.trackBarBrightness.Location = new System.Drawing.Point(80, 15);
			this.trackBarBrightness.Maximum = 200;
			this.trackBarBrightness.Name = "trackBarBrightness";
			this.trackBarBrightness.Size = new System.Drawing.Size(200, 56);
			this.trackBarBrightness.TabIndex = 1;
			this.trackBarBrightness.TickFrequency = 10;
			// 
			// trackBarContrast
			// 
			this.trackBarContrast.Location = new System.Drawing.Point(80, 65);
			this.trackBarContrast.Maximum = 200;
			this.trackBarContrast.Name = "trackBarContrast";
			this.trackBarContrast.Size = new System.Drawing.Size(200, 56);
			this.trackBarContrast.TabIndex = 3;
			this.trackBarContrast.TickFrequency = 10;
			// 
			// trackBarSaturation
			// 
			this.trackBarSaturation.Location = new System.Drawing.Point(80, 115);
			this.trackBarSaturation.Maximum = 300;
			this.trackBarSaturation.Name = "trackBarSaturation";
			this.trackBarSaturation.Size = new System.Drawing.Size(200, 56);
			this.trackBarSaturation.TabIndex = 5;
			this.trackBarSaturation.TickFrequency = 10;
			// 
			// trackBarHue
			// 
			this.trackBarHue.Location = new System.Drawing.Point(80, 165);
			this.trackBarHue.Maximum = 180;
			this.trackBarHue.Minimum = -180;
			this.trackBarHue.Name = "trackBarHue";
			this.trackBarHue.Size = new System.Drawing.Size(200, 56);
			this.trackBarHue.TabIndex = 7;
			this.trackBarHue.TickFrequency = 10;
			// 
			// labelBrightness
			// 
			this.labelBrightness.AutoSize = true;
			this.labelBrightness.Location = new System.Drawing.Point(20, 20);
			this.labelBrightness.Name = "labelBrightness";
			this.labelBrightness.Size = new System.Drawing.Size(37, 15);
			this.labelBrightness.TabIndex = 0;
			this.labelBrightness.Text = "亮度";
			// 
			// labelContrast
			// 
			this.labelContrast.AutoSize = true;
			this.labelContrast.Location = new System.Drawing.Point(20, 70);
			this.labelContrast.Name = "labelContrast";
			this.labelContrast.Size = new System.Drawing.Size(52, 15);
			this.labelContrast.TabIndex = 2;
			this.labelContrast.Text = "对比度";
			// 
			// labelSaturation
			// 
			this.labelSaturation.AutoSize = true;
			this.labelSaturation.Location = new System.Drawing.Point(20, 120);
			this.labelSaturation.Name = "labelSaturation";
			this.labelSaturation.Size = new System.Drawing.Size(52, 15);
			this.labelSaturation.TabIndex = 4;
			this.labelSaturation.Text = "饱和度";
			// 
			// labelHue
			// 
			this.labelHue.AutoSize = true;
			this.labelHue.Location = new System.Drawing.Point(20, 170);
			this.labelHue.Name = "labelHue";
			this.labelHue.Size = new System.Drawing.Size(37, 15);
			this.labelHue.TabIndex = 6;
			this.labelHue.Text = "色调";
			// 
			// VideoSettingsForm
			// 
			this.ClientSize = new System.Drawing.Size(320, 220);
			this.Controls.Add(this.labelBrightness);
			this.Controls.Add(this.trackBarBrightness);
			this.Controls.Add(this.labelContrast);
			this.Controls.Add(this.trackBarContrast);
			this.Controls.Add(this.labelSaturation);
			this.Controls.Add(this.trackBarSaturation);
			this.Controls.Add(this.labelHue);
			this.Controls.Add(this.trackBarHue);
			this.Name = "VideoSettingsForm";
			this.Text = "视频参数调节";
			((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarSaturation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarHue)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

            }
        }
    }




//c# 设计 程序 LibVLC调节视频的色彩 色调，亮度，对比度，饱和度 的窗口，主程序调用，将 打开的 LibVLC 参数传进并能修改
