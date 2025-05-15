using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ChatModel
{
	/// <summary>
	/// Пользователь чата
	/// </summary>
    public class User : ObservableObject
    {
		private string _Name;
		/// <summary>
		/// Имя пользователя
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; OnPropertyChanged(); }
		}

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        public User(string name) 
		{
			Name = name;
		}
    }
}
