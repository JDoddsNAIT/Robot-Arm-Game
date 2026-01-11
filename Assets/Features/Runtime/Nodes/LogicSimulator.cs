using UnityEngine;

namespace Features.LogicGates
{
	public class LogicSimulator : MonoBehaviour
	{
		private bool _isRunning;
		[SerializeField] private List<BaseLogicGate> _gates = new();

		public void AddToSimulation(BaseLogicGate gate) => _gates.Add(gate);

		public void RemoveFromSimulation(BaseLogicGate gate) => _gates.Remove(gate);

		public void StartSimulation()
		{
			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationStart();
			}
			ResumeSimulation();
		}

		public void Update()
		{
			if (!_isRunning)
				return;

			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationEarlyUpdate();
			}

			for (int i = 0; i < _gates.Count; i++)
			{
				_gates[i].OnSimulationUpdate();
			}
		}

		public void ResumeSimulation() => _isRunning = true;
		public void StopSimulation() => _isRunning = false;
	}
}
