using NetShaderc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Alkata
{ 

	public struct Shader
	{

		public string Name;
		public ShaderKind Type;
		public string Code;


		public Shader(FileStream fs, ShaderKind type)
		{
			Type = type;
			Name = Path.GetFileName(fs.Name);	
			Span<byte> buffer = new byte[(int)fs.Length];
			fs.Read(buffer);
			Code = buffer.ToString();

		}

		public static Shader FromFile(string path, ShaderKind type)
		{
			using var file = File.OpenRead(path);
			return new(file, type);
		}

		public static Shader FromString(string code, ShaderKind type, string name = "")
		{
			return new Shader {Code = code, Type = type, Name = name};
		}
	}
}
