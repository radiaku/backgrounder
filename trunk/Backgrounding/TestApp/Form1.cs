using System;
using System.Threading;
using System.Windows.Forms;
using Backgrounder;

namespace TestApp {
	public partial class Form1 : Form {
		private BackgroundHelper helper;

		public Form1() {
			InitializeComponent();
			InitializeBackgroundHelper();
		}

		private void InitializeBackgroundHelper() {
			// Start and stop background thread along with form.
			helper = new BackgroundHelper();
			Disposed += (sender, e) => { helper.Dispose(); };
		}

		private void button1_Click(object sender, EventArgs e) {
			helper.Background(() => {
				for (int i = 0; i <= 100; i++) {
					// Continually report progress to user.
					helper.Foreground<int>(i, j => {
						progressBar1.Value = j;
					});
					// Simulate doing I/O or whatever.
					Thread.Sleep(10);
				}
			});
		}

		private void button2_Click(object sender, EventArgs e) {
			// Kill off the background thread and message pump, just
			// to see how everything behaves when that happens.
			helper.Dispose();
		}
	}
}
