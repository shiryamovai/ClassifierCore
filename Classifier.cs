namespace ClassifierCore
{
	/// <summary>
	/// Класс для получения обработчика классификатора
	/// </summary>
	public class Classifier
	{
		public const long InvalidCode = -1L;

		/// <summary>
		/// Загрузчик классификатора
		/// </summary>
		public readonly LoaderXML Loader = new LoaderXML();
	}

}
