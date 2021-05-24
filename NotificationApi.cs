using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.OAuth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gnoss.ApiWrapper
{

    /// <summary>
    /// Wrapper for GNOSS notification API
    /// </summary>
    public class NotificationApi : GnossApiWrapper
    {
        private ILogHelper _logHelper;

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="NotificationApi"/>
        /// </summary>
        /// <param name="oauth">OAuth information to sign the Api requests</param>
        public NotificationApi(OAuthInfo oauth, IHttpContextAccessor httpContextAccessor, LogHelper logHelper) : base(oauth, httpContextAccessor, logHelper)
        {
            _logHelper = logHelper.Instance;
        }

        /// <summary>
        /// Consturtor of <see cref="NotificationApi"/>
        /// </summary>
        /// <param name="configFilePath">Configuration file path, with a structure like http://api.gnoss.com/v3/exampleConfig.txt </param>
        public NotificationApi(string configFilePath, IHttpContextAccessor httpContextAccessor, LogHelper logHelper) : base(configFilePath, httpContextAccessor, logHelper)
        {
            _logHelper = logHelper.Instance;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Send an e-mail notification
        /// </summary>
        /// <param name="subject">Subject of the notification</param>
        /// <param name="message">Message of the notification</param>
        /// <param name="isHTML">It indicates whether the content is html</param>
        /// <param name="receivers">Receivers of the notification</param>
        /// <param name="senderMask">Mask sender of the notification</param>
        /// <param name="communityShortName">Community short name</param>
        public int SendEmail(string subject, string message, List<string> receivers, bool isHTML = false, string senderMask = "", string communityShortName = null)
        {
            try
            {
                string url = $"{ApiUrl}/notification/send-email";

                NotificationModel model = new NotificationModel() { subject = subject, message = message, receivers = receivers, is_html = isHTML, sender_mask = senderMask, community_short_name = communityShortName };
                string result = WebRequestPostWithJsonObject(url, model);
                int mailID = Int32.Parse(result);

                _logHelper.Debug($"Email {subject} sended to {string.Join(",", receivers)}");
                return mailID;
            }
            catch (Exception ex)
            {
                _logHelper.Error($"Error sending mail {subject} to {string.Join(",", receivers)}: \r\n{ex.Message}");
                throw;
            }
        }

        public MailStateModel MailState(int mailID)
        {
            try
            {
                string url = $"{ApiUrl}/notification/mail-state?mail_id={mailID}";


                MailStateModel mailStateModel = JsonConvert.DeserializeObject<MailStateModel>(WebRequest("GET", url));

                _logHelper.Debug($"Get mails sended with ID: {mailID}");
                return mailStateModel;
            }
            catch (Exception ex)
            {
                _logHelper.Error($"Error getting mails with ID: {mailID}: \r\n{ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
