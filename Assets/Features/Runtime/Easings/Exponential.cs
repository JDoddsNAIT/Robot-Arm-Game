using static UnityEngine.Mathf;

namespace Easings
{
	public class Exponential : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return t is 0 ? 0 : Pow(2, 10 * t - 10);
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t switch {
				0 => 0,
				1 => 1,
				< 0.5f => Pow(2, 20 * t - 10) / 2,
				_ => (2 - Pow(2, -20 * t + 10)) / 2
			};
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return t is 1 ? 1 : 1 - Pow(2, -10 * t);
		}
	}
}
