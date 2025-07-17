using System.Collections.Generic;
using Game.CodeBlocks;
using UnityEngine;

namespace Game.UI
{
	public interface IBlock
	{
		IBlock this[int childIndex] => GetChild(childIndex);

		void SetParent(IBlock parent);

		void AddChild(IBlock child);
		IBlock GetChild(int index);
		void RemoveChild(IBlock child);
	}

	public interface ICodeBlock
	{
		CodeBlocks.ICodeBlock GetBlock();
	}

	public interface IFunctionBlock
	{
		IParameterBlock this[int index] { get => GetParameter(index); set => SetParameter(index, value); }

		IParameterBlock GetParameter(int index);
		void SetParameter(int parameterIndex, IParameterBlock variable);
	}

	public interface IParameterBlock
	{
		CodeBlocks.Value GetValue();
	}

	public interface IVariableBlock : IParameterBlock
	{
		string Name { get; }
	}
}
