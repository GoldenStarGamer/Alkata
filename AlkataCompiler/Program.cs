namespace AlkataCompiler
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Usage: AlkataCompiler <Alkata file path> <shader file path> <code type (binary or source)> <write mode (overwrite or append)>");
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

			
		}
	}
}
