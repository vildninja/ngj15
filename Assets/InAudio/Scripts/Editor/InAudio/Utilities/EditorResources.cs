using UnityEditor;
using UnityEngine;

namespace InAudioSystem
{
    public static class EditorResources
    {
        public static Texture Background;
        public static Texture White;

        public static Texture Plus;
        public static Texture Minus;

        public static Texture Up;
        public static Texture Down;

        public static Texture Bank;
        public static Texture Dice;
        public static Texture List;
        public static Texture Event;
        public static Texture Tree;
        public static Texture Audio;
        public static Texture Bus;

        public static Texture Folder;

        //Reorderable list icons
        public static Texture2D ContainerBackground;
        public static Texture2D ContainerBackgroundWhite;
        public static Texture2D GrabHandle;
        public static Texture2D TitleBackground;
        public static Texture2D ItemSplitter;

        public static void Reload()
        {
            
            if (Plus == null)
                Plus = LoadTexture("Plus");
            if (Minus == null)
                Minus = LoadTexture("Minus");
            if (Up == null)
                Up = LoadTexture("Up");
            if (Down == null)
                Down = LoadTexture("Down");
            if (Background == null)
                Background = LoadTexture("SelectedBackground"); 
            if (White == null)
                White = LoadTexture("White");

            if (Bank == null)
                Bank = LoadTexture("Bank");
            if (Dice == null)
                Dice = LoadTexture("Dice");
            if (List == null)
                List = LoadTexture("List");
            if (Event == null)
                Event = LoadTexture("Event");
            if (Tree == null)
                Tree = LoadTexture("Tree");
            if (Audio == null)
                Audio = LoadTexture("Audio");
            if (Folder == null)
                Folder = LoadTexture("Folder");
            if (Bus == null)
                Bus = LoadTexture("Bus");

            if (ContainerBackground == null)
                ContainerBackground = LoadTexture("ContainerBackground") as Texture2D;
            if (ContainerBackgroundWhite == null)
                ContainerBackgroundWhite = LoadTexture("ContainerBackgroundWhite") as Texture2D;
            if (GrabHandle == null) 
                GrabHandle = LoadTexture("GrabHandle") as Texture2D;
            if (TitleBackground == null)
                TitleBackground = LoadTexture("TitleBackground") as Texture2D;
            if (ItemSplitter == null)
                ItemSplitter = LoadTexture("ItemSplitter") as Texture2D;
        }

        private static Texture LoadTexture(string name)
        {
            return AssetDatabase.LoadAssetAtPath(FolderSettings.GetIconPath(name), typeof(Texture2D)) as Texture2D;
        }
    }

    
    
}
