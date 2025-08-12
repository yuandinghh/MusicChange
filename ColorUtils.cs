using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MusicChange
{
	
	public static class ColorUtils
	{
		#region 基础方法

		/// <summary>
		/// 将值限制在指定范围内
		/// </summary>
		/// <param name="value">要限制的值</param>
		/// <param name="min">最小值</param>
		/// <param name="max">最大值</param>
		/// <returns>限制后的值</returns>
		public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo( min ) < 0)
				return min;
			if (value.CompareTo( max ) > 0)
				return max;
			return value;
		}

		/// <summary>
		/// 使颜色变暗
		/// </summary>
		/// <param name="color">原始颜色</param>
		/// <param name="factor">变暗因子 (0-1)</param>
		/// <returns>变暗后的颜色</returns>
		public static Color Darken(this Color color, float factor)
		{
			factor = factor.Clamp( 0f, 1f );
			return Color.FromArgb(
				color.A,
				(int)(color.R * (1 - factor)).Clamp( 0, 255 ),
				(int)(color.G * (1 - factor)).Clamp( 0, 255 ),
				(int)(color.B * (1 - factor)).Clamp( 0, 255 ) );
		}

		/// <summary>
		/// 使颜色变亮
		/// </summary>
		/// <param name="color">原始颜色</param>
		/// <param name="factor">变亮因子 (0-1)</param>
		/// <returns>变亮后的颜色</returns>
		public static Color Lighten(this Color color, float factor)
		{
			factor = factor.Clamp( 0f, 1f );
			return Color.FromArgb(
				color.A,
				(int)(color.R + (255 - color.R) * factor).Clamp( 0, 255 ),
				(int)(color.G + (255 - color.G) * factor).Clamp( 0, 255 ),
				(int)(color.B + (255 - color.B) * factor).Clamp( 0, 255 ) );
		}

		#endregion

		#region 高级颜色操作

		/// <summary>
		/// 调整颜色饱和度
		/// </summary>
		/// <param name="color">原始颜色</param>
		/// <param name="saturationFactor">饱和度因子 (0 = 灰度, 1 = 原始, >1 = 过饱和)</param>
		/// <returns>调整后的颜色</returns>
		public static Color AdjustSaturation(this Color color, float saturationFactor)
		{
			saturationFactor = saturationFactor.Clamp( 0f, 2f );

			// 转换为HSL
			var hsl = RgbToHsl( color );

			// 调整饱和度
			hsl.S *= saturationFactor;
			hsl.S = hsl.S.Clamp( 0f, 1f );

			// 转回RGB
			return HslToRgb( hsl );
		}

		/// <summary>
		/// 混合两种颜色
		/// </summary>
		/// <param name="color1">第一种颜色</param>
		/// <param name="color2">第二种颜色</param>
		/// <param name="blendFactor">混合因子 (0 = 全color1, 1 = 全color2)</param>
		/// <returns>混合后的颜色</returns>
		public static Color Blend(this Color color1, Color color2, float blendFactor)
		{
			blendFactor = blendFactor.Clamp( 0f, 1f );

			return Color.FromArgb(
				(int)(color1.A * (1 - blendFactor) + color2.A * blendFactor).Clamp( 0, 255 ),
				(int)(color1.R * (1 - blendFactor) + color2.R * blendFactor).Clamp( 0, 255 ),
				(int)(color1.G * (1 - blendFactor) + color2.G * blendFactor).Clamp( 0, 255 ),
				(int)(color1.B * (1 - blendFactor) + color2.B * blendFactor).Clamp( 0, 255 ) );
		}

		/// <summary>
		/// 创建渐变颜色数组
		/// </summary>
		/// <param name="start">起始颜色</param>
		/// <param name="end">结束颜色</param>
		/// <param name="steps">步数</param>
		/// <returns>渐变颜色数组</returns>
		public static Color[] CreateGradient(Color start, Color end, int steps)
		{
			steps = steps.Clamp( 2, 256 );
			var gradient = new Color[steps];

			for (int i = 0; i < steps; i++) {
				float factor = i / (float)(steps - 1);
				gradient[i] = Blend( start, end, factor );
			}

			return gradient;
		}

		#endregion

		#region HSL颜色模型

		public struct HslColor
		{
			public float H; // 色相 (0-360)
			public float S; // 饱和度 (0-1)
			public float L; // 亮度 (0-1)

			public HslColor(float h, float s, float l)
			{
				H = h.Clamp( 0f, 360f );
				S = s.Clamp( 0f, 1f );
				L = l.Clamp( 0f, 1f );
			}
		}

		/// <summary>
		/// 将RGB颜色转换为HSL颜色
		/// </summary>
		public static HslColor RgbToHsl(Color color)
		{
			float r = color.R / 255f;
			float g = color.G / 255f;
			float b = color.B / 255f;

			float max = Math.Max( r, Math.Max( g, b ) );
			float min = Math.Min( r, Math.Min( g, b ) );
			float delta = max - min;

			float h = 0f;
			float s = 0f;
			float l = (max + min) / 2f;

			if (delta != 0) {
				s = l < 0.5f ? delta / (max + min) : delta / (2 - max - min);

				if (max == r)
					h = (g - b) / delta + (g < b ? 6 : 0);
				else if (max == g)
					h = (b - r) / delta + 2;
				else if (max == b)
					h = (r - g) / delta + 4;

				h *= 60;
			}

			return new HslColor( h, s, l );
		}

		/// <summary>
		/// 将HSL颜色转换为RGB颜色
		/// </summary>
		public static Color HslToRgb(HslColor hsl)
		{
			float h = hsl.H;
			float s = hsl.S;
			float l = hsl.L;

			if (s == 0) {
				// 灰度
				int value = (int)(l * 255);
				return Color.FromArgb( value, value, value );
			}

			float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
			float p = 2 * l - q;

			float hk = h / 360f;
			float[] t = new float[3];
			t[0] = hk + 1f / 3f; // R
			t[1] = hk;           // G
			t[2] = hk - 1f / 3f; // B

			for (int i = 0; i < 3; i++) {
				if (t[i] < 0)
					t[i] += 1f;
				if (t[i] > 1)
					t[i] -= 1f;

				if (t[i] < 1f / 6f)
					t[i] = p + (q - p) * 6 * t[i];
				else if (t[i] < 0.5f)
					t[i] = q;
				else if (t[i] < 2f / 3f)
					t[i] = p + (q - p) * 6 * (2f / 3f - t[i]);
				else
					t[i] = p;
			}

			return Color.FromArgb(
				(int)(t[0] * 255).Clamp( 0, 255 ),
				(int)(t[1] * 255).Clamp( 0, 255 ),
				(int)(t[2] * 255).Clamp( 0, 255 ) );
		}

		#endregion

		#region 对比度计算

		/// <summary>
		/// 计算两种颜色的对比度
		/// </summary>
		/// <returns>对比度比率 (WCAG标准建议最小4.5)</returns>
		public static double CalculateContrast(Color color1, Color color2)
		{
			double lum1 = CalculateLuminance( color1 );
			double lum2 = CalculateLuminance( color2 );

			if (lum1 > lum2)
				return (lum1 + 0.05) / (lum2 + 0.05);
			else
				return (lum2 + 0.05) / (lum1 + 0.05);
		}

		/// <summary>
		/// 计算颜色亮度 (WCAG标准)
		/// </summary>
		public static double CalculateLuminance(Color color)
		{
			double r = color.R / 255.0;
			double g = color.G / 255.0;
			double b = color.B / 255.0;

			r = r <= 0.03928 ? r / 12.92 : Math.Pow( (r + 0.055) / 1.055, 2.4 );
			g = g <= 0.03928 ? g / 12.92 : Math.Pow( (g + 0.055) / 1.055, 2.4 );
			b = b <= 0.03928 ? b / 12.92 : Math.Pow( (b + 0.055) / 1.055, 2.4 );

			return 0.2126 * r + 0.7152 * g + 0.0722 * b;
		}

		/// <summary>
		/// 确保文本颜色在背景上有足够的对比度
		/// </summary>
		/// <returns>调整后的文本颜色</returns>
		public static Color EnsureContrast(Color textColor, Color backgroundColor, double minContrast = 4.5)
		{
			double contrast = CalculateContrast( textColor, backgroundColor );

			if (contrast >= minContrast)
				return textColor;

			// 计算背景亮度
			double bgLuminance = CalculateLuminance( backgroundColor );

			// 根据背景亮度确定最佳文本颜色方向
			bool useLightText = bgLuminance < 0.5;

			// 尝试调整文本颜色
			Color adjustedColor = textColor;
			double currentContrast = contrast;
			int attempts = 0;

			while (currentContrast < minContrast && attempts < 20) {
				if (useLightText) {
					// 向白色方向调整
					adjustedColor = adjustedColor.Lighten( 0.1f );
				}
				else {
					// 向黑色方向调整
					adjustedColor = adjustedColor.Darken( 0.1f );
				}

				currentContrast = CalculateContrast( adjustedColor, backgroundColor );
				attempts++;
			}

			return adjustedColor;
		}

		#endregion

		#region 主题应用工具

		/// <summary>
		/// 应用主题到ToolTipEx控件
		/// </summary>
		public static void ApplyThemeToToolTip(ToolTipEx toolTip, Color primaryColor, bool isDarkMode)
		{
			if (isDarkMode) {
				// 深色模式
				toolTip.BackColor1 = primaryColor.Darken( 0.4f );
				toolTip.BackColor2 = primaryColor.Darken( 0.6f );
				toolTip.BorderColor = primaryColor.Lighten( 0.2f );
				toolTip.ForeColor = Color.WhiteSmoke;
				toolTip.TitleColor = primaryColor.Lighten( 0.4f );
				toolTip.ShadowColor = Color.FromArgb( 30, 0, 0, 0 );
			}
			else {
				// 浅色模式
				toolTip.BackColor1 = primaryColor.Lighten( 0.85f );
				toolTip.BackColor2 = primaryColor.Lighten( 0.75f );
				toolTip.BorderColor = primaryColor.Darken( 0.2f );
				toolTip.ForeColor = primaryColor.Darken( 0.7f );
				toolTip.TitleColor = primaryColor;
				toolTip.ShadowColor = Color.FromArgb( 20, 0, 0, 0 );
			}

			// 确保标题有足够的对比度
			toolTip.TitleColor = EnsureContrast(
				toolTip.TitleColor,
				toolTip.BackColor1,
				4.5
			);
		}

		/// <summary>
		/// 从图像中提取主色调
		/// </summary>
		public static Color ExtractDominantColor(Image image, int sampleSize = 10)
		{
			// 简化实现 - 实际应用中可使用更复杂的算法
			using (var bmp = new Bitmap( image )) {
				// 采样图像中的像素
				var colorCount = new Dictionary<Color, int>();
				int stepX = bmp.Width / sampleSize;
				int stepY = bmp.Height / sampleSize;

				for (int x = 0; x < bmp.Width; x += stepX) {
					for (int y = 0; y < bmp.Height; y += stepY) {
						Color pixel = bmp.GetPixel( x, y );

						// 忽略接近透明或灰度的像素
						if (pixel.A < 50)
							continue;
						if (Math.Abs( pixel.R - pixel.G ) < 20 &&
							Math.Abs( pixel.G - pixel.B ) < 20)
							continue;

						// 量化颜色以减少变化
						Color quantized = Color.FromArgb(
							(pixel.R / 10) * 10,
							(pixel.G / 10) * 10,
							(pixel.B / 10) * 10
						);

						if (colorCount.ContainsKey( quantized ))
							colorCount[quantized]++;
						else
							colorCount[quantized] = 1;
					}
				}

				// 找到最常见的颜色
				return colorCount.OrderByDescending( kv => kv.Value ).FirstOrDefault().Key;
			}
		}

		/// <summary>
		/// 生成协调的调色板
		/// </summary>
		public static Color[] GenerateColorPalette(Color baseColor, int count = 5)
		{
			var palette = new Color[count];
			var hsl = RgbToHsl( baseColor );

			for (int i = 0; i < count; i++) {
				// 变化色相
				float hueShift = (i * 360f / count) % 360;
				float newHue = (hsl.H + hueShift) % 360;

				// 变化饱和度和亮度
				float newSat = (hsl.S * (0.9f + i * 0.05f)).Clamp( 0f, 1f );
				float newLight = (hsl.L * (0.8f + i * 0.05f)).Clamp( 0f, 1f );

				palette[i] = HslToRgb( new HslColor( newHue, newSat, newLight ) );
			}

			return palette;
		}

		#endregion
	}
}
