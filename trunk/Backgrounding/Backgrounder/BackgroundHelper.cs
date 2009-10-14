using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Backgrounder {
	/// <summary>
	/// Helper to support a design pattern where code
	/// execution shifts back and forth between a foreground
	/// user interface thread and a background worker thread.
	/// </summary>
	public class BackgroundHelper : ContextSetup, IDisposable {
		private WindowsFormsSynchronizationContext foreground;
		private WindowsFormsSynchronizationContext background;
		private BackgroundThread worker;

		/// <summary>
		/// Creates a BackgroundHelper tied to the current
		/// thread and Windows.Forms context.
		/// </summary>
		public BackgroundHelper() {
			SetupForegroundContext(ref foreground);
			SetupBackgroundContext(ref worker, ref background);
		}

		/// <summary>
		/// Schedule code to be run on the background thread.
		/// </summary>
		[DebuggerHidden]
		public void Background(Code code) {
			SendToBackground(code);
		}

		/// <summary>
		/// Schedule code to be run on the background thread,
		/// copying 1 parameter along onto the execution queue.
		/// </summary>
		[DebuggerHidden]
		public void Background<T>(T parameter, Code<T> code) {
			SendToBackground(code, parameter);
		}

		/// <summary>
		/// Schedule code to be run on the background thread,
		/// copying 2 parameters along onto the execution queue.
		/// </summary>
		[DebuggerHidden]
		public void Background<T1,T2>(T1 p1, T2 p2, Code<T1,T2> code) {
			SendToBackground(code, p1, p2);
		}

		/// <summary>
		/// Schedule code to be run on the background thread,
		/// copying 3 parameters along onto the execution queue.
		/// </summary>
		[DebuggerHidden]
		public void Background<T1,T2,T3>(T1 p1, T2 p2, T3 p3, Code<T1,T2,T3> code) {
			SendToBackground(code, p1, p2, p3);
		}

		/// <summary>
		/// Schedule code to be run on the background thread,
		/// copying 4 parameters along onto the execution queue.
		/// </summary>
		[DebuggerHidden]
		public void Background<T1,T2,T3,T4>(T1 p1, T2 p2, T3 p3, T4 p4, Code<T1,T2,T3,T4> code) {
			SendToBackground(code, p1, p2, p3, p4);
		}

		/// <summary>
		/// Schedule code to be run on the foreground / UI thread.
		/// </summary>
		public void Foreground(Code code) {
			SendToForeground(code);
		}

		/// <summary>
		/// Schedule code to be run on the foreground / UI thread,
		/// copying 1 parameter along onto the execution queue.
		/// </summary>
		public void Foreground<T>(T parameter, Code<T> code) {
			SendToForeground(code, parameter);
		}

		/// <summary>
		/// Schedule code to be run on the foreground / UI thread,
		/// copying 2 parameters along onto the execution queue.
		/// </summary>
		public void Foreground<T1,T2>(T1 p1, T2 p2, Code<T1,T2> code) {
			SendToForeground(code, p1, p2);
		}

		/// <summary>
		/// Schedule code to be run on the foreground / UI thread,
		/// copying 3 parameters along onto the execution queue.
		/// </summary>
		public void Foreground<T1,T2,T3>(T1 p1, T2 p2, T3 p3, Code<T1,T2,T3> code) {
			SendToForeground(code, p1, p2, p3);
		}

		/// <summary>
		/// Schedule code to be run on the foreground / UI thread,
		/// copying 4 parameters along onto the execution queue.
		/// </summary>
		public void Foreground<T1,T2,T3,T4>(T1 p1, T2 p2, T3 p3, T4 p4, Code<T1,T2,T3,T4> code) {
			SendToForeground(code, p1, p2, p3, p4);
		}

		/// <summary>
		/// Signals to stop the message pump and terminate the background
		/// thread as soon as the last currently-scheduled task finishes
		/// executing.  As a side-effect, also tells the garbage collector
		/// that finalization is no longer necessary, thus potentially
		/// shortening system-wide garbage collection pauses by having
		/// resources being disposed of while everything is still in a
		/// running state.
		/// </summary>
		public virtual void Dispose() {
			ShutdownBackgroundContext(ref worker, ref background);
			GC.SuppressFinalize(this);
		}

 		/// <summary>
		/// Disposes of the background helper.  Called by the garbage
		/// collector unless the object was already disposed or killed.
		/// </summary>
		~BackgroundHelper() {
			Dispose();
		}

		/// <summary>
		/// Given a delegate and a variable number of parameters,
		/// schedule the delegate to run on the background thread.
		/// </summary>
		[DebuggerHidden]
		protected void SendToBackground(Delegate code, params object[] parameters) {
			ScheduledCode task = new ScheduledCode();
			task.context = background;
			task.handler = BackgroundErrorHandler;
			task.code = code;
			task.parameters = parameters;
			FireAndForget(task);
		}

		/// <summary>
		/// Given a delegate and a variable number of parameters,
		/// schedule the delegate to run on the foreground thread.
		/// </summary>
		protected void SendToForeground(Delegate code, params object[] parameters) {
			ScheduledCode task = new ScheduledCode();
			task.context = foreground;
			task.handler = null;
			task.code = code;
			task.parameters = parameters;
			FireAndForget(task);
		}

		/// <summary>
		/// Propagate unhandled exceptions from user code
		/// running in the background thread to the foreground
		/// thread's handler.
		/// </summary>
		protected void BackgroundErrorHandler(Exception fault) {
			Foreground(() => {
				Application.OnThreadException(fault);
			});
		}
	}
}
