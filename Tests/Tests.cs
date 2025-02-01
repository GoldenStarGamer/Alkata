using Alkata;
using NetShaderc;

namespace Tests
{
	[TestClass]
	public sealed class Tests
	{
		[TestMethod]
		public void CompileTest()
		{
			Shader vert = Shader.FromFile("vertex.glsl", ShaderKind.VertexShader, Shader.CodeType.Source);
			Shader frag = Shader.FromFile("fragment.glsl", ShaderKind.FragmentShader, Shader.CodeType.Source);
			Shader[] startshaders = [vert, frag];
			var compiled = Alkata.Alkata.Compile(startshaders);
			var endshaders = Alkata.Alkata.FromBytes(compiled);
			for (int i = 0; i < startshaders.Length; i++)
			{
				Assert.AreEqual(startshaders[i].Type, endshaders[i].Type);
				Assert.AreEqual(startshaders[i].Code.Length, endshaders[i].Code.Length);
				for (int ii = 0; ii < startshaders[i].Code.Length; ii++)
				{
					Assert.AreEqual(startshaders[i].Code[ii], endshaders[i].Code[ii]);
				}
				
			}
		}
	}
}
