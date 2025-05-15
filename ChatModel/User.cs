using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChatModel
{
	/// <summary>
	/// Пользователь чата
	/// </summary>
    public class User : ObservableObject, INotifyDataErrorInfo
    {
		private string _Name;
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [UserName]
		public string Name
		{
			get { return _Name; }
			set 
			{
                ValidateProperty(value, nameof(Name));

                if (!HasErrors)
                {
                    _Name = value;
                }

                OnPropertyChanged();
            }
		}

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        public User(string name) 
		{
			_Name = name;
		}


        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        public User()
		{
			_Name = string.Empty;
		}

        private void ValidateProperty(object value, string propertyName)
        {
            var validationContext = new ValidationContext(this) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(value, validationContext, validationResults);

            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
            }

            if (validationResults.Count > 0)
            {
                _errors[propertyName] = validationResults.Select(vr => vr.ErrorMessage).ToList();
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        public bool HasErrors => _errors.Count > 0;
    }


    public class UserNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string userName)
            {
                if (userName.Length < 2 || userName.Length > 30)
                    ErrorMessage = "Имя должно содержать от 2 до 30 символов";
                else if (userName.ToLower() == "admin")
                    ErrorMessage = "Некорректное имя: admin";
                else
                    return true;
            }
            return false;
        }
    }
}
