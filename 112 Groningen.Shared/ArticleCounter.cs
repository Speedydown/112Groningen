using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace _112_Groningen
{
    public static class ArticleCounter
    {
        public static void AddArticleCount()
        {
            int Counter = GetCurrentCount() + 1;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["NumberOfArticles"] = Counter;
            }
            catch
            {

            }

            if (Counter == 25)
            {
                Task t = ShowRateDialog();
            }
        }

        private static int GetCurrentCount()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                return (int)localSettings.Values["NumberOfArticles"];
            }
            catch
            {
                return 0;
            }
        }

        private static async Task ShowRateDialog()
        {
            var messageDialog = new MessageDialog("Wij bieden 112Groningen kostenloos aan en we zouden het op prijs stellen als u de 112Groningen app een positieve review geeft.", "Bedankt");
            messageDialog.Commands.Add(
            new UICommand("Review", CommandInvokedHandler));
            messageDialog.Commands.Add(
            new UICommand("Annuleren", CommandInvokedHandler));
            await messageDialog.ShowAsync();
        }


        private async static void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Review")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + "2547c1d0-9568-4b37-bcf0-35695783caf5"));
            }
        }
    }
}
