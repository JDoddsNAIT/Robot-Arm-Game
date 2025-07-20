using System;
using System.Collections.Generic;

namespace Game.UI
{
	public partial class SnappingPoint
	{
		[Flags]
		public enum Type : sbyte
		{
			Any = -0b1,
			Disabled = 0b0,
			BlockTop = 0b1,
			BlockBottom = 0b10,
			ParamSlot = 0b100,
			Param = 0b1000,
		}

		private static readonly Registry _registry = new();

		public class Registry
		{
			private readonly Dictionary<Type, List<SnappingPoint>> _dict;
			private readonly Type[] _enumValues = { Type.BlockTop, Type.BlockBottom, Type.ParamSlot, Type.Param };

			public Registry()
			{
				_dict = new();

				for (int i = 0; i < _enumValues.Length; i++)
				{
					if (_enumValues[i] is not Type.Any or Type.Disabled)
						_dict.Add(_enumValues[i], new List<SnappingPoint>());
				}
			}

			public IEnumerable<SnappingPoint> this[Type type, params SnappingPoint[] exclusions] => GetSnappingPoints(type, exclusions);

			public IEnumerable<SnappingPoint> GetSnappingPoints(Type ofType, IReadOnlyList<SnappingPoint> exclusions = null)
			{
				exclusions ??= Array.Empty<SnappingPoint>();

				for (int i = 0; i < _enumValues.Length; i++)
				{
					var value = _enumValues[i];

					if (!ofType.HasFlag(value))
						continue;

					foreach (SnappingPoint point in _dict[value])
					{
						if (ValidatePoint(point, _enumValues[0..(i - 1)], exclusions))
						{
							yield return point;
						}
					}
				}
			}

			private static bool ValidatePoint(SnappingPoint point, IReadOnlyList<Type> excludedTypes, IReadOnlyList<SnappingPoint> exclusions)
			{
				if (point == null)
					return false;

				for (int i = 0; i < excludedTypes.Count; i++)
				{
					Type t = excludedTypes[i];
					if (point._type.HasFlag(t))
						return false;
				}

				for (int i = 0; i < exclusions.Count; i++)
				{
					SnappingPoint p = exclusions[i];
					if (p == point)
						return false;
				}

				return true;
			}

			public void Register(SnappingPoint point)
			{
				if (point._type is Type.Disabled)
					return;

				for (int i = 0; i < _enumValues.Length; i++)
				{
					if (!_dict.ContainsKey(_enumValues[i]))
						throw new Exception("Failed to register point: Point Registry was not initialized properly.");

					if (point._type.HasFlag(_enumValues[i]) && !_dict[_enumValues[i]].Contains(point))
					{
						_dict[_enumValues[i]].Add(point);
					}
				}
			}

			public void Deregister(SnappingPoint point)
			{
				if (point._type is Type.Disabled)
					return;

				for (int i = 0; i < _enumValues.Length; i++)
				{
					if (!_dict.ContainsKey(_enumValues[i]))
						throw new Exception("Failed to deregister point: Point Registry was not initialized properly.");

					if (point._type.HasFlag(_enumValues[i]) && _dict[_enumValues[i]].Contains(point))
					{
						_dict[_enumValues[i]].Remove(point);
					}
				}
			}
		}//eoc
	}//eoc
}
