using static UnityEngine.Mathf;
using static Easings.EasingUtils;

namespace Easings
{
	public class Back : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return C3 * Pow(t, 3) - C1 * Pow(t, 2);
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t < 0.5f
				? (Pow(2 * t, 2) * ((C2 + 1) * 2 * t - C2)) / 2
				: (Pow(2 * t - 2, 2) * ((C2 + 1) * (t * 2 - 2) + C2) + 2) / 2;
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return 1 + C3 * Pow(t - 1, 3) + C1 * Pow(t - 1, 2);
		}
	}
}
