using Game.CodeBlocks;
using UnityEngine;

namespace Game.UI
{
	public class TestBlock : BaseInstruction
	{
		public override string Name { get; } = nameof(TestBlock);

		public override ICodeBlock CreateBlock()
		{
			throw new System.NotImplementedException();
		}

		public override void Delete()
		{
			throw new System.NotImplementedException();
		}

		public override void Duplicate()
		{
			throw new System.NotImplementedException();
		}
	}
}
