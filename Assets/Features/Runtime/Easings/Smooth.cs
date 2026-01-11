using static UnityEngine.Mathf;

namespace Easings
{
	public class Smooth : IEasing
	{
		public enum Function : ushort { Linear = 1, Quadratic = 2, Cubic = 3, Quartic = 4, Quintic = 5 }

		private readonly ushort _degree;

		public Smooth(Function function = Function.Quadratic) => _degree = (ushort)function;
		public Smooth(ushort degree) => _degree = degree;

		public float In(float t)
		{
			t = Clamp01(t);
			return Pow(t, _degree);
		}

		public float Out(float t)
		{
			t = Clamp01(t);
			return 1 - Pow(1 - t, _degree);
		}

		public float InOut(float t)
		{
			t = Clamp01(t);
			return t < 0.5f
				? Pow(2, _degree - 1) * Pow(t, _degree)
				: 1 - Pow(-2 * t + 2, _degree) / 2;
		}
	}
}
