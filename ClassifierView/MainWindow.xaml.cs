namespace ClassifierView
{
	using System.Windows;

	using ClassifierView.Viewmodel;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ClassificierEditorVM();
		}
	}
}