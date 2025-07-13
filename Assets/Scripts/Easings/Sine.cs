using static UnityEngine.Mathf;

namespace Easings
{
	public class Sine : IEasing
	{
		public float In(float t)
		{
			t = Clamp01(t);
			return 1 - Cos((t * PI) / 2);
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return -(Cos(t * PI) - 1) / 2;
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return Sin((t * PI) / 2);
		}
	}
}
