using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class SearchHelper
{

    public static void SearchFor(AudioEventAction action)
    {
        var audioAction = action as InEventAudioAction;
        if (audioAction != null && audioAction.Node != null)
        {
            EditorWindow.GetWindow<InAudioWindow>().Find(audioAction.Node);
        }
        var busAction = action as InEventBusAction;
        if (busAction != null && busAction.Bus != null)
        {
            EditorWindow.GetWindow<AuxWindow>().FindBus(busAction.Bus);
        }
        var busMuteAction = action as InEventBusMuteAction;
        if (busMuteAction != null && busMuteAction.Bus != null)
        {
            EditorWindow.GetWindow<AuxWindow>().FindBus(busMuteAction.Bus);
        }
        var bankAction = action as InEventBankLoadingAction;
        if (bankAction != null && bankAction.BankLink != null)
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(bankAction.BankLink);
        }
    }

    public static void SearchFor(InAudioBus bus)
    {
        EditorWindow.GetWindow<AuxWindow>().FindBus(bus);

    }

    public static void SearchFor(InAudioBankLink bank)
    {
        EditorWindow.GetWindow<AuxWindow>().FindBank(bank);

    }

    public static void SearchFor(InAudioNode node)
    {
        EditorWindow.GetWindow<InAudioWindow>().Find(node);
    }

    public static void SearchForObject<T>(T node) where T : Object, InITreeNode<T>
    {
        if (node is InAudioBus)
            SearchFor(node as InAudioBus);
        if (node is InAudioNode)
            SearchFor(node as InAudioNode);
        if (node is InAudioBankLink)
            SearchFor(node as InAudioBankLink);
    }
}
}