using System;
using System.Windows.Forms;
using ClassifierCore;


namespace TestApplication {
    public partial class FormTestApp : Form {
        public FormTestApp() {
            InitializeComponent();
        }

        private void ButtonTestClick(object sender, EventArgs e) {
            var classifier = new Classifier();
            var fileDialog = new OpenFileDialog
							{
								Filter = "Файл классификатора (*.xml)|*.xml|" + "Все файлы (*.*)|*.*"
							};
			ClassifierStruct classifierStruct = null;
	        if (fileDialog.ShowDialog() == DialogResult.OK) {
                // LoaderXML.CriticalLoad = true;
				classifierStruct = classifier.Loader.load(fileDialog.FileName);
            }
        }

        private void Button1Click(object sender, EventArgs e) {
            var classifier = new Classifier();
            var fileDialog = new OpenFileDialog
							{
								Filter = "Файл классификатора (*.xml)|*.xml|" + "Все файлы (*.*)|*.*"
							};
	        ClassifierStruct classifierStruct = null;
	        if (fileDialog.ShowDialog() == DialogResult.OK) {
                // LoaderXML.CriticalLoad = true;
				classifierStruct = classifier.Loader.loadAutoCad(fileDialog.FileName);
            }

			//if (classifierStruct != null)
			//{
			//    foreach (var group in classifierStruct.AttributeGroups)
			//    {
			//        var item = group.GetItem("0115000000", "COGOPoint");
			//        if (item != null)
			//        {
			//            MessageBox.Show("Группа атрибутов найдена");
			//            break;
			//        }
			//    }
			//}
        }
    }
}
