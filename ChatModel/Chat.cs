using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
	public class Chat : ObservableObject
	{
		private ObservableCollection<Message> _Messages;
		public ObservableCollection<Message> Messages
		{
			get { return _Messages; }
			set { SetProperty(ref _Messages, value); }
		}

		public Chat() 
		{
			Messages = new ObservableCollection<Message>();
		}
    }
}
