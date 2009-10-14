using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Backgrounder {
	/// <summary>
	/// Wrappers around various interesting dotnet intricacies.
	/// </summary>
	public class ContextSetup {
		const string NoSyncContextMessage = "Need an active Windows.Forms context to synchronize with.";
		const string AlreadyDisposedMessage = "Cannot schedule new items into execution flow once background context has been terminated.";
		const string MustPassCodeblockMessage = "A code reference must be given, cannot execute a null reference.";
		const string NullStateMessage = "Invalid null delegate received.";
		const string InvalidStateMessage = "Invalid delegate type received.";

		internal class ScheduledCode {
			internal WindowsFormsSynchronizationContext context;
			internal ErrorHandler handler;
			internal Delegate code;
			internal object[] parameters;
		}

		static SendOrPostCallback invoker = new SendOrPostCallback(Invoke);

		internal static void SetupForegroundContext(ref WindowsFormsSynchronizationContext context) {
			context = SynchronizationContext.Current as WindowsFormsSynchronizationContext;
			if (context == null) throw new ArgumentException(NoSyncContextMessage);
		}

		internal static void SetupBackgroundContext(ref BackgroundThread thread, ref WindowsFormsSynchronizationContext context) {
			thread = new BackgroundThread();
			context = thread.Start();
		}

		internal static void ShutdownBackgroundContext(ref BackgroundThread thread, ref WindowsFormsSynchronizationContext context) {
			if (thread == null) throw new ArgumentNullException("thread");
			thread.Stop();
			// For thread safety, the thread reference is never cleared.
			context = null;
		}

		[DebuggerHidden]
		internal static void FireAndForget(ScheduledCode task) {
			if (task == null) throw new ArgumentNullException("task");
			if (task.code == null) throw new ArgumentNullException("code", MustPassCodeblockMessage);
			if (task.context == null) throw new ObjectDisposedException("context", AlreadyDisposedMessage);
			task.context.Post(invoker, task);
		}

		static void Invoke(object state) {
			if (state == null) throw new ArgumentNullException("state", NullStateMessage);
			ScheduledCode task = state as ScheduledCode;
			if (task == null) throw new ArgumentException("state", InvalidStateMessage);
			try {
				task.code.Method.Invoke(task.code.Target, task.parameters);
			} catch (Exception fault) {
				try {
					if (task.handler == null) throw;
					else task.handler(fault);
				} catch (Exception doubleFault) {
					Application.OnThreadException(doubleFault);
				}
			}
		}
	}
}
