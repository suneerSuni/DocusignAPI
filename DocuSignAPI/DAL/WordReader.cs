using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocuSign.Utils;
using FillTheDoc.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace FillTheDoc.DAL
{
    public class WordReader
    {
        #region Public Methods

        public string FillValuesToDoc1(byte[] fileString, string tableStrXML, List<CLDocValue> docVal)
        {
            try
            {
                List<CLTableValue> tblDocVal = new List<CLTableValue>();
                if (!string.IsNullOrEmpty(tableStrXML))
                {
                    if (Utility.IsEventLogged) Utility.LogAction("Building table values");
                    tblDocVal = this.GetAllTableValues(tableStrXML);
                }
                if (Utility.IsEventLogged) Utility.LogAction("Converting base64 string to byte array");
                
                if ((fileString == null) || (fileString.Length == 0))
                {
                    Utility.LogAction("Invalid file");
                    throw new Exception("Invalid File");
                }
                if (Utility.IsEventLogged) Utility.LogAction("Begining OpenXML actions");
                byte[] inArray = this.SetMultipleWordDataFromPlaceHolder(fileString, docVal, tblDocVal);

                if (Utility.IsEventLogged) Utility.LogAction("Save successful");
                return Convert.ToBase64String(inArray); 
            }
            catch (Exception ex)
            {
                Utility.LogAction("Open XML action failed:" + ex.Message);
                return null;
            }
        }

        public string FillValuesToDoc(byte[] fileString, string tableStrXML, List<CLDocValue> docVal)
        {
            try
            {
                List<CLTableValue> tblDocVal = new List<CLTableValue>();
                if (!string.IsNullOrEmpty(tableStrXML))
                {
                    if (Utility.IsEventLogged) Utility.LogAction("Building table values");
                    tblDocVal = this.GetAllTableValues(tableStrXML);
                }
                if (Utility.IsEventLogged) Utility.LogAction("Converting base64 string to byte array");

                if ((fileString == null) || (fileString.Length == 0))
                {
                    Utility.LogAction("Invalid file");
                    throw new Exception("Invalid File");
                }
                if (Utility.IsEventLogged) Utility.LogAction("Begining OpenXML actions");
                byte[] inArray = this.SetMultipleWordDataFromPlaceHolder(fileString, docVal, tblDocVal);

                if (Utility.IsEventLogged) Utility.LogAction("Save successful");
                return Convert.ToBase64String(inArray);
            }
            catch (Exception ex)
            {
                Utility.LogAction("Open XML action failed:" + ex.Message);
                return null;
            }
        }


        #endregion

        #region ManageTable

        private string CreateXML(List<string> valueList, List<string> keyList, List<string> columnList, List<string> rowList, List<string> groupList)
        {
            StringBuilder builder = new StringBuilder("<Value>");
            int index = 0;
            while (true)
            {
                if (index >= valueList.Count<string>())
                {
                    builder.Append("</Value>");
                    return Convert.ToString(builder);
                }
                builder.Append("<Value>");
                builder.Append("<Key>" + keyList.ElementAt<string>(index) + "</Key>");
                builder.Append("<Value>" + valueList.ElementAt<string>(index) + "</Value>");
                builder.Append("<Row>" + rowList.ElementAt<string>(index) + "</Row>");
                builder.Append("<Column>" + columnList.ElementAt<string>(index) + "</Column>");
                builder.Append("<Group>" + groupList.ElementAt<string>(index) + "</Group>");
                builder.Append("</Value>");
                index++;
            }
        }

        private List<CLTableValue> GetAllTableValues(string tableXML)
        {
            List<string> valueList = GetXMLValues(tableXML);

            if (valueList.Any(s => s.Contains("&amp;") || s.Contains("&gt;") || s.Contains("&lt;") || s.Contains("&apos;") || s.Contains("&quot;")))
            {
                List<string> keyList = GetXMLTagValues(tableXML, "Key");
                List<string> columnList = GetXMLTagValues(tableXML, "Column");
                List<string> rowList = GetXMLTagValues(tableXML, "Row");
                List<string> groupList = GetXMLTagValues(tableXML, "Group");

                tableXML = CreateXML(valueList, keyList, columnList, rowList, groupList);
            }

            XDocument doc = XDocument.Parse(tableXML);

            List<CLTableValue> tblDocVal = new List<CLTableValue>();

            var list = doc.Root.Elements("Value")
                                .Select(element => new { Key = element.Element("Key").Value, Value = element.Element("Value").Value, Row = element.Element("Row").Value, Column = element.Element("Column").Value, Group = element.Element("Group").Value })
                                .ToList();

            for (int i = 0; i < list.Count(); i++)
            {
                CLTableValue tab = new CLTableValue();
                tab.Key = list.ElementAt(i).Key;
                tab.Value = list.ElementAt(i).Value;
                tab.Row = list.ElementAt(i).Row;
                tab.Column = list.ElementAt(i).Column;
                tab.Group = list.ElementAt(i).Group;
                tblDocVal.Add(tab);
            }
            return tblDocVal;
        }

        private List<string> GetXMLTagValues(string tableXML, string xmlTag)
        {
            List<string> xmlList = new List<string>();
            Regex regex = new Regex("<" + xmlTag + ">");
            string[] xmlWords = regex.Split(tableXML);
            for (int i = 0; i < xmlWords.Length; i++)
            {
                if (xmlWords[i].Contains("</" + xmlTag + ">"))
                {
                    Regex regValue1 = new Regex("</" + xmlTag + ">");
                    string[] tagValues = regValue1.Split(xmlWords[i]);
                    for (int k = 0; k < tagValues.Length; k++)
                    {
                        if (!string.IsNullOrEmpty(tagValues[k]) || !string.IsNullOrWhiteSpace(tagValues[k]))
                        {
                            xmlList.Add(tagValues[k]);
                            break;
                        }
                    }
                }
            }
            return xmlList;
        }

        private List<string> GetXMLValues(string tableXML)
        {
            List<string> valueList = new List<string>();
            Regex regex = new Regex("</Key>");
            string[] xmlWords = regex.Split(tableXML);
            for (int i = 0; i < xmlWords.Length; i++)
            {
                if (xmlWords[i].Contains("</Value>"))
                {
                    if (xmlWords[i].TrimStart().StartsWith("<Value>"))
                    {
                        Regex regValue1 = new Regex("</Value>");
                        string[] tagValues = regValue1.Split(xmlWords[i]);

                        for (int j = 0; j < tagValues.Length; j++)
                        {
                            if (tagValues[j].TrimStart().StartsWith("<Value>"))
                            {
                                Regex regValue2 = new Regex("<Value>");
                                string[] values = regValue2.Split(tagValues[j]);
                                for (int k = 0; k < values.Length; k++)
                                {
                                    if (!string.IsNullOrEmpty(values[k]) || !string.IsNullOrWhiteSpace(values[k]))
                                    {
                                        values[k] = values[k].Replace("&", "&amp;");
                                        values[k] = values[k].Replace("\"", "&quot;");
                                        values[k] = values[k].Replace("'", "&apos;");
                                        values[k] = values[k].Replace("<", "&lt;");
                                        values[k] = values[k].Replace(">", "&gt;");

                                        valueList.Add(values[k]);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return valueList;
        }

        #endregion

        #region OpenXML

        private void SetHtml(MainDocumentPart mainDocumentPart, string sdtBlockTag, string value, OpenXmlElement elem)
        {
            string html = "<html><body><div>" + value + "</div></body></html>";
            string altChunkId = sdtBlockTag;
            MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(html));
            AlternativeFormatImportPart formatImportPart =
            mainDocumentPart.AddAlternativeFormatImportPart(
            AlternativeFormatImportPartType.Html, altChunkId);
            formatImportPart.FeedData(ms);
            ms.Close();
            AltChunk altChunk = new AltChunk();
            altChunk.Id = altChunkId;
            elem.Append(altChunk);

        }

        private void SetMultipleBlockContent(MainDocumentPart mainDocumentPart, string sdtBlockTag, string value)
        {
            List<SdtElement> SdtElementList = mainDocumentPart.Document.Descendants<SdtElement>().ToList();
            Regex tagRegex = new Regex(@"<[^>]+>");
            foreach (SdtElement sdtr in SdtElementList)
            {
                var s = sdtr.SdtProperties.GetFirstChild<Tag>().Val.Value;
                if (sdtr.SdtProperties.GetFirstChild<Tag>().Val.Value == sdtBlockTag)
                {
                    bool flag = false;
                    bool hasTags = false;

                    if (sdtr.GetFirstChild<SdtContentRun>() != null)
                    {
                        OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentRun>();
                        OpenXmlElementList elements = sdtc.ChildElements;

                        var mySdtc = new SdtContentBlock(sdtc.OuterXml);

                        foreach (OpenXmlElement elem in elements)
                                      {
                            
                            if (elem.HasChildren && !flag)
                            {
                                if (elem.ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☐"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))

                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "☒";

                                    else
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }
                                else if (elem.ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☒"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "☒";

                                    else
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }
                                else if (elem.ChildElements.OfType<Text>().ElementAt(0).Text.Contains("○"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))

                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "●";

                                    else
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }
                                else if (elem.ChildElements.OfType<Text>().ElementAt(0).Text.Contains("●"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "●";

                                    else
                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }
                                else
                                {
                                    if (value!=null && value.Contains("\n"))
                                    {

                                        hasTags = tagRegex.IsMatch(value);
                                        if (hasTags)
                                        {
                                            elem.ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                        }
                                        else
                                        {
                                            string[] val = Regex.Split(value, "\n");
                                            elem.ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            for (int i = 0; i < val.Count(); i++)
                                            {
                                                elem.AppendChild(new Text(val[i]));
                                                if (i != (val.Count() - 1))
                                                    elem.AppendChild(new Break());
                                            }
                                        }
                                        flag = true;
                                    }
                                    else
                                    {
                                        hasTags = tagRegex.IsMatch(value);
                                        if (hasTags)
                                        {
                                            elem.ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                        }
                                        else
                                        {
                                            elem.ChildElements.OfType<Text>().ElementAt(0).Text = value;
                                        }
                                        flag = true;
                                    }
                                    break;
                                }
                            }
                            else if (elem.HasChildren && flag)
                            {
                                elem.ChildElements.OfType<Text>().ElementAt(0).Text = "";
                            }
                        }
                    }
                    else if (sdtr.GetFirstChild<SdtContentBlock>() != null)
                    {
                        OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentBlock>();
                        OpenXmlElementList elements = sdtc.ChildElements;

                        var mySdtc = new SdtContentBlock(sdtc.OuterXml);

                        foreach (OpenXmlElement elem in elements)
                        {
                            if (elem.HasChildren && !flag)
                            {
                                if (elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☐"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☒";
                                    else
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }
                                else if (elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☒"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☒";
                                    else
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }

                                else if (elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("○"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "●";
                                    else
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }
                                else if (elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("●"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "●";
                                    else
                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }

                                else
                                {
                                    if (value.Contains("\n"))
                                    {
                                        hasTags = tagRegex.IsMatch(value);
                                        if (hasTags)
                                        {
                                            elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                        }
                                        else
                                        {
                                            string[] val = Regex.Split(value, "\n");
                                            elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            for (int i = 0; i < val.Count(); i++)
                                            {
                                                elem.AppendChild(new Text(val[i]));
                                                if (i != (val.Count() - 1))
                                                    elem.AppendChild(new Break());
                                            }
                                        }

                                    }
                                    else
                                    {
                                        hasTags = tagRegex.IsMatch(value);
                                        if (hasTags)
                                        {
                                            elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                        }
                                        else
                                        {
                                            elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = value;
                                        }
                                        flag = true;
                                    }
                                    break;

                                }
                            }
                            else if (elem.HasChildren && flag)
                            {
                                elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                            }
                        }

                    }

                    else if (sdtr.GetFirstChild<SdtContentCell>() != null)
                    {
                        OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentCell>();
                        OpenXmlElementList elements = sdtc.ChildElements;

                        var mySdtc = new SdtContentBlock(sdtc.OuterXml);

                        foreach (OpenXmlElement elem in elements)
                        {
                            if (elem.HasChildren && !flag)
                            {
                                if (elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☐"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))

                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☒";

                                    else
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }
                                else if (elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("☒"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☒";

                                    else
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "☐";
                                }

                                else if (elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("○"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))

                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "●";

                                    else
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }
                                else if (elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text.Contains("●"))
                                {
                                    if (value.Equals("True") || value.Equals("true"))
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "●";

                                    else
                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "○";
                                }

                                else
                                {

                                    if (value.Contains("\n"))
                                    {
                                        hasTags = tagRegex.IsMatch(value);
                                        if (hasTags)
                                        {
                                            elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            elem.RemoveAllChildren<Paragraph>();
                                            SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                        }
                                        else
                                        {
                                            string[] val = Regex.Split(value, "\n");
                                            elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                            for (int i = 0; i < val.Count(); i++)
                                            {
                                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).AppendChild(new Text(val[i]));

                                                if (i != (val.Count() - 1))
                                                    elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).AppendChild(new Break());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (value.Contains("\n"))
                                        {
                                            hasTags = tagRegex.IsMatch(value);
                                            if (hasTags)
                                            {
                                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                                elem.RemoveAllChildren<Paragraph>();
                                                SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                            }
                                            else
                                            {
                                                string[] val = Regex.Split(value, "\n");
                                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                                for (int i = 0; i < val.Count(); i++)
                                                {
                                                    elem.AppendChild(new Text(val[i]));
                                                    if (i != (val.Count() - 1))
                                                        elem.AppendChild(new Break());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            hasTags = tagRegex.IsMatch(value);
                                            if (hasTags)
                                            {
                                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                                                elem.RemoveAllChildren<Paragraph>();
                                                SetHtml(mainDocumentPart, sdtBlockTag, value, elem);
                                            }
                                            else
                                            {
                                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = value;
                                            }
                                            flag = true;
                                        }
                                        break;
                                    }
                                    break;
                                }
                            }
                            else if (elem.HasChildren && flag)
                            {
                                elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = "";
                            }
                        }
                    }
                }
            }
        }

        private byte[] SetMultipleWordDataFromPlaceHolder(byte[] fileBytes, List<CLDocValue> value, List<CLTableValue> tblDocVal)
        {
            byte[] returnBytes = null;
            using (MemoryStream myStream = new MemoryStream())
            {
                myStream.Write(fileBytes, 0, (int)fileBytes.Length);
                using (WordprocessingDocument document = WordprocessingDocument.Open(myStream, true))
                {
                    MainDocumentPart workbookPart = document.MainDocumentPart;
                    foreach (var docVal in value)
                    {
                        if (!string.IsNullOrEmpty(docVal.Value))
                        {
                            SetMultipleBlockContent(workbookPart, docVal.Key, docVal.Value);
                        }
                        
                    }
                    if (tblDocVal.Count() != 0)
                    {
                        SetValueToTable(workbookPart, tblDocVal);
                    }
                    document.MainDocumentPart.Document.Save();
                    document.Close();
                }
                returnBytes = myStream.ToArray();
            }
            return returnBytes;
        }

        private void SetValueToTable(MainDocumentPart mainDocumentPart, List<CLTableValue> tblDocVal)
        {
            IEnumerable<TableProperties> tableProperties = mainDocumentPart.Document.Body.Descendants<TableProperties>().Where(tp => tp.TableCaption != null);
            List<SdtElement> SdtElementList = null;
            var tables = (from val in tblDocVal
                          where (val.Group != "?") && (val.Group != "")
                          select val.Group).Distinct();

            int Count = tables.Count();


            foreach (TableProperties tProp in tableProperties)
            {
                Table table = (Table)tProp.Parent;
                //foreach (TableRow tabRow in table.Elements<TableRow>())
                //{

                //}
                List<TableRow> theRow = table.Elements<TableRow>().ToList();

                for (int i = 0; i < Count; i++)
                {
                    var rowCount = (from val in tblDocVal
                                    where val.Group == tables.ElementAt(i)
                                    select Convert.ToInt32(val.Row)).Max();

                    if (Convert.ToString(tProp.TableCaption.Val).Equals(Convert.ToString(tables.ElementAt(i))))
                    {

                        for (int m = 0; m < Convert.ToInt32(rowCount); m++)
                        {
                            if (theRow.Count >= m)
                            {
                                SdtElementList = theRow[m].Descendants<SdtElement>().ToList();
                                var data = (from val in tblDocVal
                                            where (val.Group == tables.ElementAt(i) && (val.Row == Convert.ToString(m + 1)))
                                            select new { val.Key, val.Value, val.Column }).OrderBy(col => col.Column).ToList();
                                for (int j = 0; j < data.Count(); j++)
                                {

                                    foreach (SdtElement sdtr in SdtElementList)
                                    {
                                        if (sdtr.SdtProperties.GetFirstChild<Tag>().Val.Value == data.ElementAt(j).Key)
                                        {
                                            if (sdtr.GetFirstChild<SdtContentRun>() != null)
                                            {
                                                OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentRun>();
                                                OpenXmlElementList elements = sdtc.ChildElements;

                                                foreach (OpenXmlElement elem in elements)
                                                {
                                                    if (elem.HasChildren)
                                                    {
                                                        elem.ChildElements.OfType<Text>().ElementAt(0).Text = data.ElementAt(j).Value;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (sdtr.GetFirstChild<SdtContentBlock>() != null)
                                            {
                                                OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentBlock>();
                                                OpenXmlElementList elements = sdtc.ChildElements;

                                                foreach (OpenXmlElement elem in elements)
                                                {
                                                    if (elem.HasChildren)
                                                    {
                                                        elem.ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = data.ElementAt(j).Value;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (sdtr.GetFirstChild<SdtContentCell>() != null)
                                            {
                                                OpenXmlElement sdtc = sdtr.GetFirstChild<SdtContentCell>();
                                                OpenXmlElementList elements = sdtc.ChildElements;
                                                foreach (OpenXmlElement elem in elements)
                                                {
                                                    if (elem.HasChildren)
                                                    {
                                                        elem.ChildElements.OfType<Paragraph>().ElementAt(0).ChildElements.OfType<Run>().ElementAt(0).ChildElements.OfType<Text>().ElementAt(0).Text = data.ElementAt(j).Value;
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }


                            }
                            else
                            {
                                //Place holder name will be same asthat of the last row
                                TableRow rowCopy = (TableRow)theRow[m].CloneNode(true);
                                table.Append(rowCopy);
                            }

                        }

                        break;
                    }

                }
            }
        }

        #endregion

        

    }
}