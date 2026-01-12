using UnityEngine;

namespace Features.LogicGates
{
	public class LogicSimulator : MonoBehaviour
	{
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
			ResumeSimulation();
		}

		public void Update()
		{
			if (!_running || !enabled)
				return;

			foreach (var gate in _gates) gate.SetOutputValues();
			foreach (var gate in _gates) gate.UpdateOutputValues();
		}

		public void ResumeSimulation() => _running = true;
		public void StopSimulation() => _running = false;
	}
}
