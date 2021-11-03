using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using TextBox = System.Windows.Forms.TextBox;
using Point = System.Drawing.Point;
using System.Windows.Forms;
using System.ComponentModel;
using Autodesk.Revit.UI;

namespace Task2
{
	class FormWindow
    {
		public static bool CollectDataInput(string title, out int ret)
		{
			System.Windows.Forms.Form dc = new System.Windows.Forms.Form();
			dc.Text = title;

			dc.HelpButton = dc.MinimizeBox = dc.MaximizeBox = false;
			dc.ShowIcon = dc.ShowInTaskbar = false;
			dc.TopMost = true;

			dc.Height = 100;
			dc.Width = 300;
			dc.MinimumSize = new Size(dc.Width, dc.Height);

			int margin = 5;
			Size size = dc.ClientSize;

			TextBox tb = new TextBox();
			tb.TextAlign = HorizontalAlignment.Right;
			tb.Height = 20;
			tb.Width = size.Width - 2 * margin;
			tb.Location = new Point(margin, margin);
			tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			dc.Controls.Add(tb);

			Button ok = new Button();
			ok.Text = "Ok";
			ok.Click += new EventHandler(ok_Click);
			ok.Height = 23;
			ok.Width = 75;
			ok.Location = new Point(size.Width / 2 - ok.Width / 2, size.Height / 2);
			ok.Anchor = AnchorStyles.Bottom;
			dc.Controls.Add(ok);
			dc.AcceptButton = ok;

			dc.ShowDialog();

			return int.TryParse(tb.Text, out ret);
		}

		private static void ok_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.Form form = (sender as System.Windows.Forms.Control).Parent as System.Windows.Forms.Form;
			form.DialogResult = DialogResult.OK;
			form.Close();
		}
        
    }
}
