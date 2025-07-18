using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	public interface IUIBlock
	{
		string Name { get; }

		void Duplicate();
		void Delete();
	}

	public interface IInstruction : IUIBlock, IEquatable<IInstruction>
	{
		Component Component { get; }

		CodeBlocks.ICodeBlock CreateBlock();

		IInstruction GetParent();
		void SetParent(IInstruction newParent);

		void AddChild(IInstruction child);
		void RemoveChild(IInstruction child);

		bool IEquatable<IInstruction>.Equals(IInstruction other)
		{
			return this.Component == other.Component;
		}
	}

	public interface IMethod : IUIBlock, IEnumerable<IParameter>
	{
		int ParameterCount { get; }

		IParameter this[int index] { get => this.GetParameter(index); set => this.SetParameter(index, value); }

		IParameter GetParameter(int index);
		void SetParameter(int index, IParameter value);

		IEnumerator<IParameter> IEnumerable<IParameter>.GetEnumerator()
		{
			for (int i = 0; i < ParameterCount; i++)
			{
				yield return this.GetParameter(i);
			}
		}
	}

	public interface IParameter : IUIBlock
	{
		CodeBlocks.Value GetValue();
	}
}
