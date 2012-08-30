using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.XForms.WebControls;
using Label = System.Web.UI.WebControls.Label;

namespace EPiBooks {
    public class Global : EPiServer.Global {
        protected void Application_Start(Object sender, EventArgs e) {

            FindCrawler.Config.Current.Urls = new List<string> { "http://www.sogeti.se", "http://www.sogeti.com", "http://www.episerver.com/" };
            FindCrawler.Config.Current.NumberOfThreads = 5;
            FindCrawler.Config.Current.MaximumCrawlDepth = 3;
            FindCrawler.Config.Current.LogFilePath = @"C:\Projects\EPiBooks\Web\Logs\";
            FindCrawler.Config.Current.LogLevel = "DEBUG";

            XFormControl.ControlSetup += new EventHandler(XForm_ControlSetup);
        }

        #region Global XForm Events

        /// <summary>
        /// Sets up events for each new instance of the XFormControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>As the ControlSetup event is triggered for each instance of the XFormControl control
        /// you need to take into consideration that any event handlers will affect all XForms for the entire
        /// application. If the EPiServer UI is running in the same application this might also be affected depending
        /// on which events you attach to and what is done in the event handlers.</remarks>
        public void XForm_ControlSetup(object sender, EventArgs e) {
            XFormControl control = (XFormControl)sender;

            control.BeforeLoadingForm += new LoadFormEventHandler(XForm_BeforeLoadingForm);
            control.ControlsCreated += new EventHandler(XForm_ControlsCreated);
            control.BeforeSubmitPostedData += new SaveFormDataEventHandler(XForm_BeforeSubmitPostedData);
            control.AfterSubmitPostedData += new SaveFormDataEventHandler(XForm_AfterSubmitPostedData);
        }

        /// <summary>
        /// Handles the BeforeLoadingForm event of the XFormControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.XForms.WebControls.LoadFormEventArgs"/> instance containing the event data.</param>
        public void XForm_BeforeLoadingForm(object sender, LoadFormEventArgs e) {
            XFormControl formControl = (XFormControl)sender;

            if (String.IsNullOrEmpty(formControl.ValidationGroup)) {
                //We set the validation group of the form to match our global validation group in the master page if no group has been defined.
                formControl.ValidationGroup = "XForm";
            }
        }

        /// <summary>
        /// Handles the ControlsCreated event of the XFormControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void XForm_ControlsCreated(object sender, EventArgs e) {
            XFormControl formControl = (XFormControl)sender;

            //We set the inline error validation text to "*" as we use a
            //validation summary in the master page to display the detailed error message.
            foreach (BaseValidator validator in formControl.Controls.Cast<Control>().Where(ctrl => ctrl is BaseValidator)) {
                validator.Text = "*";
            }

            if (formControl.Page.User.Identity.IsAuthenticated) {
                formControl.Data.UserName = formControl.Page.User.Identity.Name;
            }
        }

        /// <summary>
        /// Handles the BeforeSubmitPostedData event of the XFormControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.XForms.WebControls.SaveFormDataEventArgs"/> instance containing the event data.</param>
        public void XForm_BeforeSubmitPostedData(object sender, SaveFormDataEventArgs e) {
            XFormControl control = (XFormControl)sender;

            PageBase currentPage = control.Page as PageBase;

            if (currentPage == null) {
                return;
            }

            //We set the current page that the form has been posted from
            //This might differ from the actual page that the form property exists on.
            e.FormData.PageGuid = currentPage.CurrentPage.PageGuid;
        }

        /// <summary>
        /// Handles the AfterSubmitPostedData event of the XFormControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EPiServer.XForms.WebControls.SaveFormDataEventArgs"/> instance containing the event data.</param>
        public void XForm_AfterSubmitPostedData(object sender, SaveFormDataEventArgs e) {
            XFormControl control = (XFormControl)sender;

            if (control.FormDefinition.PageGuidAfterPost != Guid.Empty) {
                PermanentContentLinkMap pageMap = PermanentLinkMapStore.Find(control.FormDefinition.PageGuidAfterPost) as PermanentContentLinkMap;
                if (pageMap != null) {
                    string internalUrl = pageMap.MappedUrl.ToString();
                    internalUrl = UriSupport.AddLanguageSelection(internalUrl, ContentLanguage.PreferredCulture.Name);
                    UrlBuilder urlBuilder = new UrlBuilder(internalUrl);
                    //Rewrite the url to make sure that it gets the friendly url if such a provider has been configured.
                    Global.UrlRewriteProvider.ConvertToExternal(urlBuilder, null, Encoding.UTF8);

                    //Always cast UrlBuilders to get a correctly encoded url since ToString() is for "human" readability.
                    control.Page.Response.Redirect((string)urlBuilder);
                    return;
                }
            }

            //After the form has been posted we remove the form elements and add a "thank you message".
            control.Controls.Clear();
            Label label = new Label();
            label.CssClass = "thankyoumessage";
            label.Text = LocalizationService.Current.GetString("/form/postedmessage");
            control.Controls.Add(label);
        }

        #endregion

        protected void Application_End(Object sender, EventArgs e) {
        }

        /// <summary>
        /// Raises the <see cref="E:ValidateUIAccess"/> event. Override this in inheriting classes to customize behavior,
        /// always calling the base-class implementation as well. Check for e.Cancel == true and do an early exit if so.
        /// </summary>
        /// <remarks>
        /// This is only a code sample on how you can utilize this event.
        /// </remarks>
        /// <param name="e">The <see cref="EPiServer.ValidateUIAccessEventArgs"/> instance containing the event data.</param>
        protected override void OnValidateRequestAccess(ValidateRequestAccessEventArgs e) {
            base.OnValidateRequestAccess(e);
            if (e.Cancel) {
                return;
            }
        }
    }
}