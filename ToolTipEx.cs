

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

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
		private int _cornerRadius = 8;
		private int _shadowSize = 8;
		private Color _shadowColor = Color.FromArgb( 30, 0, 0, 0 );
		private Image _icon;
		private int _iconSize = 20;
		private Font _titleFont;
		private string _titleText = "";
		private Color _titleColor = Color.FromArgb( 255, 241, 148 );

		// 内容文本字体和颜色
		private Font _contentFont = SystemFonts.DefaultFont;
		private Color _contentColor = Color.White;

		// 动画属性
		private bool _fadeEffect = true;
		private int _fadeDuration = 300;

		// 布局属性
		private int _padding = 10;
		private int _iconTextSpacing = 8;
		private int _titleContentSpacing = 4;
		private ContentAlignment _iconAlignment = ContentAlignment.TopLeft;

		// 存储每个控件的自定义设置
		private Dictionary<Control, ToolTipSettings> _controlSettings = new Dictionary<Control, ToolTipSettings>();

		#endregion

		#region 内部类 - 存储每个控件的设置
		private class ToolTipSettings
		{
			public string Title { get; set; } = "";
			public string Content { get; set; } = "";
			public Image Icon
			{
				get; set;
			}
			public Font TitleFont
			{
				get; set;
			}
			public Color TitleColor
			{
				get; set;
			}
			public Font ContentFont
			{
				get; set;
			}
			public Color ContentColor
			{
				get; set;
			}
		}
		#endregion

		#region 构造函数
		public ToolTipEx( )
		{
			// 启用OwnerDraw模式
			this.OwnerDraw = true;

			// 设置默认属性
			this.BackColor = Color.Transparent;
			this.IsBalloon = false;
			this.UseAnimation = true;
			this.UseFading = true;

			// 注册事件
			this.Draw += ToolTipEx_Draw;
			this.Popup += ToolTipEx_Popup;
		}
		#endregion

		#region 公共属性

		[Category("外观"), Description("工具提示的边框宽度"), DefaultValue(1)]
		public int BorderWidth { get; set; } = 1;

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

		[Category( "文本" ), Description( "工具提示的标题字体" )]
		public Font TitleFont
		{
			get => _titleFont ?? new Font( SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size + 2, FontStyle.Bold );
			set {
				_titleFont = value;
				UpdateStyles();
			}
		}

		[Category( "文本" ), Description( "工具提示的内容字体" )]
		public Font ContentFont
		{
			get => _contentFont;
			set {
				_contentFont = value;
				UpdateStyles();
			}
		}

		[Category( "文本" ), Description( "工具提示的内容文本颜色" )]
		public Color ContentColor
		{
			get => _contentColor;
			set {
				_contentColor = value;
				UpdateStyles();
			}
		}
		#endregion

		#region 事件处理
		private void ToolTipEx_Popup(object sender, PopupEventArgs e)
		{
			try {
				// 获取控件的自定义设置
				ToolTipSettings settings = GetSettingsForControl( e.AssociatedControl );

				// 计算工具提示大小
				Size size = CalculateSize( settings );
				e.ToolTipSize = new Size( size.Width + _shadowSize * 2, size.Height + _shadowSize * 2 );
			}
			catch (Exception ex) {
				Debug.WriteLine( $"Popup error: {ex.Message}" );
				e.ToolTipSize = new Size( 300, 100 );
			}
		}

		private void ToolTipEx_Draw(object sender, DrawToolTipEventArgs e)
		{
			// 获取控件的自定义设置
			ToolTipSettings settings = GetSettingsForControl( e.AssociatedControl );

			// 绘制工具提示
			DrawToolTip( e, settings );
		}
		#endregion

		#region 核心方法

		private void DrawBorder(Graphics g, Rectangle bounds)
		{
			if(BorderWidth <= 0)
				return;

			using(var path = CreateRoundedRectangle(bounds, _cornerRadius))
			using(var pen = new Pen(_borderColor, BorderWidth)) // 应用自定义宽度
			{
				g.DrawPath(pen, path);
			}
		}

		private void DrawShadow(Graphics g, Rectangle bounds)
		{
			if(_shadowSize <= 0)
				return;

			// 阴影区域需比内容区域大，避免覆盖边框
			Rectangle shadowBounds = new Rectangle(
				bounds.X + BorderWidth,
				bounds.Y + BorderWidth,
				bounds.Width - BorderWidth * 2,
				bounds.Height - BorderWidth * 2
			);

			using(var path = CreateRoundedRectangle(shadowBounds, _cornerRadius + _shadowSize))
			using(var brush = new SolidBrush(_shadowColor))
			{
				g.FillPath(brush, path);
			}
		}

		private void DrawToolTip(DrawToolTipEventArgs e, ToolTipSettings settings)
		{
			// 计算实际内容区域（排除阴影）
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // 启用抗锯齿
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
			DrawContent( e.Graphics, contentRect, settings );
		}

		private void DrawShadow1(Graphics g, Rectangle bounds)
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

		private void DrawBorder1(Graphics g, Rectangle bounds)
		{
			using (var path = CreateRoundedRectangle( bounds, _cornerRadius ))
			using (var pen = new Pen( _borderColor )) {
				g.DrawPath( pen, path );
			}
		}

		private void DrawContent(Graphics g, Rectangle bounds, ToolTipSettings settings)
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
			if (settings.Icon != null) {
				iconRect = DrawIcon( g, ref contentRect, settings.Icon );
			}

			// 绘制标题
			Rectangle? titleRect = null;
			if (!string.IsNullOrWhiteSpace( settings.Title )) {
				titleRect = DrawTitle( g, ref contentRect, settings );
			}

			// 绘制内容文本
			DrawText( g, contentRect, settings );
		}

		private Rectangle? DrawIcon(Graphics g, ref Rectangle contentRect, Image icon)
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
			g.DrawImage( icon, iconRect );

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

			return iconRect;
		}

		private Rectangle? DrawTitle(Graphics g, ref Rectangle contentRect, ToolTipSettings settings)
		{
			Font titleFont = settings.TitleFont ?? TitleFont;
			Color titleColor = settings.TitleColor.A == 0 ? _titleColor : settings.TitleColor;

			using (var brush = new SolidBrush( titleColor )) {
				// 确保最大宽度有效
				int maxTitleWidth = Math.Max( 1, contentRect.Width );

				SizeF titleSize = g.MeasureString( settings.Title, titleFont, maxTitleWidth );
				RectangleF titleRect = new RectangleF(
					contentRect.X,
					contentRect.Y,
					titleSize.Width,
					titleSize.Height
				);

				g.DrawString( settings.Title, titleFont, brush, titleRect );

				// 调整内容区域
				contentRect.Y += (int)Math.Ceiling( titleSize.Height ) + _titleContentSpacing;
				contentRect.Height -= (int)Math.Ceiling( titleSize.Height ) + _titleContentSpacing;

				return new Rectangle( (int)titleRect.X, (int)titleRect.Y, (int)titleRect.Width, (int)titleRect.Height );
			}
		}
		/// <summary>
		/// DrawText 绘写 字符
		/// </summary>
		/// <param name="g"></param>
		/// <param name="contentRect"></param>
		/// <param name="settings"></param>
		private void DrawText(Graphics g, Rectangle contentRect, ToolTipSettings settings)
		{
			if (string.IsNullOrEmpty( settings.Content ))
				return;

			Font contentFont = settings.ContentFont ?? ContentFont;
			Color contentColor = settings.ContentColor.A == 0 ? _contentColor : settings.ContentColor;

			using (var brush = new SolidBrush( contentColor ))
			using (var format = new StringFormat()) {
				format.Alignment = StringAlignment.Near;
				format.LineAlignment = StringAlignment.Near;
				format.Trimming = StringTrimming.EllipsisCharacter;
				format.FormatFlags = StringFormatFlags.LineLimit;

				// 确保宽度有效
				int safeWidth = Math.Max( 1, contentRect.Width );
				int safeHeight = Math.Max( 1, contentRect.Height );

				g.DrawString( settings.Content, contentFont, brush,
							new RectangleF( contentRect.X, contentRect.Y, safeWidth, safeHeight ),
							format );
			}
		}

		private Size CalculateSize(ToolTipSettings settings)
		{
			try {
				// 确保文本不为空
				string contentText = string.IsNullOrEmpty( settings.Content ) ? " " : settings.Content;
				string titleText = string.IsNullOrEmpty( settings.Title ) ? " " : settings.Title;
			

				// 使用临时位图计算文本大小
				using var bmp = new Bitmap(1, 1);
				using var g = Graphics.FromImage(bmp);
				g.TextRenderingHint = TextRenderingHint.AntiAlias;

				int width = 0;
				int height = _padding * 2;
				int maxContentWidth = 400; // 最大宽度限制

				// 确保最大宽度有效
				int safeMaxWidth = Math.Max(10, maxContentWidth - _padding * 2);

				// 添加标题高度
				if(!string.IsNullOrWhiteSpace(titleText))
				{
					Font titleFont = settings.TitleFont ?? TitleFont;
					SizeF titleSize = g.MeasureString(titleText, titleFont, safeMaxWidth);
					width = Math.Max(width, (int)titleSize.Width);
					height += (int)titleSize.Height + _titleContentSpacing;
				}

				// 计算内容文本尺寸
				if(!string.IsNullOrWhiteSpace(contentText))
				{
					Font contentFont = settings.ContentFont ?? ContentFont;
					SizeF textSize = g.MeasureString(contentText, contentFont, safeMaxWidth);
					width = Math.Max(width, (int)textSize.Width);
					height += (int)textSize.Height;
				}

				// 添加图标高度/宽度
				if(settings.Icon != null)
				{
					width += _iconSize + _iconTextSpacing;
					height = Math.Max(height, _iconSize + _padding * 2);
				}

				// 确保最小尺寸
				width = Math.Max(width, 100);
				height = Math.Max(height, 40);
				return new Size(width, height);
			}
			catch (Exception ex) {
				Debug.WriteLine( $"CalculateSize error: {ex.Message}" );
				return new Size( 300, 100 );
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
			foreach (Control ctrl in _controlSettings.Keys) {
				base.SetToolTip( ctrl, _controlSettings[ctrl].Content );
			}
		}

		#endregion

		#region 公共方法
		public void SetToolTip(Control control, string title, string content, Image icon = null)
		{
			SetToolTip( control, title, content, icon, null, Color.Empty, null, Color.Empty );
		}

		public void SetToolTip(Control control, string title, string content, Image icon,
							  Font titleFont, Color titleColor,
							  Font contentFont, Color contentColor)
		{
			if (!_controlSettings.ContainsKey( control )) {
				_controlSettings[control] = new ToolTipSettings();
			}

			var settings = _controlSettings[control];
			settings.Title = title;
			settings.Content = content;
			settings.Icon = icon;
			settings.TitleFont = titleFont;
			settings.TitleColor = titleColor;
			settings.ContentFont = contentFont;
			settings.ContentColor = contentColor;

			base.SetToolTip( control, content );
		}

		private ToolTipSettings GetSettingsForControl(Control control)
		{
			if (control == null)
				return GetDefaultSettings();

			if (_controlSettings.TryGetValue( control, out ToolTipSettings settings )) {
				return settings;
			}

			return GetDefaultSettings();
		}

		private ToolTipSettings GetDefaultSettings( )
		{
			return new ToolTipSettings
			{
				Title = _titleText,
				TitleFont = _titleFont,
				TitleColor = _titleColor,
				ContentFont = _contentFont,
				ContentColor = _contentColor,
				Icon = _icon
			};
		}
		#endregion
	}
}