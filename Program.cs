﻿using System;
using System.Collections.Generic;
using System.Linq;
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
			//Application.Run( new Cut() );
			//Application.Run( new Form2() );
			Application.Run( new ChangePictuer() );

		}
	}
}
