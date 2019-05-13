using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Core.FormAuthentication
{
	public interface IUser
	{
        string Name { get; set; }
        //IUser ConverFromArry(string content);
		string ToArry();
	}
}
