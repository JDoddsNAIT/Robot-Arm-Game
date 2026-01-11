using static UnityEngine.Mathf;

namespace Easings
{
	public class Circle : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return 1 - Sqrt(1 - Pow(t, 2));
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t < 0.5f
				? (1 - Sqrt(1 - Pow(2 * t, 2))) / 2
				: (Sqrt(1 - Pow(-2 * t + 2, 2)) + 1) / 2;
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return Sqrt(1 - Pow(t - 1, 2));
		}
	}
}
