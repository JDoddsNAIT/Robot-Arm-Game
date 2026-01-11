using static UnityEngine.Mathf;
using static Easings.EasingUtils;

namespace Easings
{
	public class Bounce : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return 1 - Out(1 - t);
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t < 0.5f
				? (1 - Out(1 - 2 * t)) / 2
				: (1 + Out(2 * t - 1)) / 2;
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return t switch {
				< 1 / D1 => N1 * Pow(t, 2),
				< 2 / D1 => N1 * (t -= 1.5f / D1) * t + 0.75f,
				< 2.5f / D1 => N1 * (t -= 2.25f / D1) * t + 0.9375f,
				_ => N1 * (t -= 2.625f / D1) * t + 0.984375f,
			};
		}
	}
}
