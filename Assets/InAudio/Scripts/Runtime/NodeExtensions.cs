using UnityEngine;

namespace InAudioSystem.ExtensionMethods
{
    public static class NodeExtensions
    {
        public static void AssignParent<T>(this T node, T newParent) where T : Object, InITreeNode<T>
        {
            if (node != null && newParent != null)
            {
                newParent.GetChildren.Add(node);
                node.GetParent = newParent;
            }
        }
    }

    public static class AudioNodeExtensions
    {
        public static InAudioBus GetBus(this InAudioNode node)
        {
            if (node == null)
                return null;
            if (node.OverrideParentBus || node.IsRoot)
            {
                return node.Bus;
            }
            else
                return GetBus(node.Parent);
        }

        public static InAudioBankLink GetBank(this InAudioNode node)
        {
            var data = node.NodeData as InFolderData;
            if (node.IsRoot)
                return data.BankLink;

            if (node.Type == AudioNodeType.Folder && data.OverrideParentBank && data.BankLink != null)
                return data.BankLink;

            return GetBank(node.Parent);
        }

        public static InAudioBank GetBankDirect(this InAudioNode node)
        {
            var data = node.NodeData as InFolderData;
            if (node.IsRoot)
            {
                var link = data.BankLink;
                if (link != null)
                    return link.LazyBankFetch;
            }

            if (node.Type == AudioNodeType.Folder && data.OverrideParentBank && data.BankLink != null)
            {
                var link = data.BankLink;
                if (link != null)
                    return link.LazyBankFetch;
            }

            return GetBankDirect(node.Parent);
        }
    }
}