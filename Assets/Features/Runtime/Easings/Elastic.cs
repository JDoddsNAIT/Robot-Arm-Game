using static UnityEngine.Mathf;
using static Easings.EasingUtils;

namespace Easings
{
	public class Elastic : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return t switch {
				0 => 0,
				1 => 1,
				_ => -Pow(2, 10 * t - 10) * Sin((t * 10 - 10.75f) * C4)
			};
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t switch {
				0 => 0,
				1 => 1,
				< 0.5f => -(Pow(2, 20 * t - 10) * Sin((20 * t - 11.125f) * C5)) / 2,
				_ => (Pow(2, -20 * t + 10) * Sin(20 * t - 11.125f) * C5) / 2 + 1
			};
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return t switch {
				0 => 0,
				1 => 1,
				_ => Pow(2, -10 * t) * Sin((t * 10 - 0.75f) * C4) + 1
			};
		}
	}
}
