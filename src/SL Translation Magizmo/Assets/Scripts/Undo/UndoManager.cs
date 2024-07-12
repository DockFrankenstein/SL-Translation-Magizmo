using qASIC.Input;
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

        public void AddStep(UndoItem item)
        {
            Items.RemoveRange(Items.Count - Offset, Offset);
            Offset = 0;
            Items.Add(item);
        }

        public void Undo()
        {
            if (Items.Count == 0)
                return;

            Offset = Mathf.Clamp(Offset + 1, 0, Items.Count);
            var index = GetHeadPosition();

            Items[index].Undo();

            OnUndo.Invoke();
        }

        public void Redo()
        {
            Offset = Mathf.Clamp(Offset - 1, 0, Items.Count);
            var index = GetHeadPosition();

            Items[index - 1].Redo();

            OnRedo.Invoke();
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