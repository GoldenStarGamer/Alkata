using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkata
{

	[Serializable]
	public class CompilationErrorException : Exception
	{
		public CompilationErrorException() { }
		public CompilationErrorException(string message) : base(message) { }
		public CompilationErrorException(string message, Exception inner) : base(message, inner) { }
	}
}
