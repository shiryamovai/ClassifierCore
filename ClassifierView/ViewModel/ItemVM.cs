namespace ClassifierView.Viewmodel
{
	using System.Collections.ObjectModel;
	using System.ComponentModel;

	using ClassifierCore;

	/// <summary>
	/// Вью модель класса атрибутов.
	/// </summary>
	public class ItemVM : INotifyPropertyChanged
	{
		/// <summary>
		/// Список атрибутов.
		/// </summary>
		private ObservableCollection<AttributeVM> _attributes;

		/// <summary>
		/// Код класса атрибутов.
		/// </summary>
		private string _code;

		/// <summary>
		/// Геометрия.
		/// </summary>
		private string _geomType;

		public ItemVM(ClassifierStruct.AttributeClassItem classItem)
		{
			Code = classItem.Code;
			Attributes = new ObservableCollection<AttributeVM>();
			GeomType = classItem.GeomType;
			foreach (var attr in classItem.Attributes)
				Attributes.Add(new AttributeVM(attr));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<AttributeVM> Attributes { get; set; }

		/// <summary>
		/// Код класса атрибутов.
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

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}