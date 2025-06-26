using System;
using System.Drawing;
using System.Windows.Forms;

public class ToolTipEx : ToolTip {
	public Font TipFont { get; set; } = SystemFonts.DefaultFont;

	public ToolTipEx( ) {
		this.OwnerDraw = true;
		this.Draw += ToolTipEx_Draw;
		this.Popup += ToolTipEx_Popup;
	}

	private void ToolTipEx_Draw(object sender, DrawToolTipEventArgs e) {
		e.DrawBackground();
		e.DrawBorder();
		using (Brush b = new SolidBrush( Color.Black )) {
			e.Graphics.DrawString( e.ToolTipText, TipFont, b, e.Bounds );
		}
	}
	//if (e.AssociatedWindow != null && e.AssociatedWindow.IsHandleCreated) {
	//	using (Graphics g = Graphics.FromHwnd( e.AssociatedWindow.Handle )) {
	//		SizeF size = g.MeasureString( this.GetToolTip( e.AssociatedControl ), TipFont );
	//		e.ToolTipSize = new Size( (int)size.Width + 8, (int)size.Height + 8 );
	//	}
	//}
	//else {
	//	// 兜底：给一个默认大小，避免异常
	//	e.ToolTipSize = new Size( 200, 40 );
	//}
	/*IsHandleCreated 出错通常有以下几种情况：
1.	对象不是 Control 派生类
只有继承自 System.Windows.Forms.Control 的对象才有 IsHandleCreated 属性。如果 e.AssociatedWindow 不是 Control 类型，而是 IWin32Window，则没有 IsHandleCreated 属性，会报错。
2.	解决方法
你可以将 e.AssociatedWindow 强制转换为 Control，并判断类型是否正确：
*/
	private void ToolTipEx_Popup(object sender, PopupEventArgs e) {
		var ctrl = e.AssociatedWindow as Control;
		if (ctrl != null && ctrl.IsHandleCreated) {
			using (Graphics g = Graphics.FromHwnd( ctrl.Handle )) {
				SizeF size = g.MeasureString( this.GetToolTip( e.AssociatedControl ), TipFont );
				e.ToolTipSize = new Size( (int)size.Width + 14, (int)size.Height + 14 );
			}
		}
		else {
			// 兜底：给一个默认大小，避免异常
			e.ToolTipSize = new Size( 200, 40 );
		}
	}
	public void SetFont(Font font) {
		this.TipFont = font;
	}

}

