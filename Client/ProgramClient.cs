using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;

namespace Client
{
    static class ProgramClient
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IDictionary props = new Hashtable();
            props["port"] = 0;  // let the system choose a free port
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            TcpChannel chan = new TcpChannel(props, clientProvider, serverProvider);  // instantiate the channel
            ChannelServices.RegisterChannel(chan, false);                             // register the channel

            ChannelDataStore data = (ChannelDataStore)chan.ChannelData;
            int port = new Uri(data.ChannelUris[0]).Port;                            // get the port

            RemotingConfiguration.Configure("Client.exe.config", false);             // register the server objects
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ClientNotify), "ClientNotify", WellKnownObjectMode.Singleton);  // register my remote object for service

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin(port) {FormBorderStyle = FormBorderStyle.FixedSingle});
        }
    }

}
