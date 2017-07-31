namespace ClassifierView.Viewmodel
{
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Linq;

	using ClassifierCore;

	/// <summary>
	/// Вью-модель структуры классификтора.
	/// </summary>
	public class StructVm : INotifyPropertyChanged
	{
		/// <summary>
		/// Список групп классов атрибутов.
		/// </summary>
		private ObservableCollection<AttributeGroupVM> _attriuAttributeGroupsVM;

		/// <summary>
		/// Класс аттрибута.
		/// </summary>
		private ClassifierStruct.AttributeGroup _classItem;

		/// <summary>
		/// Экземпляр загрузчика.
		/// </summary>
		private LoaderXML _loader;

		/// <summary>
		/// Экземпляр базового класса модели.
		/// </summary>
		private ClassifierStruct myClassifierStruct;

		/// <summary>
		/// Коллекция разделов.
		/// </summary>
		private ObservableCollection<SectionVm> sectionsVM;

		public StructVm()
		{
			MyClassifierStruct = new ClassifierStruct();
			Loader = new LoaderXML();
			SectionsVm = new ObservableCollection<SectionVm>();
			AttributeGroupsVm = new ObservableCollection<AttributeGroupVM>();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Список групп классов.
		/// </summary>
		public ObservableCollection<AttributeGroupVM> AttributeGroupsVm { get; set; }

		/// <summary>
		/// Свойство класс аттрибута.
		/// </summary>
		public ClassifierStruct.AttributeGroup ClassItem
		{
			get
			{
				return _classItem;
			}
			set
			{
				_classItem = value;
				OnPropertyChanged("ClassItem");
			}
		}

		/// <summary>
		/// Свойство загрузчика.
		/// </summary>
		public LoaderXML Loader
		{
			get
			{
				return _loader;
			}
			set
			{
				_loader = value;
				OnPropertyChanged("Loader");
			}
		}

		/// <summary>
		/// Свойство базового класса модели.
		/// </summary>
		public ClassifierStruct MyClassifierStruct
		{
			get
			{
				return myClassifierStruct;
			}
			set
			{
				myClassifierStruct = value;
				OnPropertyChanged("MyClassifierStruct");
			}
		}

		/// <summary>
		/// Коллекция разделов.
		/// </summary>
		public ObservableCollection<SectionVm> SectionsVm { get; set; }

		/// <summary>
		/// Заполнение группы аттрибутов.
		/// </summary>
		public void AttributeGroupSet()
		{
			foreach (var attgr in MyClassifierStruct.AttributeGroups)
				AttributeGroupsVm.Add(new AttributeGroupVM(attgr));
		}

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		/// <summary>
		/// Заполнение разделов.
		/// </summary>
		public void SectionSet()
		{
			foreach (var section in MyClassifierStruct.Sections)
			{
				var sectionVm = new SectionVm(section);
				SectionsVm.Add(sectionVm);
			}

			var itemDescriptionsVm = SectionsVm.SelectMany(section => section.ItemDescriptionVms).ToList();
			foreach (var item in AttributeGroupsVm.SelectMany(g => g.Items))
			{
				var itemsDescriptionVm = itemDescriptionsVm.FirstOrDefault(i => i.Code == item.Code);
				if (itemsDescriptionVm != null)
					itemsDescriptionVm.Item = item;
			}
		}

		/// <summary>
		/// Загрузках данных AUTOCAD.
		/// </summary>
		/// <param name="ClassfierName"></param>
		public void StructLoad(string ClassfierName)
		{
			MyClassifierStruct = Loader.loadAutoCad(ClassfierName);
			AttributeGroupSet();
			SectionSet();
		}
	}
}