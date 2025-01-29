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

		public static byte[] Compile(Shader[] shaders)
		{
			List<byte> buffer = [.. Magic, 1, .. BitConverter.GetBytes(shaders.Length)];

			List<int> positions = [];

			foreach (var shader in shaders)
			{
				buffer.AddRange(BitConverter.GetBytes((uint)shader.Type));
				positions.Add(buffer.Count);
				buffer.AddRange(BitConverter.GetBytes(ulong.MinValue));
				buffer.AddRange(BitConverter.GetBytes((ulong)shader.Code.Length));
			}

			for (int i = 0; i < shaders.Length; i++)
			{
				for (int s = positions[i]; i < positions[i] + sizeof(ulong); i++)
				{
					buffer[s] = BitConverter.GetBytes((ulong)buffer.Count)[s];
				}
				buffer.AddRange(shaders[i].Code);
			}

			return [.. buffer];
		}

		public static Shader[] FromFile(FileStream fs)
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

			var version = BitConverter.ToUInt64(buffer[8..16]);

			if(version != 1)
			{
				throw new InvalidDataException("Invalid Alkata version");
			}

			var shaderCount = BitConverter.ToUInt32(buffer[16..20]);
			var shaderDefs = new ShaderDef[shaderCount];

			for (int i = 0; i < shaderCount; i++)
			{
				buffer = new byte[sizeof(uint)];
				fs.Read(buffer);
				shaderDefs[i].Kind = (ShaderKind)BitConverter.ToUInt32(buffer);

				buffer = new byte[sizeof(ulong)];
				fs.Read(buffer);
				shaderDefs[i].Position = BitConverter.ToUInt64(buffer);

				buffer = new byte[sizeof(ulong)];
				fs.Read(buffer);
				shaderDefs[i].Size = BitConverter.ToUInt64(buffer);
			}

			var shaders = new Shader[shaderCount];
			for (int i = 0; i < shaderCount; i++)
			{
				fs.Seek((long)shaderDefs[i].Position, SeekOrigin.Begin);
				buffer = new byte[shaderDefs[i].Size];
				fs.Read(buffer);
				shaders[i] = new Shader { Code = buffer.ToArray(), Type = shaderDefs[i].Kind };
			}

			return shaders;
		}

		struct ShaderDef
		{
			public ShaderKind Kind;
			public ulong Position;
			public ulong Size;
		}
	}
}
