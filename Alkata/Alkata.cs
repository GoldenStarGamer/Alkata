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
			Span<byte> buffer = new byte[(int)fs.Length];
			fs.Read(buffer);
			for (int i = 0; i < Magic.Length; i++)
			{
				if (buffer[i] != Magic[i])
				{
					throw new InvalidDataException("Invalid Alkata file");
				}
			}

		}

		static string[] GetUniforms()
		{

		}
	}
}
