using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project_Work_WPF.Commands
{
	class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		private Action<object> _execute;
		private Predicate<object> _canExecute;

		public RelayCommand(Action<object> exe, Predicate<object> canExe = null)
		{
			if (exe == null)
				throw new NullReferenceException();

			_execute = exe;
			_canExecute = canExe;

		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute.Invoke(parameter);
		}
	}

}
