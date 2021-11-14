using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Project_Work_WPF.CustomExceptions;
using Project_Work_WPF.Models;
using Project_Work_WPF.Navigation;
using PropertyChanged;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class MainViewModel : BaseViewModel
	{
		static List<Person> People = new List<Person>();

		public static void Add_Person(string username, string password)
		{
			if (People.Exists(x => x.Username == username))
			{
				throw new InvalidDataException();
			}
			else {
				Person person = new Person(username, password);
				People.Add(person);
			}
		}

		public static bool Check_Person(string username, string password)
		{

			if (People.Exists(x => x.Username == username && x.Password == password))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		private List<IPageViewModel> _pageViewModels;

		public List<IPageViewModel> PageViewModels
		{
			get
			{
				if (_pageViewModels == null)
					_pageViewModels = new List<IPageViewModel>();

				return _pageViewModels;
			}
		}

		public IPageViewModel _currentPageViewModel { get; set; }

		public IPageViewModel CurrentPageViewModel
		{

			get
			{
				return _currentPageViewModel;
			}

			set
			{
				_currentPageViewModel = value;
				OnPropertyChanged("CurrentPageViewModel");
			}
		}

		private void ChangeViewModel(IPageViewModel viewModel)
		{
			if (!PageViewModels.Contains(viewModel))
				PageViewModels.Add(viewModel);

			CurrentPageViewModel = PageViewModels
				.FirstOrDefault(vm => vm == viewModel);
		}

		private void GoToLogIn(object obj)
		{
			ChangeViewModel(PageViewModels[0]);
		}

		private void GoToUser(object obj)
		{
			(PageViewModels[1] as User_Page_ViewModel).GetCurrentLocation();
			//(PageViewModels[1] as User_Page_ViewModel).zoomlevel = 5;
			//(PageViewModels[1] as User_Page_ViewModel).center = new Microsoft.Maps.MapControl.WPF.Location(40.4093, 49.8671);
			ChangeViewModel(PageViewModels[1]);
		} 

		private void GoToRegister(object obj)
		{
			(PageViewModels[2] as Register_Page_ViewModel).Passwordd = string.Empty;
			(PageViewModels[2] as Register_Page_ViewModel).password_box_visibility = System.Windows.Visibility.Visible;
			(PageViewModels[2] as Register_Page_ViewModel).password_box_visibility_2 = System.Windows.Visibility.Collapsed;
			(PageViewModels[2] as Register_Page_ViewModel).Hidden = false;
			ChangeViewModel(PageViewModels[2]);
		}

		private void GoToHistory(object obj)
		{ 
			ChangeViewModel(PageViewModels[3]);
		}

		public MainViewModel()
		{
			// Add available pages and set page
			PageViewModels.Add(new Login_Page_ViewModel());
			PageViewModels.Add(new User_Page_ViewModel());
			PageViewModels.Add(new Register_Page_ViewModel());
			PageViewModels.Add(new History_Page_ViewModel());
			ChangeViewModel(PageViewModels[1]);
			 

			Mediator.Subscribe("GoToLogIn", GoToLogIn);
			Mediator.Subscribe("GoToUser", GoToUser);
			Mediator.Subscribe("GoToRegister", GoToRegister);
			Mediator.Subscribe("GoToHistory", GoToHistory);
		}
	}
}
