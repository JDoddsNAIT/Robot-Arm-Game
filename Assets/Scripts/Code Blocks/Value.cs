using UnityEngine;

namespace Game.CodeBlocks
{
	public record Value
	{
		private string _underlyingValue = string.Empty;

		public Value(string value) => SetValue(value);
		public Value(int value) => SetValue(value);
		public Value(float value) => SetValue(value);
		public Value(bool value) => SetValue(value);

		public string GetStringValue() => _underlyingValue;

		public int GetIntValue() => string.IsNullOrWhiteSpace(_underlyingValue)
			? default : int.Parse(_underlyingValue);

		public float GetFloatValue() => string.IsNullOrEmpty(_underlyingValue)
			? default : float.Parse(_underlyingValue);

		public bool GetBoolValue() => ToBool(this.GetIntValue());

		public void SetValue(string value) => _underlyingValue = value;
		public void SetValue(int value) => _underlyingValue = value.ToString();
		public void SetValue(float value) => _underlyingValue = value.ToString();
		public void SetValue(bool value) => _underlyingValue = ToInt(value).ToString();

		public bool CompareValue(Value other)
		{
			return _underlyingValue == other._underlyingValue;
		}

		public static int ToInt(bool value) => value ? 1 : 0;
		public static bool ToBool(int value) => value != 0;

		public static implicit operator Value(string value) => new(value);
		public static implicit operator Value(int value) => new(value);
		public static implicit operator Value(float value) => new(value);
		public static implicit operator Value(bool value) => new(value);

		public static explicit operator string(Value value) => value.GetStringValue();
		public static explicit operator int(Value value) => value.GetIntValue();
		public static explicit operator float(Value value) => value.GetFloatValue();
		public static explicit operator bool(Value value) => value.GetBoolValue();

		public static bool operator true(Value value) => value.GetBoolValue();
		public static bool operator false(Value value) => !value.GetBoolValue();
	}
}
