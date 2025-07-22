using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicChange {
	internal static class Program {
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main( ) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			//Application.Run(new Cut());
			Application.Run(new LaserEditing());
			//Application.Run( new Form1() );
			//Application.Run( new ChangePictuer() );
			bool createNew;
			using(Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
			{
				if(createNew)
				{
					Application.Run(new Form1());
				}
				else
				{
					MessageBox.Show("应用程序已经在运行中...");
					Thread.Sleep(1000);
					Environment.Exit(1);
				}
			}

		}
	}
}
