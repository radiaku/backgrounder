using System;

namespace Backgrounder {
	/// <summary>
	/// Methods for anti-optimization, ie. ensuring coherency
	/// on top of caches and instruction reordering.
	///
	/// Mostly here for the symbolic value of source code
	/// readability in callers.  Almost all NUMA systems
	/// have hardware to synchronize caches, and the compiler
	/// and JIT rarely (never?) do instruction reordering AFAIK.
	///
	/// Subject to change when a best-practice develops.
	/// </summary>
	internal class Coherency {
		/// <summary>
		/// Does half the work of ensuring that two threads sees
		/// the same value in a variable.
		///	<list type="bullet">
		///		<item>
		///			<description>
		/// Flush registers and write caches, so that writes
		/// occurring previously in the code stream gets retired
		/// to main memory, ultimately gaining visibility in
		/// other threads (as soon as they flush read caches).
		///			</description>
		///		</item>
		///		<item>
		///			<description>
		/// Prevent any load/store reordering that would cause
		/// writes occurring previously in the code stream to
		/// creep downwards below the point of the write flush.
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		internal static void FlushWrites() {
			// A lock is a "critical execution point"; it prevents
			// "side effects" such as non-volatile stores from
			// creeping further down in the code stream due to
			// compiler or JIT optimization.  To be useful, it
			// should also cause a flush of all CPU write caches.
			// See also §10.10 of ECMA-334 (version 4).
			//
			// In practice, the compiler and JIT probably does not
			// reorder anything.  As far as the CPU goes, x86_{32,64}
			// stores have something near release semantics based on
			// a bunch of hardware that does cache synchronization.
			lock (new object()) { }
		}

		/// <summary>
		/// Does half the work of ensuring that two threads sees
		/// the same value in a variable.
		///	<list type="bullet">
		///		<item>
		///			<description>
		/// Flush registers and read caches, so that reads
		/// occurring previously in the code stream are invalidated.
		///			</description>
		///		</item>
		///		<item>
		///			<description>
		/// Prevent any load/store reordering that would cause
		/// reads occurring later in the code stream to
		/// creep upwards above the point of the read flush.
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		internal static void InvalidateRead() {
			// See comment in FlushWrites().
			lock (new object()) { }
		}
	}
}
