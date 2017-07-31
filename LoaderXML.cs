using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace ClassifierCore
{
	using System.IO;

	/// <summary>
	/// Загрузчик классификатора
	/// </summary>
	public class LoaderXML
	{
		/// <summary>
		/// При любой ошибке выдавать исключение
		/// </summary>
		[Obsolete("Теперь классификатор ведет себя так, как будто значение всегда выставлено в true.", true)]
		public static bool CriticalLoad = true;

		/// <summary>
		/// Количество секций. Используется для отображения прогресса.
		/// </summary>
		public volatile int NumSection = 0;

		/// <summary>
		/// Текущая секция. Используется для отображения прогресса.
		/// </summary>
		public volatile int CurSection = 0;

		/// <summary>
		/// Наименование текущей секции. Используется для отображения прогресса.
		/// </summary>
		public volatile string CurSectionName = string.Empty;

		/// <summary>
		/// Документ XML файла
		/// </summary>
		private XmlDocument _xmlDocument;

		/// <summary>
		/// Базовый навигатор
		/// </summary>
		private XPathNavigator _navigator;

		#region Открытые методы

		/// <summary>
		/// Загрузка классификатора из XML файла в класс ClassifierStruct
		/// </summary>
		public ClassifierStruct loadAutoCad(string pathXml)
		{
			return loadClassifier(pathXml, "AutoCad");
		}

		/// <summary>
		/// Загрузка классификатора из XML файла в класс ClassifierStruct
		/// </summary>
		public ClassifierStruct load(string pathXml)
		{
			return loadClassifier(pathXml, "*");
		}

		/// <summary>
		/// Загрука классификатора из XML потока в класс <see cref="ClassifierStruct"/>.
		/// </summary>
		/// <param name="reader">
		/// <see cref="TextReader"/> потока, используется для управления кодировкой потока.
		/// </param>
		/// <returns>
		/// The <see cref="ClassifierStruct"/>.
		/// </returns>
		public ClassifierStruct load(TextReader reader)
		{
			return loadClassifier(reader, "*");
		}

		/// <summary>
		/// Загрузка классификатора из XML файла в класс ClassifierStruct
		/// </summary>
		public ClassifierStruct loadClassifier(string pathXml, string platform)
		{
			Open(pathXml);

			return this.ReadDocument(platform);
		}

		/// <summary>
		/// Загрука классификатора из XML потока в класс <see cref="ClassifierStruct"/>.
		/// </summary>
		/// <param name="reader">
		/// <see cref="TextReader"/> потока, используется для управления кодировкой потока.
		/// </param>
		/// <param name="platform">
		/// The platform.
		/// </param>
		/// <returns>
		/// The <see cref="ClassifierStruct"/>.
		/// </returns>
		public ClassifierStruct loadClassifier(TextReader reader, string platform)
		{
			Open(reader);

			return this.ReadDocument(platform);
		}

		public HashSet<string> getClassifierScales(string pathXml)
		{
			var scales = new HashSet<string>();
			Open(pathXml);

			XPathNodeIterator representationTemplates = _navigator.Select("//RepresentationTemplate");
			foreach (XPathNavigator representationTemplate in representationTemplates)
			{
				string contents = representationTemplate.GetAttribute("name", string.Empty);
				scales.Add(contents);
			}

			return scales;
		}

		public string getClassifierVersion(string pathXml)
		{
			string version = string.Empty;
			Open(pathXml);

			XPathNodeIterator representationTemplates = _navigator.Select("//RNGIS_DataSet");
			foreach (XPathNavigator representationTemplate in representationTemplates)
			{
				version = representationTemplate.GetAttribute("version", string.Empty);
			}

			if (string.IsNullOrEmpty(version)) throw new Exception("Невозможно получить версию классификатора.");

			return version;
		}

		public string getClassifierName(string pathXml)
		{
			string name = string.Empty;
			Open(pathXml);

			XPathNodeIterator rngisDataSets = _navigator.Select("//RNGIS_DataSet");
			foreach (XPathNavigator rngisDataSet in rngisDataSets)
			{
				name = rngisDataSet.GetAttribute("name", string.Empty);
				break;
			}

			return name;
		}


		#endregion

		/// <summary>
		/// Получение атрибутов объекта
		/// </summary>
		/// <param name="node">
		/// The node.
		/// </param>
		/// <param name="nameAttr">
		/// The name Attr.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private static string GetAttrNode(XmlNode node, string nameAttr)
		{
			if (node.Attributes != null && node.Attributes[nameAttr] != null) return node.Attributes[nameAttr].Value ?? string.Empty;
			return string.Empty;
		}

		private static void AddAttributes(XPathNavigator dataLayerPath, ClassifierStruct.DataLayer dataLayer, string itemId)
		{
			// Получение атрибутов и правил атрибутов
			XPathNodeIterator attributesLayerPaths = dataLayerPath.Select("./Attribute");
			foreach (XPathNavigator attributeLayerPath in attributesLayerPaths)
			{
				var attribute =
					new ClassifierStruct.Attribute(
						attributeLayerPath.GetAttribute("name", string.Empty),
						attributeLayerPath.GetAttribute("field_name", string.Empty),
						attributeLayerPath.GetAttribute("field_type", string.Empty),
						attributeLayerPath.GetAttribute("comment", string.Empty),
						attributeLayerPath.GetAttribute("status", string.Empty));

				// Получение правил атрибутов                
				XPathNodeIterator attributesRulesPaths =
					dataLayerPath.Select(
						"./ItemRules[@item_id='" + itemId + "']/AttributeRule[@field_name='" + attribute.Field_name + "']");
				foreach (XPathNavigator attributeRulePath in attributesRulesPaths)
				{
					if (attributeRulePath.GetAttribute("required", string.Empty) != string.Empty)
						attribute.AttributeRules.Requried = attributeRulePath.GetAttribute("required", string.Empty);

					//Проверка домена
					XPathNodeIterator codeDefsPaths = attributeRulePath.Select("./CodedDomain/CodeDef");
					foreach (XPathNavigator codeDefPath in codeDefsPaths)
					{
						attribute.AttributeRules.CodedDomain.Add(
							new ClassifierStruct.DomainCodeDef(
								codeDefPath.GetAttribute("code", string.Empty), codeDefPath.GetAttribute("name", string.Empty)));
					}
				}

				// Добавление атрибута в DataLayer
				dataLayer.Attributes.Add(attribute);
			}
		}

		/// <summary>
		/// Получить описание слоя
		/// </summary>
		/// <param name="dataLayersPaths">
		/// The data Layers Paths.
		/// </param>
		/// <param name="itemId">
		/// The item Id.
		/// </param>
		/// <returns>
		/// The <see cref="ClassifierStruct.DataLayer"/>.
		/// </returns>
		private static ClassifierStruct.DataLayer GetDataLayer(XPathNodeIterator dataLayersPaths, string itemId)
		{
			ClassifierStruct.DataLayer dataLayer = null;
			foreach (XPathNavigator dataLayerPath in dataLayersPaths)
			{
				dataLayer = new ClassifierStruct.DataLayer(
					dataLayerPath.GetAttribute("layer_id", string.Empty),
					dataLayerPath.GetAttribute("name", string.Empty),
					dataLayerPath.GetAttribute("table_name", string.Empty),
					dataLayerPath.GetAttribute("description", string.Empty),
					dataLayerPath.GetAttribute("geom_type", string.Empty),
					dataLayerPath.GetAttribute("z_order", string.Empty),
					dataLayerPath.GetAttribute("comment", string.Empty),
					dataLayerPath.GetAttribute("status", string.Empty));

				XPathNodeIterator appearancesPaths = dataLayerPath.Select("./Appearance");
				foreach (XPathNavigator appearancePath in appearancesPaths)
				{
					try
					{
						dataLayer.AutoCadAppearance = new ClassifierStruct.DataLayer.AutoCadAppearanceDef();

						// Получаем цвет
						string appereanceColor = appearancePath.GetAttribute("color", string.Empty);

						if (!int.TryParse(appereanceColor, out dataLayer.AutoCadAppearance.Color))
							dataLayer.AutoCadAppearance.Color = int.MinValue;

						// Получаем стиль линии
						dataLayer.AutoCadAppearance.Pattern = appearancePath.GetAttribute("pattern", string.Empty);

						// Получаем вес линии
						string appereanceLineWeight = appearancePath.GetAttribute("line_weight", string.Empty);
						if (appereanceLineWeight != string.Empty && appereanceLineWeight.ToLower() != "none")
							dataLayer.AutoCadAppearance.LineWeight = double.Parse(appereanceLineWeight, System.Globalization.CultureInfo.InvariantCulture);
						else
							dataLayer.AutoCadAppearance.LineWeight = double.MinValue;

						// Получаем глобальную толщину полилинии
						string appereanceLineWidth = appearancePath.GetAttribute("line_width", string.Empty);
						if (string.IsNullOrEmpty(appereanceLineWidth) || string.Equals(appereanceLineWidth, "none", StringComparison.OrdinalIgnoreCase))
							dataLayer.AutoCadAppearance.LineWidth = double.NaN;
						else
							dataLayer.AutoCadAppearance.LineWidth = double.Parse(appereanceLineWidth, System.Globalization.CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						throw new Exception("Неправильные аттрибуты для AutoCad Appearance:" + ex.Message);
					}
				}

				XPathNodeIterator annotationsPaths = dataLayerPath.Select("./Annotation");
				foreach (XPathNavigator annotationPath in annotationsPaths)
				{
					dataLayer.AutoCadAnnotation = new ClassifierStruct.DataLayer.AutoCadAnnotationDef();
					string textHeight = annotationPath.GetAttribute("text_height", string.Empty);
					if (textHeight == string.Empty) dataLayer.AutoCadAnnotation.TextHeight = double.MinValue;
					else
						dataLayer.AutoCadAnnotation.TextHeight = double.Parse(
							annotationPath.GetAttribute("text_height", string.Empty), System.Globalization.CultureInfo.InvariantCulture);
				}

				AddAttributes(dataLayerPath, dataLayer, itemId);
			}

			return dataLayer;
		}

		/// <summary>
		/// Загрузка классов в секции
		/// </summary>
		/// <param name="classSectionPath">
		/// The class Section Path.
		/// </param>
		/// <param name="section">
		/// The section.
		/// </param>
		/// <param name="platform">
		/// The platform.
		/// </param>
		private void FillClassItems(
			XPathNavigator classSectionPath,
			ClassifierStruct.Section section,
			string platform)
		{
			XPathNodeIterator classItems = classSectionPath.Select("./ClassItem");
			foreach (XPathNavigator classItem in classItems)
			{
				string itemName = classItem.GetAttribute("name", string.Empty);
				string itemId = classItem.GetAttribute("item_id", string.Empty);
				string code = classItem.GetAttribute("code", string.Empty);

				string status = classItem.GetAttribute("status", string.Empty);
				string lineBottomId = classItem.GetAttribute("line_bottom_id", string.Empty);
				string lineTopId = classItem.GetAttribute("line_top_id", string.Empty);
				string typeClass = classItem.GetAttribute("type", string.Empty);
				string cameralDatalayer = classItem.GetAttribute("cameral_datalayer", string.Empty);
				var item = section.AddItem(itemName, itemId, code, status, lineBottomId, lineTopId, cameralDatalayer, typeClass);

				XPathNodeIterator representationTemplates;

				if (platform == "AutoCad")
				{
					representationTemplates =
						_navigator.Select(
							"/RNGIS_DataSet/DataStorage/DataLayer/ItemRules[@item_id='" + itemId + "']/RepresentationTemplate");
				}
				else
				{
					representationTemplates =
						_navigator.Select(
							"/RNGIS_DataSet/DataStorage/DataLayer/ItemRules[@item_id='" + itemId + "']/RepresentationTemplate");
				}

				foreach (XPathNavigator representationTemplate in representationTemplates)
				{
					string templateName = representationTemplate.GetAttribute("name", string.Empty);
					string templateDesc = representationTemplate.GetAttribute("descriprtion", string.Empty); //Bug in classifier
					if (string.IsNullOrEmpty(templateDesc)) templateDesc = representationTemplate.GetAttribute("description", string.Empty);

					List<ClassifierStruct.Subtype> subtypes = FillSubtypes(representationTemplate, item);
					switch (subtypes[0].DataLayer.Geom_type)
					{
						case "point":
							item.Point.RepresentationTemplates.Add(new ClassifierStruct.RepresentationTemplate(templateName, templateDesc, subtypes, item));
							break;
						case "line":
							item.Line.RepresentationTemplates.Add(new ClassifierStruct.RepresentationTemplate(templateName, templateDesc, subtypes, item));
							break;
						case "area":
							item.Area.RepresentationTemplates.Add(new ClassifierStruct.RepresentationTemplate(templateName, templateDesc, subtypes, item));
							break;
						case "nogeometry":
							item.NoGeometry.RepresentationTemplates.Add(new ClassifierStruct.RepresentationTemplate(templateName, templateDesc, subtypes, item));
							break;
						default:
							if (string.IsNullOrEmpty(subtypes[0].DataLayer.Geom_type))
								item.Unknown.RepresentationTemplates.Add(new ClassifierStruct.RepresentationTemplate(templateName, templateDesc, subtypes, item));
							else throw new Exception("Неправильный тип геометрии '" + subtypes[0].DataLayer.Geom_type + "'.");
							break;
					}
				}
			}
		}

		/// <summary>
		/// Заполнение подтипов объекта
		/// </summary>
		/// <param name="representationTemplate">
		/// The representation Template.
		/// </param>
		/// <param name="itemDesc">
		/// The item Desc.
		/// </param>
		/// <returns>
		/// The <see cref="List"/>.
		/// </returns>
		private List<ClassifierStruct.Subtype> FillSubtypes(
			XPathNavigator representationTemplate,
			ClassifierStruct.ItemDescription itemDesc)
		{
			var subtypes = new List<ClassifierStruct.Subtype>();

			XPathNodeIterator subtypesPaths = representationTemplate.Select("./Subtype");
			foreach (XPathNavigator subtypePath in subtypesPaths)
			{
				var subtype = new ClassifierStruct.Subtype(
					subtypePath.GetAttribute("name", string.Empty),
					subtypePath.GetAttribute("filter", string.Empty));

				// Установить статус для SymbolStyle
				XPathNodeIterator symbolStylePaths = subtypePath.Select("./SymbolStyle");
				foreach (XPathNavigator symbolStylePath in symbolStylePaths)
				{
					var symbolStyle = new ClassifierStruct.SymbolStyle();
					symbolStyle.Status = symbolStylePath.GetAttribute("status", string.Empty);

					// Данный атрибут содержится только в AutoCAD классификаторе
					symbolStyle.GeomType = symbolStylePath.GetAttribute("geom_type", string.Empty);

					// Установить SymbolDef для SymbolStyle
					XPathNodeIterator symbolDefsPaths = symbolStylePath.Select("./SymbolDef");
					foreach (XPathNavigator symbolDefPath in symbolDefsPaths)
					{
						symbolStyle.SetSymbolDef(
							symbolDefPath.GetAttribute("platform", string.Empty),
							symbolDefPath.GetAttribute("definition", string.Empty));
					}

					// Установить AnnotationDefAutoCad для SymbolStyle
					XPathNodeIterator annotationDefAutoCad = symbolStylePath.Select("./Annotation");
					foreach (XPathNavigator annotationDefAutoCadPath in annotationDefAutoCad)
					{
						symbolStyle.SetAnnotationDefAutoCad(
							annotationDefAutoCadPath.GetAttribute("table_name", string.Empty),
							annotationDefAutoCadPath.GetAttribute("colorByFeature", string.Empty));
					}

					subtype.SymbolStyle.Add(symbolStyle);
				}

				// Установить описание слоя
				XPathNodeIterator dataLayersPaths = representationTemplate.Select("parent::*/parent::*");
				subtype.DataLayer = GetDataLayer(dataLayersPaths, itemDesc.ItemId);

				// Установить описание слоя для камерального варианта
				if (!string.IsNullOrEmpty(itemDesc.CameralDatalayer))
				{
					XPathNodeIterator datalayersPathsCameral =
						_navigator.Select("/RNGIS_DataSet/DataStorage/DataLayer[@layer_id='" + itemDesc.CameralDatalayer + "']");
					subtype.CameralDataLayer = GetDataLayer(datalayersPathsCameral, itemDesc.ItemId);
				}

				// Заполнение подписей и добавление их в подтипы
				FillLabelsStyles(subtypePath, subtype, itemDesc);

				// Добавление подтипа
				subtypes.Add(subtype);
			}
			
			return subtypes;
		}

		/// <summary>
		/// Заполнение подписей для объектов
		/// </summary>
		/// <param name="subtypePath">
		/// The subtype Path.
		/// </param>
		/// <param name="subtype">
		/// The subtype.
		/// </param>
		/// <param name="itemDesc">
		/// The item Desc.
		/// </param>
		private void FillLabelsStyles(
			XPathNavigator subtypePath, ClassifierStruct.Subtype subtype, ClassifierStruct.ItemDescription itemDesc)
		{
			XPathNodeIterator labelStylesPaths = subtypePath.Select("./LabelStyle");
			foreach (XPathNavigator labelStylePath in labelStylesPaths)
			{
				var labelStyle = new ClassifierStruct.LabelStyle(
					labelStylePath.GetAttribute("name", string.Empty),
					labelStylePath.GetAttribute("storage", string.Empty),
					labelStylePath.GetAttribute("usage", string.Empty),
					labelStylePath.GetAttribute("status", string.Empty));
				if (!subtype.LabelStyles.ContainsKey(labelStyle.Name)) subtype.LabelStyles.Add(labelStyle.Name, labelStyle);

				XPathNodeIterator symbolDefsPaths = labelStylePath.Select("./StyleDef");
				foreach (XPathNavigator symbolDefPath in symbolDefsPaths)
				{
					labelStyle.setSymbolDef(
						symbolDefPath.GetAttribute("platform", string.Empty), symbolDefPath.GetAttribute("definition", string.Empty));

					XPathNodeIterator labelDataLayerPaths =
						_navigator.Select("/RNGIS_DataSet/DataStorage/DataLayer[@layer_id='" + labelStyle.Storage + "']");
					foreach (XPathNavigator labelDataLayerPath in labelDataLayerPaths)
					{
						labelStyle.setDataLayer(
							labelDataLayerPath.GetAttribute("layer_id", string.Empty),
							labelDataLayerPath.GetAttribute("name", string.Empty),
							labelDataLayerPath.GetAttribute("table_name", string.Empty),
							labelDataLayerPath.GetAttribute("description", string.Empty),
							labelDataLayerPath.GetAttribute("geom_type", string.Empty),
							labelDataLayerPath.GetAttribute("z_order", string.Empty),
							labelDataLayerPath.GetAttribute("comment", string.Empty),
							labelDataLayerPath.GetAttribute("status", string.Empty));
						AddAttributes(labelDataLayerPath, labelStyle.DataLayer, itemDesc.ItemId);

						// Устанавливаем дополнительные поля
						labelStyle.DataLayer.IsLabel = true;
					}
				}
			}
		}

		/// <summary>
		/// Дополнительные члены класса классификатора AutoCad
		/// </summary>
		/// <param name="classifier">
		/// The classifier.
		/// </param>
		private void UpdateAutocadAttributes(ClassifierStruct classifier)
		{
			XmlNodeList xmlAttributeGroups = _xmlDocument.SelectNodes("//AttributeGroup");
			if (xmlAttributeGroups == null) return;

			foreach (XmlNode xmlAttributeGroup in xmlAttributeGroups)
			{
				var attributeGroup = new ClassifierStruct.AttributeGroup();

				var dataLayerAttributes = new List<ClassifierStruct.Attribute>();

				// Записываем аттрибуты
				XmlNodeList xmlAttributes = xmlAttributeGroup.SelectNodes("./Attribute");
				if (xmlAttributes != null)
				{
					foreach (XmlNode xmlAttr in xmlAttributes)
					{
						var attribute = new ClassifierStruct.Attribute(
							GetAttrNode(xmlAttr, "name"),
							GetAttrNode(xmlAttr, "field_name"),
							GetAttrNode(xmlAttr, "field_type"),
							GetAttrNode(xmlAttr, "comment"),
							GetAttrNode(xmlAttr, "status"));
						//string fieldName = this.GetAttrNode(xmlAttr, "field_name");                    
						dataLayerAttributes.Add(attribute);
					}
				}

				// Записываем все классы
				XmlNodeList xmlClassItems = xmlAttributeGroup.SelectNodes(".//ClassItem");
				if (xmlClassItems != null)
				{
					foreach (XmlNode xmlClassItem in xmlClassItems)
					{
						var itemAttrs = new List<ClassifierStruct.Attribute>();

						var classItem = new ClassifierStruct.AttributeClassItem(
							GetAttrNode(xmlClassItem, "code"), GetAttrNode(xmlClassItem, "geom_type"));

						foreach (ClassifierStruct.Attribute attribute in dataLayerAttributes)
						{
							var newAttr = new ClassifierStruct.Attribute(
								attribute.Name, attribute.Field_name, attribute.Field_type.Text, attribute.Comment, attribute.Status);

							XmlNode attrRule = xmlClassItem.SelectSingleNode("./AttributeRule[@field_name='" + attribute.Field_name + "']");
							if (attrRule != null && attrRule.Attributes != null)
							{
								if (attrRule.Attributes["required"] != null)
								{
									newAttr.AttributeRules.Requried = attrRule.Attributes["required"].Value;
								}

								// Проверка домена
								XmlNodeList codeDefsPaths = attrRule.SelectNodes("./CodedDomain/CodeDef");
								if (codeDefsPaths != null)
								{
									foreach (XmlNode codeDefPath in codeDefsPaths)
									{
										newAttr.AttributeRules.CodedDomain.Add(
											new ClassifierStruct.DomainCodeDef(
												GetAttrNode(codeDefPath, "code"), GetAttrNode(codeDefPath, "name")));
									}
								}
							}

							itemAttrs.Add(newAttr);
						}

						classItem.Attributes = itemAttrs;
						attributeGroup.Items.Add(classItem);
					}
				}

				classifier.AttributeGroups.Add(attributeGroup);
			}
		}

		/// <summary>
		/// Дополнительные члены класса классификатора
		/// </summary>
		/// <param name="classifier">
		/// The classifier.
		/// </param>
		private void UpdateDataLayers(ClassifierStruct classifier)
		{
			XPathNodeIterator datalayersPaths = _navigator.Select("/RNGIS_DataSet/DataStorage/DataLayer");
			foreach (XPathNavigator datalayerPath in datalayersPaths)
			{
				var dataLayer = new ClassifierStruct.DataLayer(
						datalayerPath.GetAttribute("layer_id", string.Empty),
						datalayerPath.GetAttribute("name", string.Empty),
						datalayerPath.GetAttribute("table_name", string.Empty),
						datalayerPath.GetAttribute("description", string.Empty),
						datalayerPath.GetAttribute("geom_type", string.Empty),
						datalayerPath.GetAttribute("z_order", string.Empty),
						datalayerPath.GetAttribute("comment", string.Empty),
						datalayerPath.GetAttribute("status", string.Empty));

				// Получение атрибутов.
				XPathNodeIterator appearancesPaths = datalayerPath.Select("./Appearance");
				foreach (XPathNavigator appearancePath in appearancesPaths)
				{
					try
					{
						dataLayer.AutoCadAppearance = new ClassifierStruct.DataLayer.AutoCadAppearanceDef();
						string appereanceColor = appearancePath.GetAttribute("color", string.Empty);

						if (!int.TryParse(appereanceColor, out dataLayer.AutoCadAppearance.Color)) dataLayer.AutoCadAppearance.Color = int.MinValue;

						dataLayer.AutoCadAppearance.Pattern = appearancePath.GetAttribute("pattern", string.Empty);
						string appereanceLineWeight = appearancePath.GetAttribute("line_weight", string.Empty);
						if (string.IsNullOrEmpty(appereanceLineWeight) || string.Equals(appereanceLineWeight, "none", StringComparison.OrdinalIgnoreCase))
						{
							dataLayer.AutoCadAppearance.LineWeight = double.MinValue;
						}
						else
						{
							dataLayer.AutoCadAppearance.LineWeight = double.Parse(
								appereanceLineWeight, System.Globalization.CultureInfo.InvariantCulture);
						}
					}
					catch (Exception ex)
					{
						throw new Exception("Не правильные аттрибуты для AutoCad Appearance.", ex);
					}
				}

				XPathNodeIterator annotationsPaths = datalayerPath.Select("./Annotation");
				foreach (XPathNavigator annotationPath in annotationsPaths)
				{
					dataLayer.AutoCadAnnotation = new ClassifierStruct.DataLayer.AutoCadAnnotationDef();
					string textHeight = annotationPath.GetAttribute("text_height", string.Empty);
					if (string.IsNullOrEmpty(textHeight)) dataLayer.AutoCadAnnotation.TextHeight = double.MinValue;
					else
						dataLayer.AutoCadAnnotation.TextHeight = double.Parse(
							annotationPath.GetAttribute("text_height", string.Empty), System.Globalization.CultureInfo.InvariantCulture);
				}

				// Мы не получаем правила для атрибутов для вспомогательного члена структуры
				XPathNodeIterator attributesLayerPaths = datalayerPath.Select("./Attribute");
				foreach (XPathNavigator attributeLayerPath in attributesLayerPaths)
				{
					var attribute =
						new ClassifierStruct.Attribute(
							attributeLayerPath.GetAttribute("name", string.Empty),
							attributeLayerPath.GetAttribute("field_name", string.Empty),
							attributeLayerPath.GetAttribute("field_type", string.Empty),
							attributeLayerPath.GetAttribute("comment", string.Empty),
							attributeLayerPath.GetAttribute("status", string.Empty));

					// Добавление атрибута в DataLayer
					dataLayer.Attributes.Add(attribute);
				}

				// Добавление DataLayer в спомогательную стуктуру
				classifier.DataLayers.Add(dataLayer);
			}
		}

		/// <summary>
		/// Обновление информации о масштабах из заголовка классификатора
		/// </summary>
		/// <param name="classifier">
		/// The classifier.
		/// </param>
		private void UpdateScales(ClassifierStruct classifier)
		{
			XPathNodeIterator dataSetRequirements = _navigator.Select("/RNGIS_DataSet/DataSet_Requirement");
			foreach (XPathNavigator dataSetRequirement in dataSetRequirements)
			{
				string contents = dataSetRequirement.GetAttribute("contents", string.Empty);
				int index = contents.IndexOf(':');
				if (index < 2)
				{
					//log.Error("Невозможно определить масштаб :" + contents);
					continue;
				}

				string scaleContents = contents.Substring(index - 1);
				if (classifier.Scales.Contains(scaleContents))
				{
					//log.Error("Классификатор уже содержит масштаб :" + scaleContents);
				}
				else
				{
					classifier.Scales.Add(scaleContents);
					classifier.ScalesDescription.Add(contents);
				}
			}
		}

		private ClassifierStruct ReadDocument(string platform)
		{
			var classifierStruct = new ClassifierStruct();
			XPathNodeIterator classificators = _navigator.Select("//Classificator");			//select Classificator
			foreach (XPathNavigator classificator in classificators)
			{
				NumSection = classificator.Select("//ClassSection").Count;
				this.FillClassSections(classificator, null, classifierStruct, platform);
			}

			UpdateDataLayers(classifierStruct);
			UpdateAutocadAttributes(classifierStruct);
			UpdateScales(classifierStruct);
			return classifierStruct;
		}

		/// <summary>
		/// Извлечь разделы ClassSection из pathSections и вставить в структуру
		/// </summary>
		/// <param name="pathSections">XML-раздел, из которого нужно извлечь ClassSections</param>
		/// <param name="parentSection">Секция верхнего уровня (родитель) для извлекаемых секций. Для первого уровня будет null.</param>
		/// <param name="classifierStruct">Структура классификатора</param>
		/// <param name="platform">Платформа (GIS или Autocad)</param>
		private void FillClassSections(XPathNavigator pathSections, ClassifierStruct.Section parentSection, 
			ClassifierStruct classifierStruct, string platform)
		{
			XPathNodeIterator classSections = pathSections.Select("./ClassSection");		//select ClassSection
			foreach (XPathNavigator classSection in classSections)
			{
				CurSection++;
				var section = new ClassifierStruct.Section(classSection.GetAttribute("name", string.Empty), parentSection);
				CurSectionName = section.Name;												//can use for show progress
				if (parentSection != null)
				{
					parentSection.Subsections.Add(section);
				}

				classifierStruct.Sections.Add(section);
				
				// рекурсивно ищем вложенные секции
				this.FillClassSections(classSection, section, classifierStruct, platform);

				// заполняем ClassItems секции
				FillClassItems(classSection, section, platform);
			}
		}

		/// <summary>
		/// Открытие
		/// </summary>
		/// <param name="pathXml">Имя файла для чтения.</param>
		private void Open(string pathXml)
		{
			_xmlDocument = new XmlDocument();
			_xmlDocument.Load(pathXml);

			_navigator = _xmlDocument.CreateNavigator();
			CurSection = 0;
			NumSection = 0;
		}

		/// <summary>
		/// Открывает XML документ из потока.
		/// </summary>
		/// <param name="reader">
		/// <see cref="TextReader"/> открываемого потока. Используется для управления кодировкой.
		/// </param>
		private void Open(TextReader reader)
		{
			_xmlDocument = new XmlDocument();
			_xmlDocument.Load(reader);

			_navigator = _xmlDocument.CreateNavigator();
			CurSection = 0;
			NumSection = 0;
		}
	}
}
