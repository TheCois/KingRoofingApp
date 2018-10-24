namespace KRF.Core.FunctionalContracts
{
    /// <summary>
    /// Mail service Implementation
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Function to send emails
        /// Additional To / CC/ BCC email address can be specified in MailSettings.xml
        /// Will lookup for templateName.cshtml file in the folder specified in MailSettings.xml
        /// If templateName.txt file is also found, will also create a plain text version of the mail
        /// </summary>
        /// <param name="model">Any typed / anonymous class from which to pick up values </param>
        /// <param name="templateFileName"> The template file name (.cshtml), without extension. Cannot be null</param>
        /// <param name="mailTo">The To email address, can have multiple comma seperated email addresses, can be null</param>
        /// <param name="mailCC">The CC email address, can have multiple comma seperated email addresses, can be null</param>
        /// <param name="subject">The Subject of the email</param>
        /// <param name="configSectionName">The name of the section in the MailSettings.xml from which to pick up server and other details. If null, will use 'DefaultMailSettings'</param>
        /// <returns>Void if mail sent successfully, else will raise exception on error.</returns>
        void SendMail<T>(T model, string templateFileName, string mailTo, string mailCC, string subject, string mailSettingPath, string configSectionName = null);
    }
}