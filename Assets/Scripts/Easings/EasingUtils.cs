using System;
using static UnityEngine.Mathf;

namespace Easings
{
	public delegate float EaseFunc(float t);

	public interface IEasing
	{
		float In(float t);
		float Out(float t);
		float InOut(float t);
	}

	public static class EasingUtils
	{
		// Magic number corner of shame
		internal const float
			C1 = 1.70158f,
			C2 = C1 * 1.525f,
			C3 = C1 + 1,
			C4 = (2 * PI) / 3,
			C5 = (2 * PI) / 4.5f,
			N1 = 7.5625f,
			D1 = 2.75f;

		public static readonly Sine sine = new();
		public static readonly Smooth smooth = new();
		public static readonly Exponential exponential = new();
		public static readonly Circle circle = new();
		public static readonly Back back = new();
		public static readonly Elastic elastic = new();
		public static readonly Bounce bounce = new();

		public static float Ease(float t, EasingType type, EasingMethod method)
		{
			IEasing easing = type switch {
				EasingType.None => throw new ArgumentNullException(nameof(type)),
				EasingType.Sine => sine,
				EasingType.Smooth => smooth,
				EasingType.Exponential => exponential,
				EasingType.Circle => circle,
				EasingType.Back => back,
				EasingType.Elastic => elastic,
				EasingType.Bounce => bounce,
				_ => throw new ArgumentOutOfRangeException(nameof(type), "Given easing type is not valid."),
			};
			
			EaseFunc func = method switch {
				EasingMethod.None => throw new ArgumentNullException(nameof(method)),
				EasingMethod.In => easing.In,
				EasingMethod.Out => easing.Out,
				EasingMethod.InOut => easing.InOut,
				_ => throw new ArgumentOutOfRangeException(nameof(method), "Given easing method is not valid."),
			};

			return func(t);
		}
	}

	public enum EasingType
	{
		None = 0, Sine = 1, Smooth = 2, Exponential = 3, Circle = 4, Back = 5, Elastic = 6, Bounce = 7
	}

	[Flags]
	public enum EasingMethod
	{
		None = 0, In = 1, Out = 2, InOut = In | Out,
	}
}
