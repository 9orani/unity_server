using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RenderStreaming.Signaling;
using System.Threading;

namespace Gorani.Classroom {
    internal enum SignalingType
    {
        WebSocket,
        Http,
        Furioos
    }

    internal static class RenderStreamingSettings
    {
        private static bool s_enableHWCodec = false;
        private static SignalingType s_signalingType = SignalingType.WebSocket;
        private static string s_signalingAddress = "127.0.0.1"; 
        private static float s_signalingInterval = 5;
        private static bool s_signalingSecured = false;

        public static bool EnableHWCodec
        {
            get { return s_enableHWCodec; }
            set { s_enableHWCodec = value; }
        }

        public static SignalingType SignalingType
        {
            get { return s_signalingType; }
            set { s_signalingType = value; }
        }

        public static string SignalingAddress
        {
            get { return s_signalingAddress; }
            set { s_signalingAddress = value; }
        }

        public static bool SignalingSecured
        {
            get { return s_signalingSecured; }
            set { s_signalingSecured = value; }
        }

        public static float SignalingInterval
        {
            get { return s_signalingInterval; }
            set { s_signalingInterval = value; }
        }

        public static ISignaling Signaling
        {
            get
            {
                switch (s_signalingType)
                {
                    case SignalingType.Furioos:
                    {
                        var schema = s_signalingSecured ? "https" : "http";
                        return new FurioosSignaling(
                            $"{schema}://{s_signalingAddress}", s_signalingInterval, SynchronizationContext.Current);
                    }
                    case SignalingType.WebSocket:
                    {
                        var schema = s_signalingSecured ? "wss" : "ws";
                        return new WebSocketSignaling(
                            $"{schema}://{s_signalingAddress}", s_signalingInterval, SynchronizationContext.Current);
                    }
                    case SignalingType.Http:
                    {
                        var schema = s_signalingSecured ? "https" : "http";
                        return new HttpSignaling(
                            $"{schema}://{s_signalingAddress}", s_signalingInterval, SynchronizationContext.Current);
                    }
                }
                throw new InvalidOperationException();
            }
        }
    }
}