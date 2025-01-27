using NetShaderc;
using OpenTK.Graphics.OpenGL4;
using System.Text;
namespace Alkata
{
	/// <summary>
	/// Alkata is a shader container file format made to quickly expose relevant info
	/// </summary>
	public class Alkata
	{
		static readonly byte[] Magic = [ 0xFF, .. Encoding.UTF8.GetBytes("ALKATA"), 0xFF ];
		Alkata()
		{

		}

		public static Alkata Compile(Shader[] shaders, FileStream fs)
		{
			List<byte> buffer = [.. Magic, .. BitConverter.GetBytes(shaders.Length)];
			
			
		}

		public Alkata FromFile(FileStream fs)
		{
			Span<byte> buffer = new byte[20];
			fs.Read(buffer);
			for (int i = 0; i < Magic.Length; i++)
			{
				if (buffer[i] != Magic[i])
				{
					throw new InvalidDataException("Invalid Alkata file");
				}
			}

			var version = BitConverter.ToInt64(buffer[8..16]);

			if(version != 1)
			{
				throw new InvalidDataException("Invalid Alkata version");
			}

			var shaderCount = BitConverter.ToInt32(buffer[16..20]);
			var shaders = new ShaderDef[shaderCount];
			for (int i = 0; i < shaderCount; i++)
			{
				var buffr = new byte[sizeof(uint)];
				fs.Read(buffr, 0, sizeof(uint));
				shaders[i].Kind = (ShaderKind)BitConverter.ToUInt32(buffr[0..sizeof(uint)]);
				buffr = new byte[sizeof(int)];
				shaders[i].Uniforms = new ShaderDef.Uniform[fs.Read(buffr, 0, sizeof(int))];
				for (int s = 0; s < shaders[i].Uniforms.Length; s++)
				{
					buffr = new byte[sizeof(int)];
					char[] bufs = new char[fs.Read(buffr, 0, sizeof(int))];

					shaders[i].Uniforms[s].Name = bufs.ToString() ?? throw new NullReferenceException();
				}
				buffr = new byte[sizeof(uint)];
				// do position and size

			}

		}

		struct ShaderDef
		{
			public struct Uniform
			{
				public string Name;
			}

			public ShaderKind Kind;
			public Uniform[] Uniforms;
			public uint Position;
			public uint Size;
		}

	}
}
