using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//namespace MusicChange
//{
//	internal class ToolTipEx
//	{
//	}
//}

namespace MusicChange
{
	[ToolboxItem( true )]
	public class ToolTipEx : ToolTip
	{
		#region 属性字段

		// 外观属性
		private Color _backColor1 = Color.FromArgb( 45, 45, 48 );
		private Color _backColor2 = Color.FromArgb( 30, 30, 32 );
		private Color _borderColor = Color.FromArgb( 65, 65, 70 );
		private Color _foreColor = Color.White;
		private int _cornerRadius = 8;
		private int _shadowSize = 8;
		private Color _shadowColor = Color.FromArgb( 30, 0, 0, 0 );
		private Image _icon;
		private int _iconSize = 24;
		//private Font _titleFont = new( SystemFonts.DefaultFont.FontFamily, 20, FontStyle.Bold );
		private string _titleText = "";
		private Color _titleColor = Color.FromArgb( 255, 241, 148 );
		Font font3 = new Font( "微软雅黑", 20, FontStyle.Italic | FontStyle.Underline ); // 组合样式

		// 动画属性
		private bool _fadeEffect = true;
		private int _fadeDuration = 300;

		// 布局属性
		private int _padding = 10;
		private int _iconTextSpacing = 8;
		private int _titleContentSpacing = 4;
		private ContentAlignment _iconAlignment = ContentAlignment.TopLeft;
		private int _maxWidth = 400; // 新增最大宽度限制
		private int _maxHeight = 500; // 新增最大高度限制

		#endregion

		#region 公共属性

		[Category( "外观" ), Description( "工具提示的背景起始颜色" )]
		public Color BackColor1
		{
			get => _backColor1;
			set {
				_backColor1 = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的背景结束颜色" )]
		public Color BackColor2
		{
			get => _backColor2;
			set {
				_backColor2 = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的边框颜色" )]
		public Color BorderColor
		{
			get => _borderColor;
			set {
				_borderColor = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的文本颜色" )]
		public Color ForeColor
		{
			get => _foreColor;
			set {
				_foreColor = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的圆角半径" )]
		public int CornerRadius
		{
			get => _cornerRadius;
			set {
				_cornerRadius = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的阴影大小" )]
		public int ShadowSize
		{
			get => _shadowSize;
			set {
				_shadowSize = Math.Max( 0, value );
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的阴影颜色" )]
		public Color ShadowColor
		{
			get => _shadowColor;
			set {
				_shadowColor = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的图标" )]
		public Image Icon
		{
			get => _icon;
			set {
				_icon = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的图标尺寸" )]
		public int IconSize
		{
			get => _iconSize;
			set {
				_iconSize = Math.Max( 16, value );
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的标题字体" )]
		public Font TitleFont
		{
			get => font3 ?? new Font( SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size + 2, FontStyle.Bold );
			set {
				font3 = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的标题文本" )]
		public string TitleText
		{
			get => _titleText;
			set {
				_titleText = value;
				UpdateStyles();
			}
		}

		[Category( "外观" ), Description( "工具提示的标题颜色" )]
		public Color TitleColor
		{
			get => _titleColor;
			set {
				_titleColor = value;
				UpdateStyles();
			}
		}

		[Category( "行为" ), Description( "是否启用淡入淡出效果" )]
		public bool FadeEffect
		{
			get => _fadeEffect;
			set {
				_fadeEffect = value;
			}
		}

		[Category( "行为" ), Description( "淡入淡出动画持续时间(毫秒)" )]
		public int FadeDuration
		{
			get => _fadeDuration;
			set {
				_fadeDuration = Math.Max( 0, value );
			}
		}

		[Category( "布局" ), Description( "工具提示的内边距" )]
		public int Padding
		{
			get => _padding;
			set {
				_padding = Math.Max( 0, value );
				UpdateStyles();
			}
		}

		[Category( "布局" ), Description( "图标和文本之间的间距" )]
		public int IconTextSpacing
		{
			get => _iconTextSpacing;
			set {
				_iconTextSpacing = Math.Max( 0, value );
				UpdateStyles();
			}
		}

		[Category( "布局" ), Description( "标题和内容之间的间距" )]
		public int TitleContentSpacing
		{
			get => _titleContentSpacing;
			set {
				_titleContentSpacing = Math.Max( 0, value );
				UpdateStyles();
			}
		}

		[Category( "布局" ), Description( "图标对齐方式" )]
		public ContentAlignment IconAlignment
		{
			get => _iconAlignment;
			set {
				_iconAlignment = value;
				UpdateStyles();
			}
		}

		[Category( "布局" ), Description( "工具提示的最大宽度" )]
		public int MaxWidth
		{
			get => _maxWidth;
			set {
				_maxWidth = Math.Max( 100, value );
				UpdateStyles();
			}
		}

		[Category( "布局" ), Description( "工具提示的最大高度" )]
		public int MaxHeight
		{
			get => _maxHeight;
			set {
				_maxHeight = Math.Max( 50, value );
				UpdateStyles();
			}
		}

		public Font Font
		{
			get;
			private set;
		}

		#endregion

		#region 构造函数

		public ToolTipEx( )
		{
			// 启用OwnerDraw模式
			this.OwnerDraw = true;

			// 设置默认属性
			this.BackColor = Color.Transparent;
			this.ForeColor = Color.White;
			this.IsBalloon = false;
			this.UseAnimation = true;
			this.UseFading = true;

			// 注册事件
			this.Draw += ToolTipEx_Draw;
			this.Popup += ToolTipEx_Popup;
		}

		#endregion

		#region 事件处理

		private void ToolTipEx_Popup(object sender, PopupEventArgs e)
		{
			// 计算工具提示大小
			Size size = CalculateSize( GetToolTip( e.AssociatedControl ) );
			e.ToolTipSize = new Size(
				Math.Min( size.Width + _shadowSize * 2, _maxWidth + _shadowSize * 2 ),
				Math.Min( size.Height + _shadowSize * 2, _maxHeight + _shadowSize * 2 ) );
		}

		private void ToolTipEx_Draw(object sender, DrawToolTipEventArgs e)
		{
			// 绘制工具提示
			DrawToolTip( e );
		}

		#endregion

		#region 核心方法

		private void DrawToolTip(DrawToolTipEventArgs e)
		{
			// 计算实际内容区域（排除阴影）
			Rectangle contentRect = new Rectangle(
				e.Bounds.X + _shadowSize,
				e.Bounds.Y + _shadowSize,
				e.Bounds.Width - _shadowSize * 2,
				e.Bounds.Height - _shadowSize * 2
			);

			// 绘制阴影
			DrawShadow( e.Graphics, e.Bounds );

			// 绘制背景
			DrawBackground( e.Graphics, contentRect );

			// 绘制边框
			DrawBorder( e.Graphics, contentRect );

			// 绘制内容
			DrawContent( e.Graphics, contentRect, e.ToolTipText );
		}

		private void DrawShadow(Graphics g, Rectangle bounds)
		{
			if (_shadowSize <= 0)
				return;

			using (var path = CreateRoundedRectangle( bounds, _cornerRadius + _shadowSize ))
			using (var brush = new SolidBrush( _shadowColor )) {
				g.FillPath( brush, path );
			}
		}

		private void DrawBackground(Graphics g, Rectangle bounds)
		{
			using (var path = CreateRoundedRectangle( bounds, _cornerRadius ))
			using (var brush = new LinearGradientBrush(
				bounds,
				_backColor1,
				_backColor2,
				LinearGradientMode.Vertical )) {
				g.FillPath( brush, path );
			}
		}

		private void DrawBorder(Graphics g, Rectangle bounds)
		{
			using (var path = CreateRoundedRectangle( bounds, _cornerRadius ))
			using (var pen = new Pen( _borderColor )) {
				g.DrawPath( pen, path );
			}
		}

		private void DrawContent(Graphics g, Rectangle bounds, string text)
		{
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			// 计算内容区域（减去内边距）
			Rectangle contentRect = new Rectangle(
				bounds.X + _padding,
				bounds.Y + _padding,
				bounds.Width - _padding * 2,
				bounds.Height - _padding * 2
			);

			// 绘制图标
			Rectangle? iconRect = null;
			if (_icon != null) {
				iconRect = DrawIcon( g, ref contentRect );
			}

			// 绘制标题
			Rectangle? titleRect = null;
			if (!string.IsNullOrWhiteSpace( _titleText )) {
				titleRect = DrawTitle( g, ref contentRect, iconRect );
			}

			// 绘制内容文本 - 确保在可用空间内绘制
			DrawText( g, contentRect, text, titleRect, iconRect );
		}

		private Rectangle? DrawIcon(Graphics g, ref Rectangle contentRect)
		{
			int iconWidth = _iconSize;
			int iconHeight = _iconSize;

			Rectangle iconRect = new Rectangle(
				contentRect.X,
				contentRect.Y,
				iconWidth,
				iconHeight
			);

			// 根据对齐方式调整位置
			if (_iconAlignment == ContentAlignment.TopRight ||
				_iconAlignment == ContentAlignment.MiddleRight ||
				_iconAlignment == ContentAlignment.BottomRight) {
				iconRect.X = contentRect.Right - iconWidth;
			}
			else if (_iconAlignment == ContentAlignment.TopCenter ||
					 _iconAlignment == ContentAlignment.MiddleCenter ||
					 _iconAlignment == ContentAlignment.BottomCenter) {
				iconRect.X = contentRect.X + (contentRect.Width - iconWidth) / 2;
			}

			if (_iconAlignment == ContentAlignment.BottomLeft ||
				_iconAlignment == ContentAlignment.BottomCenter ||
				_iconAlignment == ContentAlignment.BottomRight) {
				iconRect.Y = contentRect.Bottom - iconHeight;
			}
			else if (_iconAlignment == ContentAlignment.MiddleLeft ||
					 _iconAlignment == ContentAlignment.MiddleCenter ||
					 _iconAlignment == ContentAlignment.MiddleRight) {
				iconRect.Y = contentRect.Y + (contentRect.Height - iconHeight) / 2;
			}

			// 绘制图标
			g.DrawImage( _icon, iconRect );

			// 调整内容区域
			if (_iconAlignment == ContentAlignment.TopLeft ||
				_iconAlignment == ContentAlignment.MiddleLeft ||
				_iconAlignment == ContentAlignment.BottomLeft) {
				contentRect.X += iconWidth + _iconTextSpacing;
				contentRect.Width -= iconWidth + _iconTextSpacing;
			}
			else if (_iconAlignment == ContentAlignment.TopRight ||
					 _iconAlignment == ContentAlignment.MiddleRight ||
					 _iconAlignment == ContentAlignment.BottomRight) {
				contentRect.Width -= iconWidth + _iconTextSpacing;
			}
			else if (_iconAlignment == ContentAlignment.TopCenter ||
					 _iconAlignment == ContentAlignment.MiddleCenter ||
					 _iconAlignment == ContentAlignment.BottomCenter) {
				// 顶部/中间/底部居中不影响内容区域宽度
			}

			return iconRect;
		}

		private Rectangle? DrawTitle(Graphics g, ref Rectangle contentRect, Rectangle? iconRect)
		{
			using (var titleFont = TitleFont)
			using (var brush = new SolidBrush( _titleColor )) {
				// 计算标题所需高度
				int maxTitleWidth = contentRect.Width;

				SizeF titleSize = g.MeasureString( _titleText, titleFont, maxTitleWidth );

				// 确保标题不会超出可用空间
				int titleHeight = (int)Math.Ceiling( titleSize.Height );
				if (titleHeight > contentRect.Height - _titleContentSpacing) {
					titleHeight = contentRect.Height - _titleContentSpacing;
				}

				Rectangle titleRect = new Rectangle(
					contentRect.X,
					contentRect.Y,
					maxTitleWidth,
					titleHeight
				);

				// 绘制标题文本 - 支持多行
				g.DrawString( _titleText, titleFont, brush,
							new RectangleF( titleRect.X, titleRect.Y, maxTitleWidth, titleHeight ) );

				// 调整内容区域
				contentRect.Y += titleRect.Height + _titleContentSpacing;
				contentRect.Height -= titleRect.Height + _titleContentSpacing;

				return titleRect;
			}
		}

		private void DrawText(Graphics g, Rectangle contentRect, string text, Rectangle? titleRect, Rectangle? iconRect)
		{
			using (var brush = new SolidBrush( _foreColor )) {
				// 使用DrawString而不是TextRenderer，以便正确处理换行
				using (StringFormat format = new StringFormat()) {
					format.Alignment = StringAlignment.Near;
					format.LineAlignment = StringAlignment.Near;
					format.Trimming = StringTrimming.EllipsisCharacter;
					format.FormatFlags = StringFormatFlags.LineLimit;

					// 确保文本不会超出可用空间
					int maxTextHeight = contentRect.Height;
					
					if (maxTextHeight > 0) {
						g.DrawString( text, font3, brush,   //font3
									new RectangleF( contentRect.X, contentRect.Y,
												  contentRect.Width, maxTextHeight ),
									format );
					}
				}
			}
		}

		private Size CalculateSize(string text)
		{
			// 使用临时位图计算文本大小
			using (var bmp = new Bitmap( 1, 1 ))
			using (var g = Graphics.FromImage( bmp )) {
				g.TextRenderingHint = TextRenderingHint.AntiAlias;

				int width = 0;
				int height = _padding * 2;
				int maxContentWidth = Math.Min( _maxWidth - _padding * 2, 500 );

				// 计算图标尺寸
				int iconWidth = 0;
				int iconHeight = 0;
				if (_icon != null) {
					iconWidth = _iconSize + _iconTextSpacing;
					iconHeight = _iconSize;
				}

				// 计算标题尺寸
				int titleHeight = 0;
				if (!string.IsNullOrWhiteSpace( _titleText )) {
					using var titleFont = TitleFont;
					SizeF titleSize = g.MeasureString( _titleText, titleFont, maxContentWidth );
					width = Math.Max( width, (int)titleSize.Width );
					titleHeight = (int)Math.Ceiling( titleSize.Height );
					height += titleHeight + _titleContentSpacing;
				}

				// 计算内容文本尺寸
				if (!string.IsNullOrWhiteSpace( text )) {
					Font font3 = new Font( "微软雅黑", 20, FontStyle.Italic | FontStyle.Underline ); // 组合样式
					SizeF textSize = g.MeasureString( text, font3, maxContentWidth );  //this.Font    font3
					width = Math.Max( width, (int)textSize.Width );
					height += (int)Math.Ceiling( textSize.Height );
				}

				// 考虑图标尺寸
				if (_icon != null) {
					// 如果图标在左侧或右侧，需要增加宽度
					if (_iconAlignment == ContentAlignment.TopLeft ||
						_iconAlignment == ContentAlignment.MiddleLeft ||
						_iconAlignment == ContentAlignment.BottomLeft ||
						_iconAlignment == ContentAlignment.TopRight ||
						_iconAlignment == ContentAlignment.MiddleRight ||
						_iconAlignment == ContentAlignment.BottomRight) {
						width += iconWidth;
					}

					// 如果图标在顶部或底部，需要增加高度
					if (_iconAlignment == ContentAlignment.TopCenter ||
						_iconAlignment == ContentAlignment.TopLeft ||
						_iconAlignment == ContentAlignment.TopRight ||
						_iconAlignment == ContentAlignment.BottomCenter ||
						_iconAlignment == ContentAlignment.BottomLeft ||
						_iconAlignment == ContentAlignment.BottomRight) {
						height = Math.Max( height, iconHeight + _padding * 2 + titleHeight + _titleContentSpacing );
					}
					else // 中间对齐
					{
						height = Math.Max( height, iconHeight + _padding * 2 );
					}
				}

				// 确保最小尺寸
				width = Math.Min( _maxWidth, Math.Max( width + _padding * 2, 100 ) );
				height = Math.Min( _maxHeight, Math.Max( height, 40 ) );

				return new Size( width, height );
			}
		}

		private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
		{
			GraphicsPath path = new GraphicsPath();

			if (radius <= 0) {
				path.AddRectangle( bounds );
				return path;
			}

			int diameter = radius * 2;
			Rectangle arcRect = new Rectangle( bounds.Location, new Size( diameter, diameter ) );

			// 左上角
			path.AddArc( arcRect, 180, 90 );

			// 右上角
			arcRect.X = bounds.Right - diameter;
			path.AddArc( arcRect, 270, 90 );

			// 右下角
			arcRect.Y = bounds.Bottom - diameter;
			path.AddArc( arcRect, 0, 90 );

			// 左下角
			arcRect.X = bounds.Left;
			path.AddArc( arcRect, 90, 90 );

			path.CloseFigure();
			return path;
		}

		private void UpdateStyles( )
		{
			// 强制重绘所有工具提示
			foreach (Control ctrl in this.GetAllToolTipControls()) {
				this.SetToolTip( ctrl, this.GetToolTip( ctrl ) );
			}
		}

		private Control[] GetAllToolTipControls( )
		{
			// 获取所有设置了工具提示的控件
			// 注意：实际实现中需要更健壮的方法
			return new Control[0]; // 简化实现
		}

		#endregion

		#region 公共方法

		public void SetToolTip(Control control, string title, string text, Image icon = null)
		{
			_titleText = title;
			_icon = icon;
			base.SetToolTip( control, text );
		}

		#endregion
	}
}