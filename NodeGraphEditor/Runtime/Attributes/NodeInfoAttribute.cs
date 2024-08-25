using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class NodeInfoAttribute : Attribute
    {

        private string m_nodeTitle;
        private string m_menuItem;
        private bool m_hasFlowInputs;
        private bool m_hasFlowOutputs;
        public bool hasFlowInputs => m_hasFlowInputs;
        public bool hasFlowOutputs => m_hasFlowOutputs;

        public string title => m_nodeTitle;
        public string menuItem => m_menuItem;

        public NodeInfoAttribute(string nodeTitle, string menuItem = "", bool hasFlowInputs = false, bool hasFlowOutputs = false)
        {
            m_nodeTitle = nodeTitle;
            m_menuItem = menuItem;
            m_hasFlowInputs = hasFlowInputs;
            m_hasFlowOutputs = hasFlowOutputs;
        }
    }
}
