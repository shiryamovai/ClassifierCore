using System;
using System.Text.RegularExpressions;

namespace ClassifierCore
{
	public class ParserDisplayStyle
	{
		/// <summary>
		/// Получение параметров из функции
		/// </summary>
		/// <param name="data"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string[] GetParams(string data, string type)
		{
			data = data.Trim();
			if (string.IsNullOrEmpty(data)) throw new Exception("Передана пустая строка");

			if (!data.StartsWith(type)) throw new Exception("Строка не может начинаться с " + type);

			var r = new Regex(".*\\((.*)\\)");
			Match m = r.Match(data);
			if (!m.Success) throw new Exception("Входная строка не корректна: " + type);

			return m.Groups[1].Value.Split(',');
		}
		
		#region Стили MapInfo

		/// <summary>
		/// Стиль условного знака
		/// </summary>
		public struct FontSymbol
		{
			/// <summary>
			/// Код символа
			/// </summary>
			public int SymbolCode;

			/// <summary>
			/// Цвет символа
			/// </summary>
			public int SymbolColor;

			/// <summary>
			/// Размер символа
			/// </summary>
			public int PointSize;

			/// <summary>
			/// Имя шрифта
			/// </summary>
			public string FontName;

			/// <summary>
			/// Стиль шрита
			/// </summary>
			public int FontStyle;

			/// <summary>
			/// Угол поворота символа
			/// </summary>
			public int Angle;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Описание стиля линии
		/// </summary>
		public struct Pen
		{
			/// <summary>
			/// Толщина линии
			/// </summary>
			public int Width;

			/// <summary>
			/// Стиль шаблона линии
			/// </summary>
			public int Pattern;

			/// <summary>
			/// Цвет линии
			/// </summary>
			public int Color;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Описание заливки контура
		/// </summary>
		public struct Brush
		{
			/// <summary>
			/// Стиль штриховки
			/// </summary>
			public int Pattern;

			/// <summary>
			/// Цвет штриховки
			/// </summary>
			public int ForeColor;

			/// <summary>
			/// Цвет заднего фона
			/// </summary>
			public int BackColor;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Описание шрифта
		/// </summary>
		public struct Font
		{
			/// <summary>
			/// Имя шрифта
			/// </summary>
			public string FontName;

			/// <summary>
			/// Цвет шрифта
			/// </summary>
			public int Color;

			/// <summary>
			/// Цвет заднего фона
			/// </summary>
			public int BackColor;

			/// <summary>
			/// Размер шрифта
			/// </summary>
			public int Size;

			/// <summary>
			/// Тип линии
			/// </summary>
			public int LineType;

			/// <summary>
			/// Неиспользуемы параметр
			/// </summary>
			public int unused;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Разбирает строку типа : FontSymbol(51,9502608,18,'RN_UZgeo_punkt',0,0)
		/// </summary>
		public static void Parse(string data, ref FontSymbol fontsymbol)
		{
			string[] arguments = GetParams(data, "FontSymbol");
			if (arguments.Length < 6)
			{
				throw new Exception("Invalid numbers of params");
			}

			fontsymbol.SymbolCode = int.Parse(arguments[0]);
			fontsymbol.SymbolColor = int.Parse(arguments[1]);
			fontsymbol.PointSize = int.Parse(arguments[2]);
			fontsymbol.FontName = arguments[3];
			fontsymbol.FontName = fontsymbol.FontName.Replace('\'', ' ').Trim();
			fontsymbol.FontName = fontsymbol.FontName.Replace('\"', ' ').Trim();
			fontsymbol.FontStyle = int.Parse(arguments[4]);
			fontsymbol.Angle = arguments[5] == "*" ? int.MinValue : int.Parse(arguments[5]);
			fontsymbol.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : Pen(1,2,0)
		/// </summary>
		public static void Parse(string data, ref Pen pen)
		{
			string[] arguments = GetParams(data, "Pen");
			if (arguments.Length < 3) throw new Exception("Invalid numbers of params");
			pen.Width = int.Parse(arguments[0]);
			pen.Pattern = int.Parse(arguments[1]);
			pen.Color = int.Parse(arguments[2]);
			pen.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : Brush(1,0,-1)
		/// </summary>
		public static void Parse(string data, ref Brush brush)
		{
			string[] arguments = GetParams(data, "Brush");
			if (arguments.Length < 3) throw new Exception("Invalid numbers of params");
			brush.Pattern = int.Parse(arguments[0]);
			brush.ForeColor = int.Parse(arguments[1]);
			brush.BackColor = int.Parse(arguments[2]);
			brush.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : Font("T-132",0,20,0,-1,[0-2])
		/// </summary>
		public static void Parse(string data, ref Font font)
		{
			string[] arguments = GetParams(data, "Font");
			if ((arguments.Length < 5) || (arguments.Length > 6)) throw new Exception("Invalid numbers of params");

			font.FontName = arguments[0];
			font.FontName = font.FontName.Replace('\'', ' ').Trim();
			font.FontName = font.FontName.Replace('\"', ' ').Trim();
			font.Color = int.Parse(arguments[1]);            
			font.Size = int.Parse(arguments[2]);
			font.BackColor = int.Parse(arguments[3]);
			font.unused = int.Parse(arguments[4]);
			font.LineType = 0;
			if (arguments.Length > 5)
			{
				int i;
				int.TryParse(arguments[5], out i);
				if (i <= 2) font.LineType = i;
				else throw new Exception("Invalid line type");
			}

			font.Valid = true;
		}


		/// <summary>
		/// Разбирает строку типа : Subtype("Все");
		/// </summary>
		public static void ParseSubtype(string data, ref string type)
		{
			string[] arguments = GetParams(data, "Subtype");
			if (arguments.Length < 1) throw new Exception("Invalid numbers of params");

			type = arguments[0].Replace('\'', ' ').Replace('\"', ' ').Trim();
		}

		#endregion

		#region  Стили AutoCad

		/// <summary>
		/// Параметр берётся из слоя
		/// </summary>
		const string BY_LAYER = "bylayer";

		/// <summary>
		/// Параметр отсутствует
		/// </summary>
		const string NONE_VALUE = "none";

		/// <summary>
		/// Стиль линии
		/// </summary>
		public struct LineAutoCad
		{
			/// <summary>
			/// Тип линии
			/// </summary>
			public string Pattern;

			/// <summary>
			/// Цвет линии
			/// </summary>
			public int Color;

			/// <summary>
			/// Толщина линии
			/// </summary>
			public double Thickness;

			/// <summary>
			/// Толщина линии
			/// </summary>
			public double LineWidth;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Описание заливки контура
		/// </summary>
		public struct BrushAutoCad
		{
			/// <summary>
			/// Тип штриховки
			/// </summary>
			public string Pattern;

			/// <summary>
			/// Цвет штриховки
			/// </summary>
			public int ForeColor;

			/// <summary>
			/// Цвет заливки
			/// </summary>
			public int BackColor;

			/// <summary>
			/// Масштаб штриховки
			/// </summary>
			public double Scale;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Описание COGO точки
		/// </summary>
		public struct COGOPointAutoCad
		{
			/// <summary>
			/// Угол поворота
			/// </summary>
			public double Angle;

			/// <summary>
			/// Цвет
			/// </summary>
			public int Color;

			/// <summary>
			/// Масштаб
			/// </summary>
			public double Scale;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Разбирает строку типа : Line(TN_2,32,0.15)
		/// </summary>
		public static void Parse(string data, ref LineAutoCad pen)
		{
			string[] arguments = GetParams(data, data.StartsWith("Line") ? "Line" : "Mline");
			if (arguments.Length < 3) throw new Exception("Неверное описание оформления линии");

			pen.Pattern = arguments[0];
			var arg = arguments[1].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) pen.Color = int.MinValue;
			else pen.Color = int.Parse(arg);

			arg = arguments[2].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE)
				pen.Thickness = double.MinValue;
			else
				pen.Thickness = double.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);

			if (arguments.Length > 3)
			{
				arg = arguments[3].ToLower();
				if (arg == BY_LAYER || arg == NONE_VALUE) pen.LineWidth = double.MinValue;
				else pen.LineWidth = double.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				pen.LineWidth = double.MinValue;
			}

			pen.Valid = true;
		}
		
		/// <summary>
		/// Разбирает строку типа : Brush(TN_xx,3,None,1);
		/// </summary>
		public static void Parse(string data, ref BrushAutoCad brush)
		{
			string[] arguments = GetParams(data, "Brush");
			if (arguments.Length < 4) throw new Exception("Неверное описание оформления заливки");

			brush.Pattern = arguments[0];

			var arg = arguments[1].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) brush.ForeColor = int.MinValue;
			else brush.ForeColor = int.Parse(arg);

			arg = arguments[2].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) brush.BackColor = int.MinValue;
			else brush.BackColor = int.Parse(arg);

			arg = arguments[3].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) brush.Scale = double.MinValue;
			else brush.Scale = double.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);

			brush.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : COGOPoint(0,ByLayer,0.5)
		/// </summary>
		public static void Parse(string data, ref COGOPointAutoCad cogoPoint)
		{
			string[] arguments = GetParams(data, "COGOPoint");
			if (arguments.Length < 3) throw new Exception("Неверное описание оформления условного знака");

			cogoPoint.Angle = double.Parse(arguments[0], System.Globalization.CultureInfo.InvariantCulture);

			var arg = arguments[1].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) cogoPoint.Color = int.MinValue;
			else cogoPoint.Color = int.Parse(arg);

			arg = arguments[2].ToLower();
			if (arg == BY_LAYER || arg == NONE_VALUE) cogoPoint.Scale = double.MinValue;
			else cogoPoint.Scale = double.Parse(arg, System.Globalization.CultureInfo.InvariantCulture);
			cogoPoint.Valid = true;
		}

		#endregion

		#region  Стили ArcMap

		/// <summary>
		/// Стиль линии
		/// </summary>
		public struct LineArcMap
		{
			/// <summary>
			/// Наименование библиотеки стилей
			/// </summary>
			public string LibraryName;

			/// <summary>
			/// Тип линии
			/// </summary>
			public string Linestyle;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Стиль полигона
		/// </summary>
		public struct AreaArcMap
		{
			/// <summary>
			/// Наименование библиотеки стилей
			/// </summary>
			public string LibraryName;

			/// <summary>
			/// Тип линии
			/// </summary>
			public string FillStyle;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Стиль полигона
		/// </summary>
		public struct PointArcMap
		{
			/// <summary>
			/// Наименование библиотеки стилей
			/// </summary>
			public string LibraryName;

			/// <summary>
			/// Тип линии
			/// </summary>
			public string Markerstyle;

			/// <summary>
			/// Является ли описание корректным
			/// </summary>
			public bool Valid;
		}

		/// <summary>
		/// Разбирает строку типа :rn_topo_500.style;linestyle:0415100000
		/// </summary>
		public static void Parse(string data, ref LineArcMap pen)
		{
			string[] arguments = data.Split(';');
			if (arguments.Length < 2) throw new Exception("Неверное описание оформления линии");
			pen.LibraryName = arguments[0];

			string[] styleParts = arguments[1].Split(':');
			if (styleParts.Length < 2) throw new Exception("Неверное описание оформления линии");
			pen.Linestyle = styleParts[1];
			if (!string.Equals(styleParts[0], "linestyle", StringComparison.OrdinalIgnoreCase))
				throw new Exception("Неверное описание оформления линии");
			if (string.IsNullOrEmpty(pen.Linestyle))
				throw new Exception("Неверное описание оформления линии");            
			pen.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : rn_topo_500.style;markerstyle:0871500000
		/// </summary>
		public static void Parse(string data, ref PointArcMap point)
		{
			string[] arguments = data.Split(';');
			if (arguments.Length < 2)
				throw new Exception("Неверное описание оформления условного знака");
			point.LibraryName = arguments[0];

			string[] styleParts = arguments[1].Split(':');
			if (styleParts.Length < 2)
				throw new Exception("Неверное описание оформления условного знака");
			point.Markerstyle = styleParts[1];
			if (!string.Equals(styleParts[0], "markerstyle", StringComparison.OrdinalIgnoreCase))
				throw new Exception("Неверное описание оформления условного знака");
			if (string.IsNullOrEmpty(point.Markerstyle))
				throw new Exception("Неверное описание оформления условного знака");
			point.Valid = true;
		}

		/// <summary>
		/// Разбирает строку типа : rn_topo_500.style;fillstyle:0830000000
		/// </summary>
		public static void Parse(string data, ref AreaArcMap area)
		{
			string[] arguments = data.Split(';');
			if (arguments.Length < 2)
				throw new Exception("Неверное описание оформления полигона");
			area.LibraryName = arguments[0];

			string[] styleParts = arguments[1].Split(':');
			if (styleParts.Length < 2)
				throw new Exception("Неверное описание оформления полигона");
			area.FillStyle = styleParts[1];
			if (!string.Equals(styleParts[0], "fillstyle", StringComparison.OrdinalIgnoreCase))
				throw new Exception("Неверное описание оформления полигона");
			if (string.IsNullOrEmpty(area.FillStyle))
				throw new Exception("Неверное описание оформления полигона");
			area.Valid = true;
		}

		#endregion
	}
}
