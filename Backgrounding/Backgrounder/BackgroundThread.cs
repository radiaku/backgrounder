using System;
using System.Threading;
using System.Windows.Forms;

namespace Backgrounder {
	/// <summary>
	/// The background thread where all the heavy lifting takes place.
	/// </summary>
	class BackgroundThread {
		private WindowsFormsSynchronizationContext synchronization;
		private ApplicationContext application;
		private AutoResetEvent ready;
		private Thread thread;

		public WindowsFormsSynchronizationContext Start() {
			// Create and start background thread and
			// wait for it to initialize.
			ready = new AutoResetEvent(false);
			thread = new Thread(ThreadProcedure);
			// When set, framework will kill thread on application shutdown.
			thread.IsBackground = true;
			// Used for setting the thread's name.
			int workingFor = Thread.CurrentThread.ManagedThreadId;
			// For readability; ...
			Coherency.FlushWrites();
			// ... a thread start also constitutes a CEP.
			thread.Start(workingFor);
			// Wait for other thread to finish setting variables.
			ready.WaitOne();
			// Make sure a memory load actually happens below.
			Coherency.InvalidateRead();
			return synchronization;
		}

		public void Stop() {
			// Kill the message pump, which in turn exits the thread.
			//
			// Application uses PostThreadMessage(WM_QUIT) to do this,
			// meaning subsequent BeginInvoke messages are ignored.
			//
			// (That would not be the case with PostQuitMessage(), I
			//  suppose PQM isn't used because it can't cross threads.)
			application.ExitThread();
		}

		private void ThreadProcedure(object data) {
			// Give ourselves a name.
			int workingFor = (int) data;
			thread.Name =
				"Background thread #" + Thread.CurrentThread.ManagedThreadId +
				" working for UI thread #" + workingFor
			;
			// Setup up a Windows.Forms synchronization context.
			synchronization = new WindowsFormsSynchronizationContext();
			// Create an application context.
			application = new ApplicationContext();
			// Guarantee that above is visible from other thread.
			Coherency.FlushWrites();
			// Signal the creator that internal state is stable.
			ready.Set();
			// Start the message pump.
			Application.Run(application);
			// There is probably no exception handler attached to
			// the new thread context, so framework and native
			// window exceptions pass through to the framework.
		}
	}
}
