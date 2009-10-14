using System;

namespace Backgrounder {
	/// <summary>
	/// A delegate that takes no parameters and produces void result.
	///
	/// Use a lambda expression to create such a delegate, eg.:
	///
	///	helper.Background(() => {
	///		// background code
	///
	///		helper.Foreground(() => {
	///			// report progress
	///		});
	///	});
	/// </summary>
	public delegate void Code();

	/// <summary>
	/// A delegate that takes 1 parameter and produces void result.
	///
	/// Use a lambda expression to create such a delegate, eg.:
	///
	///	helper.Background(() => {
	///		// background code
	///		string i = "pass me";
	///		helper.Foreground<![CDATA[<]]>int<![CDATA[>]]>(i, j => {
	///			// report progress
	///			MessageBox.Show(j);
	///		});
	///	});
	/// </summary>
	public delegate void Code<T>(T parameter);

	/// <summary>
	/// A delegate that takes 2 parameters and produces void result.
	/// </summary>
	public delegate void Code<T1, T2>(T1 parameter1, T2 parameter2);

	/// <summary>
	/// A delegate that takes 3 parameters and produces void result.
	/// </summary>
	public delegate void Code<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3);

	/// <summary>
	/// A delegate that takes 4 parameters and produces void result.
	/// </summary>
	public delegate void Code<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);

	/// <summary>
	/// Delegate for an error handler useful for propagating errors from one thread to another.
	/// </summary>
	public delegate void ErrorHandler(Exception e);
}
