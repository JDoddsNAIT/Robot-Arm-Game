using System;
using System.Collections.Generic;
using Game.CodeBlocks;
using UnityEngine;

namespace Game.UI
{
	public interface IBlock : IEquatable<IBlock>
	{
		MonoBehaviour Behaviour { get; }

		void SetParent(IBlock newParent);

		void AddChild(IBlock child);
		void RemoveChild(IBlock child);

		ICodeBlock CreateBlock();
		IBlock GetNext();

		bool IEquatable<IBlock>.Equals(IBlock other) => this is not null && other is not null && Behaviour == other.Behaviour;
	}

	public interface IFunctionBlock
	{
		IParameterBlock this[int index] { get => GetParameter(index); set => SetParameter(index, value); }

		IParameterBlock GetParameter(int index);
		void SetParameter(int parameterIndex, IParameterBlock variable);
	}

	public interface IParameterBlock
	{
		Value GetValue();
	}

	public interface IVariableBlock : IParameterBlock
	{
		string Name { get; }
	}
}
