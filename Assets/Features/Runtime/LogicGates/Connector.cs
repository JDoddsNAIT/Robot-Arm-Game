namespace Features.LogicGates
{
	public class Connector : RegulatorSingleton<Connector>
	{
		private GateConnector _selected;

		public void StartConnection(GateConnector connectable)
		{
			if (_selected == null)
				_selected = connectable;
			else
			{
				if (connectable != null)
					GateConnector.ToggleConnection(_selected, connectable);
				_selected = null;
			}
		}
	}
}
