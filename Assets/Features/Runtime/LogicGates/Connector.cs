namespace Features.LogicGates
{
	public class Connector : RegulatorSingleton<Connector>
	{
		private Connectable _selected;

		public void StartConnection(Connectable connectable)
		{
			if (_selected == null)
				_selected = connectable;
			else
			{
				if (connectable != null)
					Connectable.ToggleConnection(_selected, connectable);
				_selected = null;
			}
		}
	}
}
