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

    public class MailStateModel
    {
        public List<string> pending_mails { get; set; }
        public List<string> error_mails { get; set; }
    }

    public class MailConfigurationModel
    {
        public string email { get; set; }
        public string smtp { get; set; }
        public short puerto { get; set; }
        public string clave { get; set; }
        public string tipo { get; set; }
    }
}