using System;
using System.Configuration;
using KRF.Core.FunctionalContracts;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml.Linq;
using KRF.Core;

namespace KRF.Mail
{
    public class MailServiceImpl : IMailService
    {
        private readonly SecretsReader sReader_;
        
        #region Constructor

        public MailServiceImpl()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(Encoding.Raw));

            // create a new TemplateService and pass in the configuration to the constructor
            var myConfiguredTemplateService = new TemplateService(config);

            // set the template service to our configured one
            Razor.SetTemplateService(myConfiguredTemplateService);
            sReader_ = new SecretsReader(ConfigurationManager.AppSettings["ThirdPartySecretsFile"]);
        }

        #endregion

        // Note: MailTo, MailCC can have multiple comma separated email addresses.
        public void SendMail<T>(T model, string templateName, string mailTo, string mailCc, string subject,
            string mailSettingPath, string configSectionName = null)
        {
            // read settings from config file.
            // using System.Net.Mail create mail object.
            // add additional cc / bcc addresses from config file.
            // send mail.

            var xmlConfigContents = GetConfigFileContents(mailSettingPath);

            //Folder Path + Template name will give the Template File appended by ".cshtml" or ".txt"
            //string templateNameHTML = xmlConfigContents.Element("TemplateFolderPath").Value + "\\" + templateName + ".cshtml";
            var templateNameHtml = Path.Combine(mailSettingPath, templateName + ".cshtml");
            //string templateNameText = xmlConfigContents.Element("TemplateFolderPath").Value + "\\" + templateName + ".txt";
            var templateNameText = Path.Combine(mailSettingPath, templateName + ".txt");

            // If config section not found then use default setting.
            if (string.IsNullOrWhiteSpace(configSectionName) || xmlConfigContents.Element(configSectionName) == null)
                configSectionName = "DefaultMailSettings";

            var xmlConfigSection = xmlConfigContents.Element(configSectionName);

            //Read from File Template
            var mailContentsHtml = ReadFile(templateNameHtml);
            string mailContentsText = null;

            if (File.Exists(templateNameText))
                mailContentsText = ReadFile(templateNameText);

            //Replace @codes with Model Values.
            mailContentsHtml = DoTokenReplacement(model, mailContentsHtml);

            if (mailContentsText != null)
                mailContentsText = DoTokenReplacement(model, mailContentsText);

            if (xmlConfigSection == null)
            {
                throw new ArgumentException("The Configuration Section desired is not present in the Mail settings XML file");
            }
            //Create Mail Object
            var objMail = new MailMessage {From = new MailAddress(xmlConfigSection.Element("MailFrom").Value)};

            // Add addresses which are passed as parameters.
            if (!string.IsNullOrWhiteSpace(mailTo))
                objMail.To.Add(mailTo);

            if (!string.IsNullOrWhiteSpace(mailCc))
                objMail.CC.Add(mailCc);

            // Add additional addresses, if specified in config file.
            var xmlConfigItem = xmlConfigSection.Element("MailTo");

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
                var objHtmlAltView =
                    AlternateView.CreateAlternateViewFromString(mailContentsHtml, null, MediaTypeNames.Text.Html);
                objMail.AlternateViews.Add(objHtmlAltView);

                var objPlainAltView =
                    AlternateView.CreateAlternateViewFromString(mailContentsText, null, MediaTypeNames.Text.Plain);
                objMail.AlternateViews.Add(objPlainAltView);
            }
            else
            {
                objMail.Body = mailContentsHtml;
                objMail.IsBodyHtml = true;
            }

            var client = new SmtpClient(xmlConfigSection.Element("host").Value
                , int.Parse(xmlConfigSection.Element("port").Value)) {UseDefaultCredentials = false, EnableSsl = true};

            var loginInfo = new System.Net.NetworkCredential(xmlConfigSection.Element("userName").Value,
                sReader_["SmtpPassword"]);
            client.Credentials = loginInfo;
            client.Send(objMail);
        }

        #region Private Methods

        private XElement GetConfigFileContents(string mailSettingPath)
        {
            //string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            //filePath = Path.GetDirectoryName(filePath);
            var filePath = Path.Combine(mailSettingPath, "MailSettings.xml");

            return XElement.Parse(ReadFile(filePath));
        }

        private string ReadFile(string fileName)
        {
            var myFile = new StreamReader(fileName);
            var strFileContents = myFile.ReadToEnd();
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