﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace img_vector
{
    public partial class ExportChoiceForm : Form
    {
        public enum ExportFormat
        {
            None,
            JSON,
            XML,
            PNG_Segmentation_Mask
        }

        /// <summary>
        /// Data to be formatted and exported.
        /// </summary>
        object data;

        /// <summary>
        /// Type of the data that will be formatted and exported.
        /// </summary>
        Type type;

        public string XML_Data
        {
            get
            {
                StringWriter writer = new StringWriter();
                XmlSerializer serializer = new XmlSerializer(type);
                serializer.Serialize(writer, data);

                return writer.ToString();
            }
        }

        public string JSON_Data
        {
            get
            {
                return JsonConvert.SerializeObject(data);
            }
        }

        public string Text_Export_Data
        {
            get
            {
                switch(DataFormat)
                {
                    case ExportFormat.JSON:
                        return JSON_Data;
                    case ExportFormat.XML:
                        return XML_Data;
                    case ExportFormat.PNG_Segmentation_Mask:
                        return "This data cannot be displayed via text.";
                    default:
                        return "";
                }
            }
        }

        public Image[] Image_Export_Data(int imgWidth, int imgHeight)
        {
            List<Image> masks = new List<Image>();

            if (type == typeof(List<ImageClassification>))
            {
                List<ImageClassification> imageClassifications = (List<ImageClassification>)data;
                foreach(ImageClassification classifier in imageClassifications)
                {
                    masks.AddRange(classifier.PNG_Masks());
                }
            }

            return masks.ToArray();
        }

        public ExportChoiceForm()
        {
            InitializeComponent();
        }

        public ExportChoiceForm(Type type, object data) : this()
        {
            this.data = data;
            this.type = type;
        }

        public ExportFormat DataFormat
        {
            get
            {
                if (exportFormatJSONRadioButton.Checked)
                { return ExportFormat.JSON; }
                else if (exportFormatXMLRadioButton.Checked)
                { return ExportFormat.XML; }
                else if(exportFormatPNGMaskRadioButton.Checked)
                { return ExportFormat.PNG_Segmentation_Mask; }
                else
                { return ExportFormat.None; }
            }
        }

        private void exportChoice_Changed(object sender, EventArgs e)
        {
            exportExampleTextbox.Text = Text_Export_Data;
        }
    }
}
