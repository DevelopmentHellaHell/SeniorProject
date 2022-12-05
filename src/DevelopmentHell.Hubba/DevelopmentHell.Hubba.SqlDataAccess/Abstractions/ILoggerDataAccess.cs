using DevelopmentHell.Hubba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;

namespace DevelopmentHell.Hubba.SqlDataAccess.Abstractions
{
	public interface ILoggerDataAccess
	{
		Task<Result> LogData(LogLevel logLevel, Category category, string userName, string message);
		Task<Result> SelectLogs(List<string> columns, List<Comparator> filters);
	}
}
