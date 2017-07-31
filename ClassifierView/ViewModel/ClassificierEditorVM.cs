namespace ClassifierView.Viewmodel
{
	using System.ComponentModel;

	using ClassifierCore;

	using ClassifierView.Command;

	using Microsoft.Win32;

	public class ClassificierEditorVM : INotifyPropertyChanged
	{
		/// <summary>
		/// Диалог открытия AUTOCAD.
		/// </summary>
		private readonly OpenFileDialog _ofdAutoCAD;

		/// <summary>
		/// Диалог открытия GIS.
		/// </summary>
		private readonly OpenFileDialog _ofdGIS;

		/// <summary>
		/// Путь сохранения AUTOCAD.
		/// </summary>
		private readonly SaveFileDialog _sfdAutoCAD;

		/// <summary>
		/// Пусть сохранения GIS.
		/// </summary>
		private readonly SaveFileDialog _sfdGIT;

		/// <summary>
		/// Путь документа AUTOCAD.
		/// </summary>
		private string _autoCADName;

		/// <summary>
		/// Путь документа GIS
		/// </summary>
		private string _gisName;

		/// <summary>
		/// Экземпляр класса LoaderXML.
		/// </summary>
		private LoaderXML _loader;

		/// <summary>
		/// Экземпляр класса SructVM.
		/// </summary>
		private StructVm _struct;

		/// <summary>
		/// Комманда открытия файла AUTOCAD.
		/// </summary>
		private RelayCommand OpenAUTOCADXML;

		/// <summary>
		/// Комманда открытия файла GIS.
		/// </summary>
		private RelayCommand OpenGISXML;

		/// <summary>
		/// Комманда сохранения AUTOCAD.
		/// </summary>
		private RelayCommand SaveAUTOCADXML;

		/// <summary>
		/// Комманда сохранения GIS.
		/// </summary>
		private RelayCommand SaveGISXML;

		public ClassificierEditorVM()
		{
			Struct = new StructVm();
			_ofdGIS = new OpenFileDialog();
			_ofdAutoCAD = new OpenFileDialog();
			_sfdGIT = new SaveFileDialog();
			_sfdAutoCAD = new SaveFileDialog();
			_ofdGIS.Filter = "Xml files(GIS_*.xml)|GIS_*.xml|All files(*.*)|*.*";
			_ofdAutoCAD.Filter = "Xml files(AUTOCAD_*.xml)|AUTOCAD_*.xml|All files(*.*)|*.*";
			_sfdGIT.Filter = "Xml files(GIS_*.xml)|GIS_*.xml|All files(*.*)|*.*";
			_sfdAutoCAD.Filter = "Xml files(AUTOCAD_*.xml)|AUTOCAD_*.xml|All files(*.*)|*.*";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Свойство путь файла AUTOCAD.
		/// </summary>
		public string AutoCadName
		{
			get
			{
				return _autoCADName;
			}
			set
			{
				_autoCADName = value;
				OnPropertyChanged("AutoCadName");
			}
		}

		/// <summary>
		/// Свойство путь файла GIS.
		/// </summary>
		public string GISname
		{
			get
			{
				return _gisName;
			}
			set
			{
				_gisName = value;
				OnPropertyChanged("GISname");
			}
		}

		/// <summary>
		/// Обработчик комманды открытия AUTOCAD файла.
		/// </summary>
		public RelayCommand openAUTOCADXML
		{
			get
			{
				return OpenAUTOCADXML ?? (OpenAUTOCADXML = new RelayCommand(
							obj =>
							{
								_ofdAutoCAD.ShowDialog();
								AutoCadName = _ofdAutoCAD.FileName;
								Struct.StructLoad(AutoCadName);
							}));
			}
		}

		/// <summary>
		/// Обработчик команды открытия GIS файла.
		/// </summary>
		public RelayCommand openGISXML
		{
			get
			{
				return OpenGISXML ?? (OpenGISXML = new RelayCommand(
							obj =>
							{
								_ofdGIS.ShowDialog();
								GISname = _ofdGIS.FileName;
							}));
			}
		}

		/// <summary>
		/// Обработчик команды сохранения AUTOCAD файла.
		/// </summary>
		public RelayCommand saveAUTOCADXML
		{
			get
			{
				return SaveAUTOCADXML ?? (SaveAUTOCADXML = new RelayCommand(
							obj =>
							{
								_sfdAutoCAD.ShowDialog();
								AutoCadName = _sfdAutoCAD.FileName;
							}));
			}
		}

		/// <summary>
		/// Обработчик команды сохранения GIS файла.
		/// </summary>
		public RelayCommand saveGISXML
		{
			get
			{
				return SaveGISXML ?? (SaveGISXML = new RelayCommand(
							obj =>
							{
								_sfdGIT.ShowDialog();
								GISname = _sfdGIT.FileName;
							}));
			}
		}

		public StructVm Struct
		{
			get
			{
				return _struct;
			}
			set
			{
				_struct = value;
				OnPropertyChanged("Struct");
			}
		}

		public void OnPropertyChanged(string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}