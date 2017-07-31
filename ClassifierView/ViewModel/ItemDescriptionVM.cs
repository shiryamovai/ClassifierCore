namespace ClassifierView.Viewmodel
{
	using System.ComponentModel;
	using System.Windows;

	using ClassifierCore;

	/// <summary>
	/// Вью-модель дескриптора спецификатора.
	/// </summary>
	public class ItemDescriptionVm : TreeItemVm, INotifyPropertyChanged
	{
		/// <summary>
		/// Выделенный элемент.
		/// </summary>
		private static object _selectedItem;

		/// <summary>
		/// Код дескриптора.
		/// </summary>
		private string _code;

		/// <summary>
		/// Геометрия.e
		/// </summary>
		private string _geomType;

		/// <summary>
		/// Состояние выделенного элемента.
		/// </summary>
		private bool _isSelected;

		/// <summary>
		/// Имя дескриптора
		/// </summary>
		private string _name;

		private ItemVM _test;

		public ItemDescriptionVm(ClassifierStruct.ItemDescription itemDescription)
		{
			Name = itemDescription.Name;
			Code = itemDescription.Code;
			onSelectedItemTest += SeleсtOn;
		}

		public delegate void SelectedItemTest();

		public event SelectedItemTest onSelectedItemTest;

		public event PropertyChangedEventHandler PropertyChanged;

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
		/// Код дескриптора.
		/// </summary>
		public string Code
		{
			get
			{
				return _code;
			}
			set
			{
				_code = value;
				OnPropertyChanged("Code");
			}
		}
		/// <summary>
		/// Геометрия.
		/// </summary>
		public string GeomType
		{
			get
			{
				return _geomType;
			}
			set
			{
				_geomType = value;
				OnPropertyChanged("GeomType");
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

		public ItemVM Item { get; set; }
		/// <summary>
		/// Имя дескриптора.
		/// </summary>
		public new string Name
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

		public ItemVM Test
		{
			get
			{
				return _test;
			}
			set
			{
				_test = value;
				OnPropertyChanged("Test");
			}
		}
		/// <summary>
		/// Возвращает имя дескриптора + его код.
		/// </summary>
		public override string ViewName => Code + " - " + Name;

		public void Changetest()
		{
		}

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		public void SeleсtOn()
		{
			MessageBox.Show(SelectedItem.GetType().ToString());
		}
	}
}