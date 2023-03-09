namespace DevelopmentHell.Hubba.SqlDataAccess.Implementations
{
	public struct Comparator
	{
		public Comparator(object key, string operation, object value)
		{
			Key = key;
			Op = operation;
			Value = value;
		}

		public object Key { get; }
		public string Op { get; }
		public object Value { get; }
	}

	public struct Joiner
	{
		public Joiner(string table1, string table2, string column1, string column2)
		{
			T1 = table1;
			T2 = table2;
			C1 = column1;
			C2 = column2;
		}

		public string T1 { get; }
		public string T2 { get; }
		public string C1 { get; }
		public string C2 { get; }

	}
	internal class SQLManip
	{
		public static string InnerJoinTables(List<Joiner> tableColumns)
		{
			string output = String.Concat(Enumerable.Repeat("(", tableColumns.Count));
			foreach (Joiner tableColumn in tableColumns)
			{
				output += String.Format("\n{0} INNER JOIN {1} ON {0}.{2} = {1}.{3})", tableColumn.T1, tableColumn.T2, tableColumn.C1, tableColumn.C2);
			}
			return output;
		}
		public static string InnerJoinTables(Joiner tableColumn)
		{
			return String.Format("\n({0} INNER JOIN {1} ON {0}.{2} = {1}.{3})", tableColumn.T1, tableColumn.T2, tableColumn.C1, tableColumn.C2);
		}
	}
}
