using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Timers;

namespace Game
{
	[RequireComponent(typeof(HingeJoint2D))]
	public class ArmJoint : MonoBehaviour
	{
		private HingeJoint2D _joint;

		private readonly List<RotateCommand> _commands = new();
		[SerializeField] private float _testMotorSpeed = 90;


		public float Rotation {
			get {
				TryGetJoint();
				return _joint.jointAngle;
			}
		}

		public void AddCommand(RotateCommand command)
		{
			_commands.Add(command);
			command.OnComplete += t => _commands.Remove(command);
		}

		public void FixedUpdate()
		{
			TryGetJoint();

			if (_commands.Count > 0)
			{
				Debug.Log(Rotation, _joint);
			}

			float totalSpeed = _commands.Select(GetSpeed).Sum();
			_joint.motor = _joint.motor with { motorSpeed = totalSpeed };
		}

		private void TryGetJoint()
		{
			if (_joint == null)
				TryGetComponent(out _joint);
		}

		private static float GetSpeed(RotateCommand command) => (float)(command.Delta / command.Time);

		[ContextMenu(nameof(AddTestCommand))]
		private void AddTestCommand()
		{
			AddCommand(new RotateCommand(_testMotorSpeed, 2));
		}
	}

	public class RotateCommand
	{
		public readonly float Delta, Time;
		private readonly CountdownTimer _timer;

		public event ITimer.TimerEvent OnComplete {
			add => _timer.OnStop += value;
			remove => _timer.OnStop -= value;
		}

		public RotateCommand(float delta, float time)
		{
			Delta = delta;
			Time = time;
			_timer = new(time);

			_timer.Start();
		}
	}
}
