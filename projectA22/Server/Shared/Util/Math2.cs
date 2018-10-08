namespace Shared.Util
{
	/// <summary>
	/// A few commonly used math-related functions.
	/// </summary>
	public static class Math2
	{
		/// <summary>
		/// Returns min, if val is lower than min, max, if val is greater than max, or simply val.
		/// </summary>
		public static int Clamp(int min, int max, int val)
		{
			if (val < min) return min;
			if (val > max) return max;
			return val;
		}
	}
}