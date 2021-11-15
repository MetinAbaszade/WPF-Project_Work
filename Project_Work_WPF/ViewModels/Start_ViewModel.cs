using Project_Work_WPF.Commands;
using Project_Work_WPF.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Work_WPF.ViewModels
{
	class Start_ViewModel : BaseViewModel, IPageViewModel
	{
		public RelayCommand GoToAdminLogin { get; set; } = new RelayCommand(x =>
		{
			MainViewModel.Logged_As = "Admin";
			Mediator.Notify("GoToLogIn", "");
		});
		public RelayCommand GoToUserLogin { get; set; } = new RelayCommand(x =>
		{
			MainViewModel.Logged_As = "User";
			Mediator.Notify("GoToLogIn", "");
		});
	}
}
