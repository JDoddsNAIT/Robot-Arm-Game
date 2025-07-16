using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.UI
{
	public partial class SnappingPoint
	{
		/// <summary>
		/// Internal database for snapping points.
		/// </summary>
		internal class DB
		{
			private readonly Dictionary<Type, List<SnappingPoint>> _db = new();

			public IEnumerable<SnappingPoint> this[Type type] => GetSnappingPoints(type);

			public void Add(SnappingPoint point)
			{
				if (point._pointType is Type.Disable)
					return;

				foreach (var type in point._pointType.GetFlags())
				{
					GetOrCreateList(type).Add(point);
				}
			}

			public void Remove(SnappingPoint point)
			{
				foreach (var type in point._pointType.GetFlags())
				{
					if (_db.TryGetValue(type, out var points))
					{
						points.Remove(point);
						ReleaseIfEmpty(type, points);
					}
				}
			}

			public IEnumerable<SnappingPoint> GetSnappingPoints(Type flags, IReadOnlyList<SnappingPoint> exclusions = null)
			{
				bool validatePoint(SnappingPoint point) => point != null && point.enabled && (exclusions is null || !exclusions.Contains(point));

				if (flags is Type.Disable)
					yield break;

				if (flags is Type.Any)
				{
					for (int i = 0; i < _db[Type.Any].Count; i++)
					{
						yield return _db[Type.Any][i];
					}
					yield break;
				}

				foreach (var flag in flags.GetFlags())
				{
					if (flag is Type.Any or Type.Disable)
						continue;

					if (!_db.TryGetValue(flag, out var points))
						continue;

					for (int i = 0; i < points.Count; i++)
					{
						SnappingPoint point = points[i];
						if (validatePoint(point))
							yield return point;
					}
				}
			}

			private List<SnappingPoint> GetOrCreateList(Type type)
			{
				if (!_db.TryGetValue(type, out var list))
				{
					list = ListPool<SnappingPoint>.Get();
					_db.Add(type, list);
				}
				return list;
			}

			private void ReleaseIfEmpty(Type type, List<SnappingPoint> list)
			{
				if (list.Count == 0 && _db.Remove(type))
				{
					ListPool<SnappingPoint>.Release(list);
				}
			}
		}
	}
}
