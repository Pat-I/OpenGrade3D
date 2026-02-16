using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace OpenGrade
{
    public partial class FormGPS
    {
        // Send and Recv socket
        private Socket loopBackSocket;
        public static UdpClient udpServer;
        private static int port = 15555; // Port to listen on

        // UDP Socket from AgIO v6.3.3
        private EndPoint endPointLoopBack = new IPEndPoint(IPAddress.Loopback, 0);

        public bool isUDPNetworkConnected;

        private EndPoint epAgIO = new IPEndPoint(IPAddress.Parse("127.255.255.255"), 17777);
        //private IPEndPoint epNtrip = new IPEndPoint(IPAddress.Parse(Properties.Settings.Default.setIP_autoSteerIP), 2233);
        //end from AgIO v6.3.3

        private bool isSendConnected = true;

        // Data stream
        private byte[] buffer = new byte[1024];

        // Status delegate
        private delegate void UpdateRecvMessageDelegate(string recvMessage);
        private UpdateRecvMessageDelegate updateRecvMessageDelegate = null;

        public void SendPgnToLoop(byte[] byteData)
        {
            if (loopBackSocket != null && byteData.Length > 2)
            {
                try
                {
                    int crc = 0;
                    for (int i = 2; i + 1 < byteData.Length; i++)
                    {
                        crc += byteData[i];
                    }
                    byteData[byteData.Length - 1] = (byte)crc;

                    loopBackSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None,
                        epAgIO, new AsyncCallback(SendAsyncLoopData), null);
                }
                catch (Exception)
                {
                    //Log.EventWriter("Sending UDP Message" + e.ToString());
                    //MessageBox.Show("Send Error: " + e.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void SendAsyncLoopData(IAsyncResult asyncResult)
        {
            try
            {
                loopBackSocket.EndSend(asyncResult);
            }
            catch (Exception)
            {
                //MessageBox.Show("SendData Error: " + ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //initialize loopback and udp network
        public void LoadUDPNetwork()
        {
            new Thread(() =>
            {
                using (udpServer = new UdpClient(port))
                {
                    while (true)
                    {
                        ListenForMessages();
                    }
                }
            })
            { IsBackground = true }.Start();

            //        udpServer.Close(); // where would I put this?

        }
        #region Receive UDP 
        //from AgIO v6.3.3

        private void ListenForMessages()
        {
            try
            {
                // Listen for UDP packets on the given port
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
                byte[] data = udpServer.Receive(ref endPoint);
                recvSentenceSettings = Encoding.UTF8.GetString(data);

                if (data[0] == 0x80 && data[1] == 0x81)
                {
                    switch (data[3])
                    {
                        case 0xD6:
                            {
                                //todo add a condition to use UDP GPS as source
                                double Lon = BitConverter.ToDouble(data, 5);
                                double Lat = BitConverter.ToDouble(data, 13);

                                if (Lon != double.MaxValue && Lat != double.MaxValue)
                                {
                                    pn.longitude = Lon;
                                    pn.latitude = Lat;

                                    //if (timerSim.Enabled) DisableSim();//todo


                                    if (pn.latitude < 0)
                                    {
                                        pn.hemisphere = 'S';
                                    }
                                    else { pn.hemisphere = 'N'; }

                                    //calculate zone and UTM coords
                                    //DecDeg2UTM();
                                    pn.ConvertWGS84ToLocal(pn.latitude, pn.longitude, out pn.northing, out pn.easting);

                                    //From dual antenna heading sentences
                                    float temp = BitConverter.ToSingle(data, 21);
                                    if (temp != float.MaxValue)
                                    {
                                        pn.headingTrue = temp;
                                    }
                                    else
                                    {
                                    //from single antenna sentences (VTG,RMC)
                                    pn.headingTrue = BitConverter.ToSingle(data, 25);
                                    }

                                    //always save the speed.
                                    temp = BitConverter.ToSingle(data, 29);
                                    if (temp != float.MaxValue)
                                    {
                                        pn.speed = temp;
                                    }

                                    //roll in degrees
                                    temp = BitConverter.ToSingle(data, 33);
                                    if (temp != float.MaxValue)
                                    {
                                        pn.GPSroll = temp;
                                    }
                                    if (temp == float.MinValue)
                                        pn.GPSroll = 0;

                                    //altitude in meters
                                    temp = BitConverter.ToSingle(data, 37);
                                    if (temp != float.MaxValue)
                                        pn.altitude = temp;

                                    ushort sats = BitConverter.ToUInt16(data, 41);
                                    if (sats != ushort.MaxValue)
                                        pn.satellitesTracked = sats;

                                    byte fix = data[43];
                                    if (fix != byte.MaxValue)
                                        pn.fixQuality = fix;

                                    ushort hdop = BitConverter.ToUInt16(data, 44);
                                    if (hdop != ushort.MaxValue)
                                        pn.hdop = hdop * 0.01;

                                    ushort age = BitConverter.ToUInt16(data, 46);
                                    if (age != ushort.MaxValue)
                                        pn.ageDiff = age * 0.01;

                                    ushort imuHead = BitConverter.ToUInt16(data, 48);
                                    if (imuHead != ushort.MaxValue)
                                    {
                                        //ahrs.imuHeading = imuHead; //no use for IMU heading yet
                                        //ahrs.imuHeading *= 0.1;
                                    }

                                    short imuRol = BitConverter.ToInt16(data, 50);
                                    if (imuRol != short.MaxValue)
                                    {
                                        double rollK = imuRol;
                                        pn.GPSroll = rollK *= 0.1;
                                    }

                                    short imuPich = BitConverter.ToInt16(data, 52);
                                    if (imuPich != short.MaxValue)
                                    {
                                        pn.GPSpitch = imuPich;
                                    }

                                    short imuYaw = BitConverter.ToInt16(data, 54);
                                    if (imuYaw != short.MaxValue)
                                    {
                                        pn.GPSyawRate = imuYaw;
                                    }

                                    //a valid VTG so set the flag
                                    recvCounter = 0;

                                    //average the speeds for display, not calcs
                                    avgSpeed[ringCounter] = pn.speed;
                                    if (ringCounter++ > 8) ringCounter = 0;

                                    ProcessFixPosition();
                                }
                            }
                            break;
                    }
                } // end of pgns
            }
            catch
            {

            }
        }

        #endregion
    
}
}
