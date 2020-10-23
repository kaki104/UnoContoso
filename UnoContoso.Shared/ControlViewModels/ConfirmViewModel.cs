using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using UnoContoso.Base;

namespace UnoContoso.ControlViewModels
{
    public class ConfirmViewModel : DialogViewModelBase
    {
        protected override void OnClose(string obj)
        {
            ButtonResult result = ButtonResult.None;
            switch(obj.ToLower())
            {
                case "save":
                    result = ButtonResult.Yes;
                    break;
                case "notsave":
                    result = ButtonResult.No;
                    break;
                case "cancel":
                    result = ButtonResult.Cancel;
                    break;
            }
            RaiseRequestClose(new DialogResult(result));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("title", out string title);
            Title = title ?? "Confirmation";
            Message = parameters.GetValue<string>("message");
        }
    }
}
