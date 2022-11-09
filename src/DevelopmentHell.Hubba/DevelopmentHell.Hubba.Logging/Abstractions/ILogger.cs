using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Logging.Abstractions
{
	public interface ILogger
	{
		public Task<Result> Log(LogLevel level, string userName, string message);
	}
}
