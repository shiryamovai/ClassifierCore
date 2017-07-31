namespace ClassifierView.Viewmodel
{
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Windows;

	using ClassifierCore;

	/// <summary>
	/// Вью-модель раздела классификтора.
	/// </summary>
	public class SectionVm : TreeItemVm, INotifyPropertyChanged
	{
		/// <summary>
		/// Выбранный элемент.
		/// </summary>
		private static object _selectedItem;

		/// <summary>
		/// Состояние выбранного элемента.
		/// </summary>
		private bool _isSelected;

		/// <summary>
		/// Коллекция дескрипторов.
		/// </summary>
		private ObservableCollection<ItemDescriptionVm> _itemDescriptionVms;

		public SectionVm(ClassifierStruct.Section section)
		{
			Name = section.Name;
			ItemDescriptionVms = new ObservableCollection<ItemDescriptionVm>();

			foreach (var itemDescription in section.ItemDescriptions)
				ItemDescriptionVms.Add(new ItemDescriptionVm(itemDescription));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Выделенный элемента.
		/// </summary>
		public static object SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			private set
			{
				if (_selectedItem != value)
				{
					_selectedItem = value;
					MessageBox.Show("Select изменился");
				}
			}
		}

		/// <summary>
		/// Состояние выделенного элемента.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged("IsSelected");
					OnPropertyChanged("SelectItem");
					if (_isSelected)
						SelectedItem = this;
				}
			}
		}

		public void OnPropertyChanged(string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}