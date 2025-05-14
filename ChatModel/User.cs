using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ChatModel
{
    public class User : ObservableObject
    {
		private string _Name;
		public string Name
		{
			get { return _Name; }
			set { _Name = value; OnPropertyChanged(); }
		}


    }
}
