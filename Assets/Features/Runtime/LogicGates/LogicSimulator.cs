using UnityEngine;

namespace Features.LogicGates
{
	public class LogicSimulator : MonoBehaviour
	{
		private bool _started;
		private bool _running;
		[SerializeField] private List<BaseLogicGate> _gates = new();

		public void AddToSimulation(BaseLogicGate gate) => _gates.Add(gate);
		public void RemoveFromSimulation(BaseLogicGate gate) => _gates.Remove(gate);

		public void StartSimulation()
		{
			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationStart();
			}
			_started = true;
			PauseSimulation();
		}

		public void Update()
		{
			if (!_running || !enabled)
				return;

			foreach (var gate in _gates) gate.SetOutputValues();
			foreach (var gate in _gates) gate.OnSimulationUpdate();
		}

		public void PauseSimulation(bool? state = null)
		{
			if (!_started) return;
			_running = state switch {
				null => !_running,
				false => false,
				true => true,
			};
		}

		public void StopSimulation()
		{
			_started = false;
			_running = false;
		}
	}
}
