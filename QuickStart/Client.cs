using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	class Client
	{
		public string SchemaFileName { get; set; }
		public string CmbName { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public bool IsNull()
		{
			return (Name == "") ? true : false;
		}
	}
}
