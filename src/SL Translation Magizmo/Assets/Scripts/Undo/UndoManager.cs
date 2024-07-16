using qASIC;
using qASIC.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Undo
{
    public class UndoManager : MonoBehaviour, IEnumerable<UndoStep>
    {
        public List<UndoStep> Steps { get; private set; } = new List<UndoStep>();
        public int Offset { get; private set; }

        [DynamicHelp(nameof(DebugText))]
        public InputMapItemReference i_undo;
        public InputMapItemReference i_redo;

        public UnityEvent OnUndo;
        public UnityEvent OnRedo;

        public event Action<object> OnChanged;

        string DebugText() =>
            $"Undo items count: {Steps.Count}\n" +
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
            Steps.Count > 0 &&
            Offset > 0;

        public bool CanUndo() =>
            Steps.Count > 0 &&
            Offset < Steps.Count;

        public int GetHeadPosition() =>
            Steps.Count - Offset;

        private void ClampOffset() =>
            Mathf.Clamp(Offset + 1, 0, Steps.Count);

        public void UpdateLatestStep(object context = null)
        {
            OnChanged?.Invoke(context);
        }

        public void AddStep(UndoStep item, object context = null)
        {
            Steps.RemoveRange(Steps.Count - Offset, Offset);
            Offset = 0;
            Steps.Add(item);
            OnChanged?.Invoke(context);
        }

        public void Undo(object context = null)
        {
            if (!CanUndo()) return;

            Offset++;
            ClampOffset();
            var index = GetHeadPosition();

            Steps[index].Undo();

            if (Steps[index].Skip && CanUndo())
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

            Offset--;
            ClampOffset();
            var index = GetHeadPosition();

            Steps[index - 1].Redo();

            if (Steps.IndexInRange(index) &&
                Steps[index].Skip && 
                CanRedo())
            {
                Redo(context);
                return;
            }

            OnRedo.Invoke();
            OnChanged?.Invoke(context);
        }


        public bool IsDirty => Steps.Any(x => x is UndoSaveStep) ?
            !(Steps[Mathf.Max(0, GetHeadPosition() - 1)] is UndoSaveStep) :
            GetHeadPosition() > 0;

        public void ClearDirty(object context = null)
        {
            var saveItems = Steps.Where(x => x is UndoSaveStep)
                .ToList();

            foreach (var item in saveItems)
            {
                var index = Steps.IndexOf(item);
                var headPos = GetHeadPosition();

                if (index > headPos)
                {
                    Offset--;
                }

                Steps.Remove(item);
                ClampOffset();
            }

            Steps.Insert(GetHeadPosition(), new UndoSaveStep());
            OnChanged?.Invoke(context);
        }

        public void ClearAll(object context = null)
        {
            Steps.Clear();
            Offset = 0;
            OnChanged?.Invoke(context);
        }

        public bool IsLatest(UndoStep item) =>
            item != null &&
            Steps.LastOrDefault() == item;

        public IEnumerator<UndoStep> GetEnumerator() =>
            Steps.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}