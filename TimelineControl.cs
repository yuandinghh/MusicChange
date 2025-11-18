using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using MusicChange;

namespace LaserEditing
{
	public class TimelineControl : FlowLayoutPanel             // Timeline
	{
		private LibVLC _libVLC;
		public TimelineControl(LibVLC libVLC)            // 构造函数
		{
			_libVLC = libVLC;
			this.WrapContents = false;
			this.FlowDirection = FlowDirection.LeftToRight;
			this.AutoScroll = true;
			this.Padding = new Padding( 8 );
			this.AllowDrop = true;
			this.DragEnter += TimelineControl_DragEnter;
			this.DragDrop += TimelineControl_DragDrop;
		}

		private void TimelineControl_DragEnter(object sender, DragEventArgs e)          // 拖拽文件进入
		{
			if (e.Data.GetDataPresent( DataFormats.FileDrop ))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void TimelineControl_DragDrop(object sender, DragEventArgs e)         // 拖拽文件释放
		{
			var files = (string[])e.Data.GetData( DataFormats.FileDrop );
			foreach (var f in files)
				AddMediaFile( f );
		}

		public void AddMediaFile(string path)                                  // 添加媒体文件
		{
			if (!File.Exists( path ))
				return;
			var ext = Path.GetExtension( path ).ToLowerInvariant();

			//var item = new MediaItemControl( _libVLC, path )
			//{
			//	Width = 220,
			//	Height = 140,
			//	Margin = new Padding( 6 )
			//};
			//this.Controls.Add( item );
		}
	}
}