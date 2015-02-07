using System.Collections.Generic;

namespace InAudioSystem
{

    public interface InITreeNode<T> where T : UnityEngine.Object, InITreeNode<T>
    {
        T GetParent { get; set; }

        List<T> GetChildren { get; }

        bool IsRoot { get; }

        string GetName { get; }

        int ID { get; set; }

#if UNITY_EDITOR
        bool IsFoldedOut { get; set; }

        bool IsFiltered { get; set; }
#endif
    }
}