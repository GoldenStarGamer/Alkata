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
		public byte[] Code;

		public enum CodeType
		{
			Source,
			Binary
		}

		public Shader(FileStream fs, ShaderKind type, CodeType codeType)
		{
			Type = type;
			Name = Path.GetFileName(fs.Name);	
			Span<byte> buffer = new byte[(int)fs.Length];
			fs.Read(buffer);
			byte[] compile(string code)
			{
				Compiler compiler = new();
				var res = compiler.Compile(code, CompileType.CodeToSPIRV, type);
				if (res.Status != CompilationStatus.Success)
				{
					throw new Exception(res.ErrorMessage);
				}
				return res.Code ?? throw new NullReferenceException();
			}
			Code = codeType switch
			{
				CodeType.Source => compile(buffer.ToString()),
				CodeType.Binary => buffer.ToArray(),
				_ => throw new NotSupportedException(nameof(codeType))
			};


		}

		public static Shader FromFile(string path, ShaderKind type, CodeType codeType)
		{
			using var file = File.OpenRead(path);
			return new(file, type, codeType);
		}

		public static Shader FromBytes(byte[] code, ShaderKind type, string name = "")
		{
			return new Shader {Code = code, Type = type, Name = name};
		}
	}
}
