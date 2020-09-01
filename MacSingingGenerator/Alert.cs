using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    //static class for creating alerts
    static public class Alert
    {
        static public AlertResult CreateAlert(string text, AlertType alertType)
        {
            using (var alert = new NSAlert())
            {
                alert.MessageText = text;
                alert.Icon = NSImage.ImageNamed("NSCaution");
                if (alertType == AlertType.Info)
                {
                    alert.Icon = NSImage.ImageNamed("NSInfo");
                }
                if (alertType == AlertType.YesNo || alertType == AlertType.YesNoCancel)
                {
                    alert.AddButton("Yes"); //1000
                    alert.AddButton("No"); //1001
                    if (alertType == AlertType.YesNoCancel)
                    {
                        alert.AddButton("Cancel"); //1002
                    }
                }
                nint result = alert.RunModal();
                switch (result)
                {
                    case 1000:
                        return AlertResult.Yes;
                    case 1001:
                        return AlertResult.No;
                    case 1002:
                        return AlertResult.Cancel;
                    default:
                        return AlertResult.None;
                }
            }
        }
    }

    //enums to keep track of alert info
    public enum AlertType { Info, Caution, YesNo, YesNoCancel }
    public enum AlertResult { None, Yes, No, Cancel }
}
