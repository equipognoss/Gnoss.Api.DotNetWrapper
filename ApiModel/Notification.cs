using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gnoss.ApiWrapper.ApiModel
{
    /// <summary>
    /// Notification parameters
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// Message of the email
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// True if the message contains html
        /// </summary>
        public bool is_html { get; set; }

        /// <summary>
        /// List of email receivers
        /// </summary>
        public List<string> receivers { get; set; }

        /// <summary>
        /// Sender mask
        /// </summary>
        public string sender_mask { get; set; }

        /// <summary>
        /// Community short name
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Transmitter smtp config defined
        /// </summary>
        public MailConfigurationModel transmitter_mail_configuration { get; set; }
    }

    /// <summary>
    /// Model to show the state of an email
    /// </summary>
    public class MailStateModel
    {
        /// <summary>
        /// List of emails pending to send
        /// </summary>
        public List<string> pending_mails { get; set; }
        /// <summary>
        /// List with the email that can't be sended
        /// </summary>
        public List<string> error_mails { get; set; }
    }

    /// <summary>
    /// Model to configure the mail
    /// </summary>
    public class MailConfigurationModel
    {
        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Smtp of the mail server
        /// </summary>
        public string smtp { get; set; }
        /// <summary>
        /// Port of the mail server
        /// </summary>
        public short puerto { get; set; }
        /// <summary>
        /// Password of the user to login in the mail server
        /// </summary>
        public string clave { get; set; }
        /// <summary>
        /// Kind of the mail server
        /// </summary>
        public string tipo { get; set; }
    }
    public class NotificationContentHtml
    {
        public Guid usuario { get; set; }
        public Guid comunidad { get; set; }
        public string contenidoNotificacion { get; set; }
        public DateTime fechaNotificacion { get; set; }
    }
    public class NotificationContentDefault
    {
        //id usuario, Guid comunidad,string contenidoNotificacion,string urlNotificacion, DateTime fechaNotificacion
        public Guid usuario { get; set; }
        public Guid comunidad { get; set; }
        public string contenidoNotificacion { get; set; }
        public string urlNotificacion { get; set; }
        public DateTime fechaNotificacion { get; set; }
    }
}