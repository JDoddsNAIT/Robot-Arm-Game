using UnityEngine;

namespace Game
{
	public interface IRobotArm
	{
		RobotHand Hand { get; }
		ArmJoint GetArmJoint(int index);
	}

	public class RobotArm : MonoBehaviour, IRobotArm
	{
		[SerializeField] private ArmJoint[] _joints;
		[SerializeField] private RobotHand _hand;

		public RobotHand Hand => _hand;

		public ArmJoint GetArmJoint(int index)
		{
			return _joints[index];
		}
	}
}
