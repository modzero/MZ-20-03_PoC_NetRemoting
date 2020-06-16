using System;
using System.Runtime.Remoting.Channels; //To support and handle Channel and channel sinks
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http; //For HTTP channel
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters;

namespace ServerApp
{
    //Service class
    public class Service : MarshalByRefObject
    {
        public void WriteMessage(int num1, int num2)
        {
            Console.WriteLine(Math.Max(num1, num2));
        }
    }
    //Server Class
    public class Server
    {
        public static void Main()
        {
            //TypeFilterLevel to use.
            //When set to Low PoC causes DOS
            //When set to Full PoC causes RCE
            TypeFilterLevel filterLevel = TypeFilterLevel.Full;

            //Try block to catch anything, fails to capture DOS exception
            try
            {
                var channel = new HttpChannel(8001); //Create a new channel, also works with Tcp and Ipc, but Http is easiest to implement

                //Iterate sinks to set filterLevel on binaryFormatter,
                //This is only required to set TypeFilterLevel to Full
                //Default is Low
                var chain = channel.ChannelSinkChain;
                while(chain!=null)
                {
                    if (chain is BinaryServerFormatterSink binChain)
                        binChain.TypeFilterLevel = filterLevel;
                    chain = chain.NextChannelSink;
                }

                //Register channel and service
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(Service), "Service", WellKnownObjectMode.Singleton);

                //Wait for enter press
                Console.WriteLine("Server ON at port number:8001");
                Console.WriteLine("Please press enter to stop the server.");
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("Could handle");
                Console.ReadLine();
            }
        }
    }
}