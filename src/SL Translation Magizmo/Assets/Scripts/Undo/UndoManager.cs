using qASIC.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Undo
{
    public class UndoManager : MonoBehaviour, IEnumerable<UndoItem>
    {
        public List<UndoItem> Items { get; private set; } = new List<UndoItem>();
        public int Offset { get; private set; }

        [DynamicHelp(nameof(DebugText))]
        public InputMapItemReference i_undo;
        public InputMapItemReference i_redo;

        public UnityEvent OnUndo;
        public UnityEvent OnRedo;

        public event Action<object> OnChanged;

        string DebugText() =>
            $"Undo items count: {Items.Count}\n" +
            $"offset: {Offset}\n" +
            $"position: {GetHeadPosition()}";

        private void Update()
        {
            if (i_undo.GetInputDown())
                Undo();

            if (i_redo.GetInputDown())
                Redo();
        }

        public bool CanRedo() =>
            Items.Count > 0 &&
            Offset > 0;

        public bool CanUndo() =>
            Items.Count > 0 &&
            Offset < Items.Count;

        public int GetHeadPosition() =>
            Items.Count - Offset;

        public void UpdateLatestStep(object context = null)
        {
            OnChanged?.Invoke(context);
        }

        public void AddStep(UndoItem item, object context = null)
        {
            Items.RemoveRange(Items.Count - Offset, Offset);
            Offset = 0;
            Items.Add(item);
            OnChanged?.Invoke(context);
        }

        public void Undo(object context = null)
        {
            if (!CanUndo()) return;

            Offset = Mathf.Clamp(Offset + 1, 0, Items.Count);
            var index = GetHeadPosition();

            Items[index].Undo();

            if (Items[index].Skip && CanUndo())
            {
                Undo(context);
                return;
            }

            OnUndo.Invoke();
            OnChanged?.Invoke(context);
        }

        public void Redo(object context = null)
        {
            if (!CanRedo()) return;

            Offset = Mathf.Clamp(Offset - 1, 0, Items.Count);
            var index = GetHeadPosition();

            Items[index - 1].Redo();

            if (Items[index - 1].Skip && CanRedo())
            {
                Redo(context);
                return;
            }

            OnRedo.Invoke();
            OnChanged?.Invoke(context);
        }


        public bool IsDirty { get; }

        public void ClearDirty()
        {

        }

        public bool IsLatest(UndoItem item) =>
            item != null &&
            Items.LastOrDefault() == item;

        public IEnumerator<UndoItem> GetEnumerator() =>
            Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}