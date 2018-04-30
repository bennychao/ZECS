using System;

namespace ZECS
{

	[ColorAttribute(0.9f, 0.02f, 0.01f, 0.95f)]
	public interface IZSystem : IZComponent
	{
		void Execute();
	}
}

