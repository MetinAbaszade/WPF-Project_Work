using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Work_WPF.Models
{
	class Driver
	{
		public Driver(string DriverID)
		{
			this.id = DriverID;
		}

		public Driver(string name, string surname, string email, int age, string ID)
		{
			Name = name;
			Surname = surname;
			Email = email;
			Age = age;
			id = ID;
		}

		public string id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public int Age { get; set; }
	}
}
