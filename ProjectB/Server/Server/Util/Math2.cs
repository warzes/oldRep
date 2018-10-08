namespace Server.Util
{
	public static class Math2
	{
		public static int Clamp(int min, int max, int val)
		{
			if (val < min)
				return min;
			if (val > max)
				return max;
			return val;
		}

		public static float Clamp(float min, float max, float val)
		{
			if (val < min)
				return min;
			if (val > max)
				return max;
			return val;
		}

		public static long Clamp(long min, long max, long val)
		{
			if (val < min)
				return min;
			if (val > max)
				return max;
			return val;
		}

		public static bool Between(int val, int min, int max)
		{
			return (val >= min && val <= max);
		}

		/// <summary>
		/// Multiplies initial value with multiplicator, returns either the result or Min/MaxValue if the multiplication caused an overflow.
		/// </summary>
		public static short MultiplyChecked(short initialValue, double multiplicator)
		{
			try
			{
				checked { return (short)(initialValue * multiplicator); }
			}
			catch
			{
				if (initialValue >= 0)
					return short.MaxValue;
				else
					return short.MinValue;
			}
		}
		/// <summary>
		/// Multiplies initial value with multiplicator, returns either the result or Min/MaxValue if the multiplication caused an overflow.
		/// </summary>
		public static int MultiplyChecked(int initialValue, double multiplicator)
		{
			try
			{
				checked { return (int)(initialValue * multiplicator); }
			}
			catch
			{
				if (initialValue >= 0)
					return int.MaxValue;
				else
					return int.MinValue;
			}
		}

		/// <summary>
		/// Multiplies initial value with multiplicator, returns either the result or Min/MaxValue if the multiplication caused an overflow.
		/// </summary>
		public static long MultiplyChecked(long initialValue, double multiplicator)
		{
			try
			{
				checked { return (long)(initialValue * multiplicator); }
			}
			catch
			{
				if (initialValue >= 0)
					return long.MaxValue;
				else
					return long.MinValue;
			}
		}
	}
}