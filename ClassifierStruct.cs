namespace ClassifierCore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Класс описывающий структуру классификатора
	/// </summary>
	public class ClassifierStruct
	{
		/// <summary>
		/// Все секции в классификаторе
		/// </summary>
		public readonly List<Section> Sections = new List<Section>();

		/// <summary>
		/// Описание типа класса
		/// </summary>
		public enum TypeItemDescription
		{
			/// <summary>
			/// Простой объект
			/// </summary>
			Simple,

			/// <summary>
			/// Обрыв
			/// </summary>
			Steep
		}

		/// <summary>
		/// Определение подписи для Autocad. XML таг : Annotation.
		/// </summary>
		public class AnnotationDef
		{
			/// <summary>
			/// Должна ли иметь подпись цвет как у подписываемого объекта
			/// </summary>
			public bool ColorByFeature;

			/// <summary>
			/// Имя таблицы где должна хранится подпись
			/// </summary>
			public string TableName;

			public AnnotationDef(string tableName, bool colorByFeature)
			{
				TableName = tableName;
				ColorByFeature = colorByFeature;
			}
		}

		/// <summary>
		/// Описание атрибута
		/// </summary>
		public class Attribute
		{
			/// <summary>
			/// Правила для атрибутов
			/// </summary>
			public readonly AttributeRules AttributeRules = new AttributeRules();

			/// <summary>
			/// Коментарий
			/// </summary>
			public readonly string Comment = string.Empty;

			/// <summary>
			/// Имя поля
			/// </summary>
			public readonly string Field_name;

			/// <summary>
			/// Тип поля
			/// </summary>
			public readonly AttributeFieldType Field_type = new AttributeFieldType();

			/// <summary>
			/// Имя атрибута
			/// </summary>
			public readonly string Name = string.Empty;

			/// <summary>
			/// Статус
			/// </summary>
			public readonly string Status;

			/// <summary>
			/// Иницилизация атрибута
			/// </summary>
			/// <param name="name">
			/// The name.
			/// </param>
			/// <param name="fieldName">
			/// The field_name.
			/// </param>
			/// <param name="fieldType">
			/// The field_type.
			/// </param>
			/// <param name="comment">
			/// The comment.
			/// </param>
			/// <param name="status">
			/// The status.
			/// </param>
			public Attribute(string name, string fieldName, string fieldType, string comment, string status)
			{
				Name = name;
				Field_name = fieldName;
				Field_type.Text = fieldType;
				if (fieldType.ToUpper().Contains("DECIMAL"))
				{
					Field_type.isDecimal = true;
					GetFieldDecimalWidths(fieldType);
				}
				else
				{
					Field_type.Width = GetFieldWidth(fieldType);
				}

				Field_type.Type = GetFieldType(fieldType);
				Comment = comment;
				Status = status;
			}

			/// <summary>
			/// Получить размерность для десятичного числа
			/// </summary>
			/// <param name="attributeType">
			/// The attribute Type.
			/// </param>
			private void GetFieldDecimalWidths(string attributeType)
			{
				var index = attributeType.IndexOf('(');
				if (index == -1)
					return;

				index++;
				var lastIndex = attributeType.IndexOf(')', index);
				if (lastIndex == -1)
					lastIndex = attributeType.Length;

				var typesWidths = attributeType.Substring(index, lastIndex - index);
				var widths = typesWidths.Split(',');

				if (widths.Length < 2)
					throw new ArgumentOutOfRangeException(
						"Невозможно получить длину типа, неверное число аргументов '" + attributeType + "'.");

				if (!int.TryParse(widths[0].Trim(), out Field_type.Width))
					throw new ArgumentException("Не верно задана длина типа '" + attributeType + "'.");

				if (!int.TryParse(widths[1].Trim(), out Field_type.Precision))
					throw new ArgumentException("Не верно задана точность типа '" + attributeType + "'.");
			}

			/// <summary>
			/// Получить тип поля
			/// </summary>
			/// <param name="attributeType">
			/// The attribute Type.
			/// </param>
			/// <returns>
			/// The <see cref="string"/>.
			/// </returns>
			private string GetFieldType(string attributeType)
			{
				var index = attributeType.IndexOf('(');
				if (index != -1)
					attributeType = attributeType.Substring(0, index);
				return attributeType.Trim().ToUpper();
			}

			/// <summary>
			/// Получить размерность поля
			/// </summary>
			/// <param name="attributeType">
			/// The attribute Type.
			/// </param>
			/// <returns>
			/// The <see cref="int"/>.
			/// </returns>
			private int GetFieldWidth(string attributeType)
			{
				var index = attributeType.IndexOf('(');
				if (index == -1)
					return 0;

				index++;
				var lastIndex = attributeType.IndexOf(')', index);
				if (lastIndex == -1)
					lastIndex = attributeType.Length;

				var type = attributeType.Substring(index, lastIndex - index).Trim();

				int width;
				if (!int.TryParse(type.Trim(), out width))
					throw new ArgumentException("Не верно задана длина типа '" + attributeType + "'.");

				return width;
			}
		}

		/// <summary>
		/// Описание аттрибутов для AutoCad классификатора
		/// </summary>
		public class AttributeClassItem
		{
			/// <summary>
			/// Атрибуты
			/// </summary>
			public List<Attribute> Attributes = new List<Attribute>();

			/// <summary>
			/// Код класса
			/// </summary>
			public string Code = string.Empty;

			/// <summary>
			/// Код класса (Int32 значение)
			/// </summary>
			public int CodeInt32;

			/// <summary>
			/// Тип геометрии
			/// </summary>
			public string GeomType = string.Empty;

			/// <summary>
			/// Иницилизация аттрибутов для AutoCad классификатора
			/// </summary>
			/// <param name="code">Код класса</param>
			/// <param name="geomType">Тип геометрии</param>
			public AttributeClassItem(string code, string geomType)
			{
				Code = code;
				CodeInt32 = int.Parse(code);
				GeomType = geomType;
			}
		}

		/// <summary>
		/// Описание типа поля 
		/// </summary>
		public class AttributeFieldType
		{
			public bool isDecimal;

			public int Precision;

			public string Text;

			public string Type;

			public int Width;
		}

		/// <summary>
		/// Описание аттрибутов для AutoCad классификатора
		/// </summary>
		public class AttributeGroup
		{
			public readonly List<AttributeClassItem> Items = new List<AttributeClassItem>();

			/// <summary>
			/// Получить атрибуты объекта из группы атрибутов
			/// </summary>
			/// <param name="classId">
			/// The class Id.
			/// </param>
			/// <param name="geomType">
			/// The geom Type.
			/// </param>
			/// <returns>
			/// The <see cref="AttributeClassItem"/>.
			/// </returns>
			public AttributeClassItem GetItem(string classId, string geomType)
			{
				int code;

				// Сравнение кодов как численных значений
				// Зачем - непонятно :|
				var integerCompare = int.TryParse(classId, out code);

				if (integerCompare)
				{
					var item =
						Items.FirstOrDefault(
							x => string.Compare(x.GeomType, geomType, StringComparison.OrdinalIgnoreCase) == 0
								&& x.CodeInt32 == code);
					if (item != null)
						return item;
				}

				return null;
			}
		}

		/// <summary>
		/// Описание правила для атрибута
		/// </summary>
		public class AttributeRules
		{
			/// <summary>
			/// Описание домена
			/// </summary>
			public List<DomainCodeDef> CodedDomain = new List<DomainCodeDef>();

			/// <summary>
			/// Обязательность атрибута
			/// </summary>
			public string Requried = string.Empty;
		}

		/// <summary>
		/// Описание структуры. XML таг : SymbolDef. 
		/// Содержит описание структуры для конкретного геометрического типа 
		/// </summary>
		public class DataLayer
		{
			/// <summary>
			/// Комментарий
			/// </summary>
			public readonly string Comment = string.Empty;

			/// <summary>
			/// Описание слоя
			/// </summary>
			public readonly string Description = string.Empty;

			/// <summary>
			/// Тип геометрии
			/// </summary>
			public readonly string Geom_type = string.Empty;

			/// <summary>
			/// Индентификатор слоя
			/// </summary>
			public readonly string Layer_id = string.Empty;

			/// <summary>
			/// Имя слоя
			/// </summary>
			public readonly string Name = string.Empty;

			/// <summary>
			/// Статус
			/// </summary>
			public readonly string Status = string.Empty;

			/// <summary>
			/// Имя таблицы слоя
			/// </summary>
			public readonly string Table_name = string.Empty;

			/// <summary>
			/// Порядок отображения
			/// </summary>
			public readonly string Z_order = string.Empty;

			/// <summary>
			/// Атрибуты слоя
			/// </summary>
			public List<Attribute> Attributes = new List<Attribute>();

			/// <summary>
			/// Описание оформления текста
			/// </summary>
			public AutoCadAnnotationDef AutoCadAnnotation;

			/// <summary>
			/// Описание оформления слоя
			/// </summary>
			public AutoCadAppearanceDef AutoCadAppearance;

			/// <summary>
			/// Описание оформления штриховки
			/// </summary>
			public AutoCadBrushDef AutoCadBrush;

			/// <summary>
			/// Является ли слой только для подписей
			/// </summary>
			public bool IsLabel = false;

			/// <summary>
			/// Иницилизация описания слоя
			/// </summary>
			/// <param name="layerId">Индентификатор слоя</param>
			/// <param name="name">Имя слоя</param>
			/// <param name="tableName">Имя таблицы слоя</param>
			/// <param name="description">Описание слоя</param>
			/// <param name="geomType">Тип геометрии</param>
			/// <param name="zorder">Порядок отображения</param>
			/// <param name="comment">Комментарий</param>
			/// <param name="status">Статус</param>
			public DataLayer(
				string layerId,
				string name,
				string tableName,
				string description,
				string geomType,
				string zorder,
				string comment,
				string status)
			{
				Layer_id = layerId;
				Name = name;
				Table_name = tableName;
				Description = description;
				Geom_type = geomType;
				Z_order = zorder;
				Comment = comment;
				Status = status;
			}

			/// <summary>
			/// Описание оформления текста
			/// </summary>
			public class AutoCadAnnotationDef
			{
				public double TextHeight;
			}

			/// <summary>
			/// Описание оформления слоя
			/// </summary>
			public class AutoCadAppearanceDef
			{
				/// <summary>
				/// Цвет
				/// </summary>
				public int Color;

				/// <summary>
				/// Вес линии
				/// </summary>
				public double LineWeight;

				/// <summary>
				/// Глобальная толщина полилинии 
				/// </summary>
				public double LineWidth;

				/// <summary>
				/// Тип линии
				/// </summary>
				public string Pattern;
			}

			/// <summary>
			/// Описание оформления штриховки
			/// </summary>
			public class AutoCadBrushDef
			{
				public int Color;

				public string Pattern;

				public double Scale;
			}
		}

		/// <summary>
		/// Описание возможных значений для конкретного поля
		/// </summary>
		public class DomainCodeDef
		{
			public readonly string Code;

			public readonly string Name;

			public DomainCodeDef(string code, string name)
			{
				Name = name;
				Code = code;
			}
		}

		/// <summary>
		/// Класс объекта.
		/// Может содержать описание для разных геометрических типов для разных масштабов.
		/// </summary>
		public class ItemDescription
		{
			/// <summary>
			/// Описание площедного объекта
			/// </summary>
			public readonly GeomType Area = new GeomType();

			/// <summary>
			/// Наименование слоя камерального варианта
			/// </summary>
			public readonly string CameralDatalayer;

			/// <summary>
			/// Хэш кода классификатора.
			/// </summary>
			public readonly int HashCode;

			/// <summary>
			/// Индентификатор класса
			/// </summary>
			public readonly string ItemId;

			/// <summary>
			/// Описание линии
			/// </summary>
			public readonly GeomType Line = new GeomType();

			/// <summary>
			/// Код класса верхней бровки откоса
			/// </summary>
			public readonly string LineBottomId;

			/// <summary>
			/// Код класса нижней бровки откоса
			/// </summary>
			public readonly string LineTopId;

			/// <summary>
			/// Описание объекта не имеющего геометрию
			/// </summary>
			public readonly GeomType NoGeometry = new GeomType();

			/// <summary>
			/// Описание точки
			/// </summary>
			public readonly GeomType Point = new GeomType();

			/// <summary>
			/// Статус
			/// </summary>
			public readonly string Status;

			/// <summary>
			/// Тип класса
			/// </summary>
			public readonly TypeItemDescription Type = TypeItemDescription.Simple;

			/// <summary>
			/// Описание объекта с неизвестным типом геометрии
			/// </summary>
			public readonly GeomType Unknown = new GeomType();

			/// <summary>
			/// Иницилизация описания класса
			/// </summary>
			/// <param name="nameDesc">Имя класса</param>
			/// <param name="itemId">Индентификатор класса</param>
			/// <param name="code">Код класса</param>
			/// <param name="status">Статус класса</param>
			/// <param name="lineBottomId">Код нижней броки обрыва</param>
			/// <param name="lineTopId">Код верхней броки обрыва</param>
			/// <param name="cameralDatalayer">Наименование слоя камерального варианта</param>
			/// <param name="typeClass">Тип класса</param>
			public ItemDescription(
				string nameDesc,
				string itemId,
				string code,
				string status,
				string lineBottomId,
				string lineTopId,
				string cameralDatalayer,
				string typeClass)
			{
				Name = nameDesc;
				ItemId = itemId;
				Code = code;
				HashCode = code.GetHashCode();

				//CodeInt64 = Int64.Parse(code);
				Status = status;
				LineBottomId = lineBottomId;
				LineTopId = lineTopId;
				CameralDatalayer = cameralDatalayer;
				Type = parseType(typeClass);
			}

			/// <summary>
			/// Код класса
			/// </summary>
			public string Code { get; }

			/// <summary>
			/// Имя класса
			/// </summary>
			public string Name { get; }

			public string VisualName
			{
				get
				{
					return Code + " - " + Name;
				}
			}

			/// <summary>
			/// Разбирает тип класса
			/// </summary>
			/// <param name="type">
			/// The type.
			/// </param>
			/// <returns>
			/// The <see cref="TypeItemDescription"/>.
			/// </returns>
			private TypeItemDescription parseType(string type)
			{
				if (string.Equals(type, "breakage", StringComparison.OrdinalIgnoreCase))
					return TypeItemDescription.Steep;
				return TypeItemDescription.Simple;
			}

			/// <summary>
			/// Описание типа геометрии в разных масштабах
			/// </summary>
			public class GeomType
			{
				public readonly List<RepresentationTemplate> RepresentationTemplates = new List<RepresentationTemplate>();
			}
		}

		/// <summary>
		/// Стиль подписи. XML таг : LabelStyle. 
		/// Содержит описание оформления подписи
		/// </summary>
		public class LabelStyle
		{
			public readonly string Name;

			public readonly string Status;

			public readonly string Storage;

			public readonly string Usage;

			public DataLayer DataLayer;

			public SymbolDef SymbolDef;

			public SymbolDef SymbolDefArcMap;

			public SymbolDef SymbolDefAutoCad;

			public LabelStyle(string name, string storage, string usage, string status)
			{
				Name = name;
				Storage = storage;
				Usage = usage;
				Status = status;
			}

			public void setDataLayer(
				string layerId,
				string name,
				string tableName,
				string description,
				string geomType,
				string zorder,
				string comment,
				string status)
			{
				DataLayer = new DataLayer(layerId, name, tableName, description, geomType, zorder, comment, status);
			}

			public void setSymbolDef(string platform, string definition)
			{
				if (string.Equals(platform, "MAPINFO", StringComparison.OrdinalIgnoreCase))
					SymbolDef = new SymbolDef(platform, definition);
				if (string.Equals(platform, "ARCGIS", StringComparison.OrdinalIgnoreCase))
					SymbolDefArcMap = new SymbolDef(platform, definition);
				if (string.Equals(platform, "AUTOCAD", StringComparison.OrdinalIgnoreCase))
					SymbolDefAutoCad = new SymbolDef(platform, definition);
			}
		}

		/// <summary>
		/// Представление для конкретного масштаба. XML таг : RepresentationTemplate. 
		/// Содержит описание подтипов для конкретного масштаба
		/// </summary>
		public class RepresentationTemplate
		{
			/// <summary>
			/// Описание пердставления
			/// </summary>
			public readonly string Descriprtion;

			/// <summary>
			/// Имя представления
			/// </summary>
			public readonly string Name;

			public readonly ParentTags Parent;

			/// <summary>
			/// Подтипы
			/// </summary>
			public readonly List<Subtype> Subtypes;

			/// <summary>
			/// Иницилизация представления
			/// </summary>
			/// <param name="name">Имя представления</param>
			public RepresentationTemplate(string name)
			{
				Name = name;
			}

			/// <summary>
			/// Иницилизация представления
			/// </summary>
			/// <param name="name">
			/// Имя представления
			/// </param>
			/// <param name="descriprtion">
			/// Описание пердставления
			/// </param>
			/// <param name="subtypes">
			/// Подтипы
			/// </param>
			/// <param name="itemDesc">
			/// The item Desc.
			/// </param>
			public RepresentationTemplate(string name, string descriprtion, List<Subtype> subtypes, ItemDescription itemDesc)
			{
				Name = name;
				Descriprtion = descriprtion;
				Subtypes = subtypes;

				// Установка родительского класса
				Parent.ItemDescription = itemDesc;
				foreach (var subtype in Subtypes)
					subtype.Parent.RepresentationTemplate = this;
			}

			/// <summary>
			/// Ссылка на родительский класс
			/// </summary>
			public struct ParentTags
			{
				public ItemDescription ItemDescription;
			}
		}

		/// <summary>
		/// Секция. XML таг : ClassSection. 
		/// Содержит в себе таги ClassItem.
		/// </summary>
		public class Section
		{
			/// <summary>
			/// Классы в секции
			/// </summary>
			public readonly Dictionary<string, ItemDescription> Items = new Dictionary<string, ItemDescription>();

			/// <summary>
			/// Секции могут быть вложены друг в друга. 
			/// ParentSection - ссылка на секцию верхнего уровня.
			/// </summary>
			public readonly Section ParentSection;

			/// <summary>
			/// Подсекции
			/// </summary>
			public readonly List<Section> Subsections = new List<Section>();

			/// <summary>
			/// Иницилизация секции (конструктор)
			/// </summary>
			/// <param name="name">
			/// Имя секции
			/// </param>
			/// <param name="parentSection">
			/// Ссылка на секцию верхнего уровня (родитель)
			/// </param>
			public Section(string name, Section parentSection = null)
			{
				Name = name;
				ParentSection = parentSection;
			}

			public List<ItemDescription> ItemDescriptions
			{
				get
				{
					return Items.Select(i => i.Value).ToList();
				}
			}

			/// <summary>
			/// Имя секции
			/// </summary>
			public string Name { get; }

			/// <summary>
			/// Добавление класса
			/// </summary>
			/// <param name="name">
			/// Имя класса
			/// </param>
			/// <param name="itemId">
			/// Индентификатор класса
			/// </param>
			/// <param name="code">
			/// Код класса
			/// </param>
			/// <param name="status">
			/// Статус
			/// </param>
			/// <param name="lineBottomId">
			/// Код нижней бровки откоса. Может быть null
			/// </param>
			/// <param name="lineTopId">
			/// Код верхней бровки откоса. Может быть null
			/// </param>
			/// <param name="cameralDatalayer">
			/// Наименование слоя камерального варианта
			/// </param>
			/// <param name="typeClass">
			/// Тип класса
			/// </param>
			/// <returns>
			/// The <see cref="ItemDescription"/>.
			/// </returns>
			public ItemDescription AddItem(
				string name,
				string itemId,
				string code,
				string status,
				string lineBottomId,
				string lineTopId,
				string cameralDatalayer,
				string typeClass)
			{
				ItemDescription item;
				if (!Items.TryGetValue(code, out item))
				{
					item = new ItemDescription(name, itemId, code, status, lineBottomId, lineTopId, cameralDatalayer, typeClass);
					Items.Add(code, item);
					return item;
				}

				return item;
			}

			/// <summary>
			/// Добавление класса
			/// </summary>
			/// <param name="item">
			/// The item.
			/// </param>
			public void AddItem(ItemDescription item)
			{
				if (!Items.ContainsKey(item.Code))
					Items.Add(item.Code, item);
			}

			/// <summary>
			/// Добавление классов
			/// </summary>
			/// <param name="items">
			/// The items.
			/// </param>
			public void AddItems(List<ItemDescription> items)
			{
				foreach (var item in items)
					AddItem(item);
			}

			/// <summary>
			/// Добавление подсекции
			/// </summary>
			/// <param name="name">
			/// The name.
			/// </param>
			public void AddSubsection(string name)
			{
				Subsections.Add(new Section(name));
			}
		}

		/// <summary>
		/// Подтип объекта. XML таг : Subtype. 
		/// Содержит описание подтипа объекта
		/// </summary>
		public class Subtype
		{
			/// <summary>
			/// Фильтр
			/// </summary>
			public readonly string Filter;

			/// <summary>
			/// Имя подтипа
			/// </summary>
			public readonly string Name;

			/// <summary>
			/// Описание оформления
			/// </summary>
			public readonly List<SymbolStyle> SymbolStyle = new List<SymbolStyle>();

			/// <summary>
			/// Описание слоя камерального варианта
			/// </summary>
			public DataLayer CameralDataLayer;

			/// <summary>
			/// Описание слоя
			/// </summary>
			public DataLayer DataLayer;

			/// <summary>
			/// Описание стилей подписей
			/// </summary>
			public Dictionary<string, LabelStyle> LabelStyles = new Dictionary<string, LabelStyle>();

			public ParentTags Parent;

			public Subtype(string name, string filter)
			{
				Name = name;
				Filter = filter;
			}

			public SymbolStyle GetSymbolStyle(string geomType)
			{
				return SymbolStyle.FirstOrDefault(sym => string.Equals(sym.GeomType, geomType, StringComparison.OrdinalIgnoreCase));
			}

			/// <summary>
			/// Ссылка на родительский класс
			/// </summary>
			public struct ParentTags
			{
				public RepresentationTemplate RepresentationTemplate;
			}
		}

		/// <summary>
		/// Оформление объекта. XML таг : SymbolDef. 
		/// </summary>
		public class SymbolDef
		{
			public readonly string Definition;

			public readonly string Platform;

			/// <summary>
			/// Описание оформления заливки ArcMap
			/// </summary>
			public ParserDisplayStyle.AreaArcMap AreaArcMap;

			/// <summary>
			/// Описание оформления заливки MapInfo
			/// </summary>
			public ParserDisplayStyle.Brush Brush;

			/// <summary>
			/// Описание оформления заливки AutoCAD
			/// </summary>
			public ParserDisplayStyle.BrushAutoCad BrushAutoCad;

			/// <summary>
			/// Описание оформления условного знака AutoCAD
			/// </summary>
			public ParserDisplayStyle.COGOPointAutoCad COGOPointAutoCad;

			/// <summary>
			/// Описание оформления подписи MapInfo
			/// </summary>
			public ParserDisplayStyle.Font Font;

			/// <summary>
			/// Описание оформления условного знака MapInfo
			/// </summary>
			public ParserDisplayStyle.FontSymbol FontSymbol;

			/// <summary>
			/// Описание оформления линии ArcMap
			/// </summary>
			public ParserDisplayStyle.LineArcMap LineArcMap;

			/// <summary>
			/// Описание оформления линии AutoCAD
			/// </summary>
			public ParserDisplayStyle.LineAutoCad LineAutoCad;

			/// <summary>
			/// Описание оформления линии MapInfo
			/// </summary>
			public ParserDisplayStyle.Pen Pen;

			/// <summary>
			/// Описание оформления условного знака ArcMap
			/// </summary>
			public ParserDisplayStyle.PointArcMap PointArcMap;

			/// <summary>
			/// Initializes a new instance of the <see cref="SymbolDef"/> class.
			/// </summary>
			/// <param name="platform">
			/// Платформа, для которой написан стиль.
			/// </param>
			/// <param name="definition">
			/// Описание стиля.
			/// </param>
			public SymbolDef(string platform, string definition)
			{
				Platform = platform;
				Definition = definition;
				if (string.Equals(platform, "MAPINFO", StringComparison.OrdinalIgnoreCase))
					ParseDefinitionMapInfo(definition);
				else if (string.Equals(platform, "AUTOCAD", StringComparison.OrdinalIgnoreCase))
					ParseDefinitionAutoCad(definition);
				else if (string.Equals(platform, "ARCGIS", StringComparison.OrdinalIgnoreCase))
					ParseDefinitionArcMap(definition);
			}

			/// <summary>
			/// Разбирает строку описания стиля для ArcMap
			/// </summary>
			/// <param name="definition">
			/// The definition.
			/// </param>
			private void ParseDefinitionArcMap(string definition)
			{
				definition = definition.ToLower();
				if (definition.Contains("linestyle"))
					ParserDisplayStyle.Parse(definition, ref LineArcMap);
				else if (definition.Contains("markerstyle"))
					ParserDisplayStyle.Parse(definition, ref PointArcMap);
				else if (definition.Contains("fillstyle"))
					ParserDisplayStyle.Parse(definition, ref AreaArcMap);
				else if (string.IsNullOrEmpty(definition))
					throw new KeyNotFoundException("Неизвестный стиль '" + definition + "'.");
			}

			/// <summary>
			/// Разбирает строку описания стиля для AutoCAD
			/// </summary>
			/// <param name="definition">
			/// The definition.
			/// </param>
			private void ParseDefinitionAutoCad(string definition)
			{
				var parts = definition.Trim().Split(';');
				foreach (var part in parts)
					if (part.StartsWith("Line") || part.StartsWith("Mline"))
						ParserDisplayStyle.Parse(part, ref LineAutoCad);
					else if (part.StartsWith("Brush"))
						ParserDisplayStyle.Parse(part, ref BrushAutoCad);
					else if (part.StartsWith("COGOPoint"))
						ParserDisplayStyle.Parse(part, ref COGOPointAutoCad);
					else if (!string.IsNullOrEmpty(part))
						throw new KeyNotFoundException("Неизвестный стиль '" + part + "'.");
			}

			/// <summary>
			/// Разбирает строку описания стиля для MapInfo
			/// </summary>
			/// <param name="defenition"></param>
			private void ParseDefinitionMapInfo(string defenition)
			{
				var parts = defenition.Trim().Split(';');
				foreach (var part in parts)
					if (part.StartsWith("Brush"))
						ParserDisplayStyle.Parse(part, ref Brush);
					else if (part.StartsWith("FontSymbol"))
						ParserDisplayStyle.Parse(part, ref FontSymbol);
					else if (part.StartsWith("Pen"))
						ParserDisplayStyle.Parse(part, ref Pen);
					else if (part.StartsWith("Font"))
						ParserDisplayStyle.Parse(part, ref Font);
					else if (!string.IsNullOrEmpty(part))
						throw new KeyNotFoundException("Неизвестный стиль '" + part + "'.");
			}
		}

		/// <summary>
		/// Описание стиля объекта. XML таг : SymbolStyle. 
		/// </summary>
		public class SymbolStyle
		{
			public AnnotationDef AnnotationDefAutoCad;

			public string GeomType = string.Empty;

			public string Status = string.Empty;

			public SymbolDef SymbolDef;

			public SymbolDef SymbolDefArcMap;

			public SymbolDef SymbolDefAutoCad;

			public void SetAnnotationDefAutoCad(string tableName, string colorByFeature)
			{
				AnnotationDefAutoCad = new AnnotationDef(tableName, bool.Parse(colorByFeature));
			}

			public void SetSymbolDef(string platform, string definition)
			{
				if (string.Equals(platform, "MAPINFO", StringComparison.OrdinalIgnoreCase))
					SymbolDef = new SymbolDef(platform, definition);
				if (string.Equals(platform, "ARCGIS", StringComparison.OrdinalIgnoreCase))
					SymbolDefArcMap = new SymbolDef(platform, definition);
				if (string.Equals(platform, "AUTOCAD", StringComparison.OrdinalIgnoreCase))
					SymbolDefAutoCad = new SymbolDef(platform, definition);
			}
		}

		#region Вспомогательные свойства структуры

		/// <summary>
		/// Все описания таблиц в классификаторе
		/// </summary>
		public List<DataLayer> DataLayers = new List<DataLayer>();

		/// <summary>
		/// Все заданные масштабы
		/// </summary>
		public List<string> Scales = new List<string>();

		/// <summary>
		/// Все заданные описания масштабов
		/// </summary>
		public List<string> ScalesDescription = new List<string>();

		/// <summary>
		/// Все аттрибуты в AutoCad классификаторе
		/// </summary>
		public List<AttributeGroup> AttributeGroups = new List<AttributeGroup>();

		#endregion
	}
}