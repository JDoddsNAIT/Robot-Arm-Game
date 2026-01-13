namespace Features.LogicGates
{
		[Serializable]
		public struct ConfigOptions
		{
			[SerializeField] private bool _invert;
			[SerializeField] private float _scale;

			public bool Invert { readonly get => _invert; init => _invert = value; }
			public float Scale { readonly get => _scale; init => _scale = value; }

			public ConfigOptions() : this(invert: false, scale: 1f) { }
			public ConfigOptions(bool invert, float scale)
			{
				_invert = invert;
				_scale = scale;
			}

			public readonly float GetValue(float value)
			{
				if (_invert)
					value = value == 0 ? 1 : 0;
				value *= _scale;
				return value;
			}
		}
}
