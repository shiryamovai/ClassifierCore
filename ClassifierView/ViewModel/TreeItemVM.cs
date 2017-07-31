namespace ClassifierView.Viewmodel
{
	using System.Collections.ObjectModel;
	using System.ComponentModel;

	public abstract class TreeItemVm : INotifyPropertyChanged
	{
		/// <summary>
		/// Имя элемента дерева.
		/// </summary>
		private string _name;

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Коллекция дескрипторов.
		/// </summary>
		public ObservableCollection<ItemDescriptionVm> ItemDescriptionVms { get; set; }

		/// <summary>
		/// Имя элемента дерева.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// Возвращяет имся элемента дерева.
		/// </summary>
		public virtual string ViewName => Name;

		public void OnPropertyChanged(string prop = " ")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}