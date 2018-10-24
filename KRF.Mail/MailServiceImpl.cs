using KRF.Core.FunctionalContracts;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Xml.Linq;

namespace KRF.Mail
{
    public class MailServiceImpl : IMailService
    {
        #region Constructor
        public MailServiceImpl()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(RazorEngine.Encoding.Raw));

            // create a new TemplateService and pass in the configuration to the constructor
            var myConfiguredTemplateService = new TemplateService(config);

            // set the template service to our configured one
            Razor.SetTemplateService(myConfiguredTemplateService);
        }
        #endregion

        // Note: MailTo, MailCC can have multiple comma seperated email addresses.
        public void SendMail<T>(T model, string templateName, string mailTo, string mailCC, string subject, string mailSettingPath, string configSectionName = null)
        {
            // read settings from config file.
            // using System.Net.Mail create mail object.
            // add additional cc / bcc addresses from config file.
            // send mail.

            XElement xmlConfigContents = GetConfigFileContents(mailSettingPath);

            //Folder Path + Template name will give the Template File appended by ".cshtml" or ".txt"
            //string templateNameHTML = xmlConfigContents.Element("TemplateFolderPath").Value + "\\" + templateName + ".cshtml";
            string templateNameHTML = mailSettingPath + "\\" + templateName + ".cshtml";
            //string templateNameText = xmlConfigContents.Element("TemplateFolderPath").Value + "\\" + templateName + ".txt";
            string templateNameText = mailSettingPath + "\\" + templateName + ".txt";

            // If config section not found then use default setting.
            if (string.IsNullOrWhiteSpace(configSectionName) || xmlConfigContents.Element(configSectionName) == null)
                configSectionName = "DefaultMailSettings";

            XElement xmlConfigSection = xmlConfigContents.Element(configSectionName);

            //Read from File Template
            string mailContentsHTML = ReadFile(templateNameHTML);
            string mailContentsText = null;

            if (File.Exists(templateNameText))
                mailContentsText = ReadFile(templateNameText);

            //Replace @codes with Model Values.
            mailContentsHTML = DoTokenReplacement(model, mailContentsHTML);

            if (mailContentsText != null)
                mailContentsText = DoTokenReplacement(model, mailContentsText);

            //Create Mail Object
            MailMessage objMail = new MailMessage();
            objMail.From = new MailAddress(xmlConfigSection.Element("MailFrom").Value);

            // Add addresses which are passed as parameters.
            if (!string.IsNullOrWhiteSpace(mailTo))
                objMail.To.Add(mailTo);

            if (!string.IsNullOrWhiteSpace(mailCC))
                objMail.CC.Add(mailCC);

            // Add additional addresses, if specified in config file.
            XElement xmlConfigItem = xmlConfigSection.Element("MailTo");

            if (xmlConfigItem != null && !string.IsNullOrWhiteSpace(xmlConfigItem.Value))
                objMail.To.Add(xmlConfigItem.Value);

            xmlConfigItem = xmlConfigSection.Element("MailCC");

            if (xmlConfigItem != null && !string.IsNullOrWhiteSpace(xmlConfigItem.Value))
                objMail.CC.Add(xmlConfigItem.Value);

            xmlConfigItem = xmlConfigSection.Element("MailBCC");

            if (xmlConfigItem != null && !string.IsNullOrWhiteSpace(xmlConfigItem.Value))
                objMail.Bcc.Add(xmlConfigItem.Value);

            objMail.Subject = subject;

            if (mailContentsText != null)
            {
                //Added Alternate view for Plain Text mail body
                AlternateView objHTMLAltView = AlternateView.CreateAlternateViewFromString(mailContentsHTML, null, MediaTypeNames.Text.Html);
                objMail.AlternateViews.Add(objHTMLAltView);

                AlternateView objPlainAltView = AlternateView.CreateAlternateViewFromString(mailContentsText, null, MediaTypeNames.Text.Plain);
                objMail.AlternateViews.Add(objPlainAltView);
            }
            else
            {
                objMail.Body = mailContentsHTML;
                objMail.IsBodyHtml = true;
            }

            SmtpClient client = new SmtpClient(xmlConfigSection.Element("host").Value);
            client.UseDefaultCredentials = false;

            System.Net.NetworkCredential loginInfo = new System.Net.NetworkCredential(xmlConfigSection.Element("userName").Value, xmlConfigSection.Element("password").Value);
            client.Credentials = loginInfo;
            client.Send(objMail);
        }

        #region Private Methods
        private XElement GetConfigFileContents(string mailSettingPath)
        {
            //string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            //filePath = Path.GetDirectoryName(filePath);
            string filePath = mailSettingPath;
            filePath += filePath.EndsWith(@"\") ? "MailSettings.xml" : @"\MailSettings.xml";

            return XElement.Parse(ReadFile(filePath));
        }

        private string ReadFile(string fileName)
        {
            StreamReader myFile = new StreamReader(fileName);
            string strFileContents = myFile.ReadToEnd();
            myFile.Close();

            return strFileContents;
        }

        private string DoTokenReplacement<T>(T model, string templateContents)
        {
            return Razor.Parse(templateContents, model);
        }
        #endregion
    }
}