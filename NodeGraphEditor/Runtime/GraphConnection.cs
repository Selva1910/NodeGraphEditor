using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public struct GraphConnection
    {
        public GraphConnectionPort inputPort;
        public GraphConnectionPort outputPort;

        public GraphConnection(GraphConnectionPort inputPort, GraphConnectionPort outputPort)
        {
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }

        public GraphConnection(string inputPortId, int inputportIndex, string outputPortId, int outputportIndex)
        {
            inputPort = new GraphConnectionPort(inputPortId, inputportIndex);
            outputPort = new GraphConnectionPort(outputPortId, outputportIndex);
        }

    }
    [Serializable]
    public struct GraphConnectionPort
    {
        public string nodeId;
        public int portIndex;

        public GraphConnectionPort(string nodeId, int portindex)
        {
            this.nodeId = nodeId;
            this.portIndex = portindex;
        }
    }
}
