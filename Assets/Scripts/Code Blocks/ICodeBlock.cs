using System;

namespace Game.CodeBlocks
{
	public interface ICodeBlock
	{
		Func<ICodeBlock> GetNext { get; }

		void Execute();
	}
}
