using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
	/// <summary>
	/// Чат с сообщениями
	/// </summary>
	public class Chat : ObservableObject
	{
		private ObservableCollection<IMessage> _Messages;
		/// <summary>
		/// Все сообщения в чате (в том числе системные)
		/// </summary>
		public ObservableCollection<IMessage> Messages
		{
			get { return _Messages; }
			set { SetProperty(ref _Messages, value); }
		}

		/// <summary>
		/// Создать экземпляр чата
		/// </summary>
		public Chat() 
		{
			_Messages = new ObservableCollection<IMessage>();
		}
    }
}
