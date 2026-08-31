using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenGrade
{
    public partial class FormGPS
    {
        private Socket loopBackSocket;
        private UdpClient _udpServer;
        private CancellationTokenSource _udpCts;
        private const int Port = 15555;

        private readonly EndPoint epAgIO = new IPEndPoint(IPAddress.Parse("127.255.255.255"), 17777);
        public bool isUDPNetworkConnected = false;
        public bool isGNSSfromOG1 = false;
        public bool isDataFromOGudpBlade = false;
        // Verrou pour empêcher les accès simultanés aux données de positionnement (Thread-safety)
        private readonly object _positionLock = new object();

        /// <summary>
        /// Initialise et démarre l'écoute UDP de manière asynchrone et non-bloquante.
        /// </summary>
        public void LoadUDPNetwork()
        {
            // Arrêter une éventuelle instance précédente
            ShutdownUDPNetwork();

            _udpCts = new CancellationTokenSource();

            // Lancement de la tâche d'écoute en arrière-plan sans bloquer l'UI
            Task.Run(() => StartListeningAsync(_udpCts.Token));
        }

        /// <summary>
        /// Arrête proprement le serveur UDP .
        /// </summary>
        public void ShutdownUDPNetwork()
        {
            _udpCts?.Cancel();
            _udpServer?.Close();
            _udpServer = null;
        }

        private async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            try
            {
                _udpServer = new UdpClient(Port);
                isUDPNetworkConnected = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Pure asynchronous wait: releases the thread until a packet arrives
                    UdpReceiveResult result = await _udpServer.ReceiveAsync().ConfigureAwait(false);
                    byte[] data = result.Buffer;

                    // 1. Minimum size validation: Frame needs at least Preamble(2), ID(2), Length(1), and CRC(1) = 6 bytes
                    if (data != null && data.Length >= 6)
                    {
                        // 2. Validate universal start flags (Preamble)
                        if (data[0] == 0x80 && data[1] == 0x81)
                        {
                            // Extract Message Identifiers (Bytes 2 and 3)
                            byte msgId1 = data[2];
                            byte msgId2 = data[3];

                            // 3. Dynamic extraction of the payload size (Byte 4)
                            byte payloadLength = data[4];

                            // 4. Calculate total expected packet length
                            // Header (5 bytes) + Payload (Variable) + CRC (1 byte)
                            int expectedTotalLength = 5 + payloadLength + 1;

                            // 5. Ensure the received buffer contains at least the full expected packet length
                            if (data.Length >= expectedTotalLength)
                            {
                                // 6. Extract the CRC byte (always the last byte of the specific frame)
                                byte receivedCrc = data[expectedTotalLength - 1];

                                // 7. Calculate local checksum for data integrity validation
                                // Standard AgIO checksum sums bytes starting from message ID up to the end of payload
                                byte calculatedCrc = 0;
                                for (int i = 2; i < expectedTotalLength - 1; i++)
                                {
                                    calculatedCrc += data[i];
                                }

                                // 8. Validate frame integrity and filter specifically for AgIO GPS Packets (ID: 0x01 0x01)
                                if (calculatedCrc == receivedCrc)
                                {
                                    if (msgId1 == 0x7C && msgId2 == 0xD6 && !isGNSSfromOG1)
                                    {
                                        // Extract data immediately to spend as little time as possible on the network thread
                                        ParseAgIoGpsPacket(data);
                                    }

                                    if (msgId1 == 0x6A && msgId2 == 0xD1)
                                    {
                                        // Extract data immediately to spend as little time as possible on the network thread
                                        isGNSSfromOG1 = true;
                                        ParseOG1GpsPacket(data);
                                    }

                                    if (msgId1 == 0x6A && msgId2 == 0xE1)
                                    {
                                        isDataFromOGudpBlade = true;
                                        // Extract data from blade module 1
                                        ParseOGBlade1Packet(data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException) { /* Normal socket closure */ }
            catch (Exception ex)
            {
                Debug.WriteLine($"UDP Network Error: {ex.Message}");
                isUDPNetworkConnected = false;
            }
        }

        private void ParseAgIoGpsPacket(byte[] data)
        {
            double lon = BitConverter.ToDouble(data, 5);
            double lat = BitConverter.ToDouble(data, 13);

            if (lon == double.MaxValue || lat == double.MaxValue) return;

            // Sécurisation de l'écriture dans l'objet partagé 'pn'
            lock (_positionLock)
            {
                pn.longitude = lon;
                pn.latitude = lat;
                pn.hemisphere = lat < 0 ? 'S' : 'N';

                // Calculs géospatiaux
                pn.ConvertWGS84ToLocal(pn.latitude, pn.longitude, out pn.northing, out pn.easting);

                // Cap (Heading)
                float tempHeading = BitConverter.ToSingle(data, 21);
                pn.headingTrue = (tempHeading != float.MaxValue) ? tempHeading : BitConverter.ToSingle(data, 25);

                // Vitesse (Speed)
                float tempSpeed = BitConverter.ToSingle(data, 29);
                if (tempSpeed != float.MaxValue) pn.speed = tempSpeed;

                // Roulis (Roll)
                float tempRoll = BitConverter.ToSingle(data, 33);
                if (tempRoll != float.MaxValue) pn.GPSroll = tempRoll;
                if (tempRoll == float.MinValue) pn.GPSroll = 0;

                // Altitude
                float tempAlt = BitConverter.ToSingle(data, 37);
                if (tempAlt != float.MaxValue) pn.altitude = tempAlt;

                // Satellites & Fix
                ushort sats = BitConverter.ToUInt16(data, 41);
                if (sats != ushort.MaxValue) pn.satellitesTracked = sats;

                byte fix = data[43];
                if (fix != byte.MaxValue) pn.fixQuality = fix;

                // HDOP & Âge de la correction
                ushort hdop = BitConverter.ToUInt16(data, 44);
                if (hdop != ushort.MaxValue) pn.hdop = hdop * 0.01;

                ushort age = BitConverter.ToUInt16(data, 46);
                if (age != ushort.MaxValue) pn.ageDiff = age * 0.01;

                // IMU Data
                short imuRol = BitConverter.ToInt16(data, 50);
                if (imuRol != short.MaxValue) pn.GPSroll = imuRol * 0.1;

                short imuPich = BitConverter.ToInt16(data, 52);
                if (imuPich != short.MaxValue) pn.GPSpitch = imuPich;

                short imuYaw = BitConverter.ToInt16(data, 54);
                if (imuYaw != short.MaxValue) pn.GPSyawRate = imuYaw;

                recvCounter = 0;

                // Moyenne glissante pour l'affichage
                avgSpeed[ringCounter] = pn.speed;
                if (ringCounter++ > 8) ringCounter = 0;
            }

            // Exécuter la logique de mise à jour graphique sur le thread de l'UI WinForms
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ProcessFixPosition));
            }
            else
            {
                ProcessFixPosition();
            }
        }

        private void ParseOG1GpsPacket(byte[] data)
        {
            // --- 1. PRE-EXTRACTION AND SENTINEL CHECK FOR COORDINATES ---
            // Byte 5 to 12: int64 longitude scaled by 1000,000,000
            long rawLon = BitConverter.ToInt64(data, 5);
            double lon = rawLon / 1000000000.0;

            // Byte 13 to 20: int64 latitude scaled by 1000,000,000
            long rawLat = BitConverter.ToInt64(data, 13);
            double lat = rawLat / 1000000000.0;

            // If coordinates match a sentinel value (e.g. 181°/361° scaled), abort early
            if (Math.Abs(lon) > 181.0 || Math.Abs(lat) > 91.0) return;

            // --- 2. THREAD-SAFE DATA POPULATION ---
            lock (_positionLock)
            {
                pn.longitude = lon;
                pn.latitude = lat;
                pn.hemisphere = lat < 0 ? 'S' : 'N';

                // Geospatial transformations
                pn.ConvertWGS84ToLocal(pn.latitude, pn.longitude, out pn.northing, out pn.easting);

                // Heading (Dual vs Single)
                // Byte 25: uint16 Dual Heading x100 | Byte 27: uint16 Single Heading x100
                ushort rawDualHeading = BitConverter.ToUInt16(data, 25);
                double dualHeading = rawDualHeading / 100.0;

                if (dualHeading <= 361.0)
                {
                    pn.headingTrue = (float)dualHeading;
                }
                else
                {
                    ushort rawSingleHeading = BitConverter.ToUInt16(data, 27);
                    double singleHeading = rawSingleHeading / 100.0;
                    if (singleHeading <= 361.0) pn.headingTrue = (float)singleHeading;
                }

                // Speed (Byte 31: uint16 Speed x100)
                ushort rawSpeed = BitConverter.ToUInt16(data, 31);
                double speed = rawSpeed / 100.0;
                if (speed <= 500.0) pn.speed = (float)speed; // Assuming max logical speed 500 km/h

                // Roll (Byte 29: int16 Roll x100)
                short rawRoll = BitConverter.ToInt16(data, 29);
                double roll = rawRoll / 100.0;
                if (Math.Abs(roll) <= 181.0) pn.GPSroll = (float)roll;

                // Altitude (Byte 21: int32 Altitude mm)
                int rawAltMm = BitConverter.ToInt32(data, 21);
                if (rawAltMm <= 20000000) // Lower than 20,000 meters in mm
                {
                    pn.altitude = (float)(rawAltMm / 1000.0);
                }

                // Satellites (Byte 46: single byte)
                byte sats = data[46];
                if (sats != 255) pn.satellitesTracked = sats;

                // Fix Quality (Byte 45: single byte)
                byte fix = data[45];
                if (fix != 255) pn.fixQuality = fix;

                // HDOP (Byte 41: uint16 HDOP x100)
                ushort rawHdop = BitConverter.ToUInt16(data, 41);
                double hdop = rawHdop / 100.0;
                if (hdop <= 100.0) pn.hdop = hdop;

                // Age of Correction (Byte 43: uint16 Age x100)
                ushort rawAge = BitConverter.ToUInt16(data, 43);
                double age = rawAge / 100.0;
                if (age <= 600.0) pn.ageDiff = age;

                // IMU Data (Bytes 35, 37, 39)
                // Byte 35: int16 IMU Roll x100
                short rawImuRoll = BitConverter.ToInt16(data, 35);
                double imuRoll = rawImuRoll / 100.0;
                if (Math.Abs(imuRoll) <= 181.0) pn.GPSroll = imuRoll; // Overwrites basic roll if valid

                // Byte 37: int16 IMU Pitch x100
                short rawImuPitch = BitConverter.ToInt16(data, 37);
                double imuPitch = rawImuPitch / 100.0;
                if (Math.Abs(imuPitch) <= 181.0) pn.GPSpitch = (float)imuPitch;

                // Byte 39: int16 IMU Yaw Rate x100
                short rawImuYaw = BitConverter.ToInt16(data, 39);
                double imuYaw = rawImuYaw / 100.0;
                if (Math.Abs(imuYaw) <= 319.0) pn.GPSyawRate = (float)imuYaw;

                // Reset timeout watchdog counter
                recvCounter = 0;

                // Moving average buffer update for UI smoothing
                avgSpeed[ringCounter] = pn.speed;
                if (ringCounter++ >= 8) ringCounter = 0;
            }

            // --- 3. UI THREAD SAFE DISPATCH ---
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ProcessFixPosition));
            }
            else
            {
                ProcessFixPosition();
            }
        }

        private void ParseOGBlade1Packet(byte[] data)
        {
            // Implementation for parsing OG Blade 1 packet
            bladeOffSetSlave = data[8];
            // to be added, 
        }

        public void SendPgnToLoop(byte[] byteData)
        {
            if (loopBackSocket == null || byteData.Length <= 2) return;

            try
            {
                int crc = 0;
                for (int i = 2; i + 1 < byteData.Length; i++)
                {
                    crc += byteData[i];
                }
                byteData[byteData.Length - 1] = (byte)crc;

                // Version moderne du renvoi de données asynchrone
                loopBackSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None,
                    epAgIO, SendAsyncLoopData, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur envoi UDP: {ex.Message}");
            }
        }

        private void SendAsyncLoopData(IAsyncResult asyncResult)
        {
            try
            {
                loopBackSocket?.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur fin envoi UDP: {ex.Message}");
            }
        }
    }
}

