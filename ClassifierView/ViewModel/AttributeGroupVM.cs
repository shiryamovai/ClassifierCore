namespace ClassifierView.Viewmodel
{
	using System.Collections.ObjectModel;
	using System.ComponentModel;

	using ClassifierCore;

	/// <summary>
	/// Вью модель группы классов атрибутов.
	/// </summary>
	public class AttributeGroupVM : INotifyPropertyChanged
	{
		/// <summary>
		/// Список классов атрибутов
		/// </summary>
		private ObservableCollection<ItemVM> _items;

		public AttributeGroupVM(ClassifierStruct.AttributeGroup attributeGroup)
		{
			Items = new ObservableCollection<ItemVM>();
			foreach (var items in attributeGroup.Items)
				Items.Add(new ItemVM(items));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Список классов атрибутов
		/// </summary>
		public ObservableCollection<ItemVM> Items { get; set; }

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}