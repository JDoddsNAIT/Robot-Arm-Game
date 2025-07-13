using Utilities.Serializables;
using UnityEngine;

namespace Game
{
	public class GameManager : Utilities.Singletons.PersistentSingleton<GameManager>
	{
		[field: SerializeField] public int CurrentLevel { get; set; }
		[field: SerializeField] public int CurrentSection { get; set; }

		[SerializeField] private InterfaceReference<IRobotArm> _robotArm;

		public IRobotArm RobotArm { get => _robotArm.Value; }

		public void SetRobotArm(Object value) => _robotArm.UnderlyingValue = value;
	}
}
