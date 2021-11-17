using Bogus;
using Project_Work_WPF.Models;
using Project_Work_WPF.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class Admin_UserPage_ViewModel : BaseViewModel, IPageViewModel
	{
		public static ObservableCollection<Driver> Drivers { get; set; } = new ObservableCollection<Driver>();
		public Admin_UserPage_ViewModel()
		{

			GetSampleTableData();
		}

		private static void GetSampleTableData()
		{
			var customerId = 1;
			Random random = new Random();

			var userFaker = new Faker<Driver>()
				.CustomInstantiator(f => new Driver(customerId++.ToString()))
				.RuleFor(o => o.Age, f => random.Next(18, 50))
				.RuleFor(o => o.Name, f => f.Person.FirstName)
				.RuleFor(o => o.Surname, f => f.Person.LastName)
				.RuleFor(o => o.Email, (f, u) => f.Internet.Email(u.Name, u.Surname));

			var drivers = userFaker.Generate(10);

			foreach (var item in drivers)
			{
				Drivers.Add(item);
			}

		}
	}
}
