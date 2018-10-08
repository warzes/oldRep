using System;
using System.Threading;

namespace Server.Util
{
	/// <summary>
	/// Thread-safe provider for "Random" instances. Use whenever no custom seed is required.
	/// </summary>
	public static class RandomProvider
	{
		private static readonly Random _seed = new Random();

		private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
		{
			lock (_seed)
				return new Random(_seed.Next());
		});

		/// <summary>
		/// Returns an instance of Random for the calling thread.
		/// </summary>
		public static Random Get()
		{
			return randomWrapper.Value;
		}
	}

	/// <summary>
	/// Extensions for Random.
	/// </summary>
	public static class RandomExtension
	{
		/// <summary>
		/// Returns random long.
		/// </summary>
		public static long NextInt64(this Random rnd)
		{
			return (((long)rnd.Next() << 8 * 4 - 1) + rnd.Next());
		}
	}
}