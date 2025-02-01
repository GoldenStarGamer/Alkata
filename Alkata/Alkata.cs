using NetShaderc;
using System.Text;
namespace Alkata
{
	/// <summary>
	/// Alkata is a shader container file format made to quickly expose relevant info
	/// </summary>
	public static class Alkata
	{
		static readonly byte[] Magic = [ 0xFF, .. Encoding.UTF8.GetBytes("ALKATA"), 0xFF ];

		public static byte[] Compile(Shader[] shaders)
		{
			List<byte> buffer = [.. Magic, .. BitConverter.GetBytes(1ul), .. BitConverter.GetBytes((uint)shaders.Length)];

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
				for (int s = 0; s < sizeof(ulong); s++)
				{
					var ss = s + positions[i];
					var sss = BitConverter.GetBytes((ulong)buffer.Count)[s];
					buffer[ss] = sss;
				}
				buffer.AddRange(shaders[i].Code);
			}

			buffer.AddRange(BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));

			return [.. buffer];
		}

		public static Shader[] FromFile(string path)
		{
			return FromBytes(File.ReadAllBytes(path));
		}

		public static Shader[] FromBytes(byte[] data)
		{
			var index = 0;
			byte[] Read(int size)
			{
				var buf = data.AsSpan()[index..(index + size)];
				index += size;
				return buf.ToArray();
			}
			Span<byte> buffer = new byte[20];
			buffer = Read(buffer.Length);
			for (int i = 0; i < Magic.Length; i++)
			{
				if (buffer[i] != Magic[i])
				{
					throw new InvalidDataException("Invalid Alkata file");
				}
			}

			var version = BitConverter.ToUInt64(buffer[8..16]);

			if (version != 1)
			{
				throw new InvalidDataException("Invalid Alkata version");
			}

			var shaderCount = BitConverter.ToUInt32(buffer[16..20]);
			var shaderDefs = new ShaderDef[shaderCount];

			for (int i = 0; i < shaderCount; i++)
			{
				buffer = new byte[sizeof(uint)];
				buffer = Read(buffer.Length);
				shaderDefs[i].Kind = (ShaderKind)BitConverter.ToUInt32(buffer);

				buffer = new byte[sizeof(ulong)];
				buffer = Read(buffer.Length);
				shaderDefs[i].Position = BitConverter.ToUInt64(buffer);

				buffer = new byte[sizeof(ulong)];
				buffer = Read(buffer.Length);
				shaderDefs[i].Size = BitConverter.ToUInt64(buffer);
			}

			var shaders = new Shader[shaderCount];
			for (int i = 0; i < shaderCount; i++)
			{
				index = (int)shaderDefs[i].Position;
				buffer = new byte[shaderDefs[i].Size];
				buffer = Read(buffer.Length);
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
