﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicChange
{
	public partial class QRCode : Form
	{
		public QRCode( )
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
				this.Close();
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
