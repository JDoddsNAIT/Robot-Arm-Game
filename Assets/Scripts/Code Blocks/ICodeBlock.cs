using System;

namespace Game.CodeBlocks
{
	public interface ICodeBlock
	{
		void Execute();
		ICodeBlock GetNextBlock();
	}
}
