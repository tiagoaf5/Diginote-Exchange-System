using System;
using Common;

namespace Client
{
    public class ClientNotify : MarshalByRefObject, IClientNotify
    {
        private MainWindowClient _win;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void PutMyForm(MainWindowClient form)
        {
            _win = form;
        }

        public void SomeMessage(string message)
        {
            _win.AddMessage(message);
        }

        public void UpdateClientView()
        {
            _win.UpdateView();
        }
    }
}