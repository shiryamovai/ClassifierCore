namespace ClassifierView.Viewmodel
{
	using System.ComponentModel;

	using ClassifierCore;

	/// <summary>
	/// Вью модель атрибута.
	/// </summary>
	public class AttributeVM : INotifyPropertyChanged
	{
		/// <summary>
		/// Имя атрибута.
		/// </summary>
		private string _name;

		public AttributeVM(ClassifierStruct.Attribute attr)
		{
			_name = attr.Name;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Имя атрибута.
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

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}