using System;
using InAudioSystem.ReorderableList;
using UnityEngine;

namespace InAudioSystem
{
    public class EventActionListAdapter : IReorderableListAdaptor
    {
        public InAudioEventNode Event;
        public Action<Rect, int> DrawEvent;
        public Action<int> ClickedInArea;

        public EventActionListAdapter() { }

        public int Count
        {
            get { return Event.ActionList.Count; }
        }

        public bool CanDrag(int index)
        {
            return true;
        }

        public bool CanRemove(int index)
        {
            return false;
        }

        public void Add()
        {
            Debug.Log("Add");
            
        }

        public void Insert(int index)
        {
            //throw new System.NotImplementedException();
        }

        public void Duplicate(int index)
        {
            //throw new System.NotImplementedException();
        }

        public void Remove(int index)
        {
            //throw new System.NotImplementedException();
        }

        public void Move(int sourceIndex, int destIndex)
        {
            UndoHelper.RecordObjectFull(Event, "Reorder Event Actions");
            if (destIndex > sourceIndex)
                --destIndex;

            var item = Event.ActionList[sourceIndex];
            Event.ActionList.RemoveAt(sourceIndex);
            Event.ActionList.Insert(destIndex, item);

        }

        public void Clear()
        {
            
        }

        public void DrawItem(Rect position, int index)
        {
            DrawEvent(position, index);
        }

        public float GetItemHeight(int index)
        {
            return 17;
        }

        public void ClickedIn(int index)
        {
            ClickedInArea(index);
        }
    }

}