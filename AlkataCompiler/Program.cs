using Alkata;
using NetShaderc;
using static Alkata.Shader;


namespace AlkataCompiler
{
	internal class Program
	{

		enum Mode
		{
			overwrite,
			append
		}

		static void Main(string[] args)
		{
			if (args.Length != 5)
			{
				Console.WriteLine("Usage: AlkataCompiler <Alkata file path> <shader file path> <code type (binary or source)> <write mode (overwrite or append)> <shader kind>");
				return;
			}

			string aktFilePath = args[0];
			string shadFilePath = args[1];
			string codeType = args[2].ToLower();
			string writeMode = args[3].ToLower();

			if (codeType != "binary" && codeType != "source")
			{
				Console.WriteLine("Invalid code type. Supported types are: binary, source");
				return;
			}

			if (writeMode != "overwrite" && writeMode != "append")
			{
				Console.WriteLine("Invalid write mode. Supported modes are: overwrite, append");
				return;
			}

			ShaderKind type;

			Mode mode = writeMode == "overwrite" ? Mode.overwrite : Mode.append;
			CodeType CodeType = codeType == "binary" ? CodeType.Binary : CodeType.Source;
			try
			{
				type = Enum.Parse<ShaderKind>(args[4]);
			}
			catch (Exception)
			{
				Console.WriteLine("you probably didn't input a shaderkind or wrote a non acceptable one");
				throw;
			}

			try
			{

				Console.WriteLine(FromFile(shadFilePath, type, CodeType));

			}
			catch (CompilationErrorException e)
			{
				Console.WriteLine($"Compilation Error: {e.Message}");
				return;
			}

			switch (mode)
			{
				case Mode.overwrite:
					File.WriteAllBytes(aktFilePath, Alkata.Alkata.Compile([FromFile(shadFilePath, type, CodeType)]));
					break;
				case Mode.append:
					File.WriteAllBytes(aktFilePath, Alkata.Alkata.Compile([.. Alkata.Alkata.FromFile(aktFilePath), FromFile(shadFilePath, type, CodeType)]));
					break;
				default:
					throw new NotSupportedException();
			}

		}
	}
}
