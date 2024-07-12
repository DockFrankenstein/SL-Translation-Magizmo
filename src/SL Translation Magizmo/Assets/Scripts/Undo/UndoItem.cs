using System;

namespace Project.Undo
{
    [Serializable]
    public abstract class UndoItem
    {
        public abstract void Undo();

        public abstract void Redo();
    }

    public class UndoItem<T> : UndoItem
    {
        public UndoItem() { }
        public UndoItem(T oldValue, T newValue, Action<T> applyValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            ApplyValue = applyValue;
        }

        public UndoItem(Action<T> applyValue) : this(default, default, applyValue) { }
        public UndoItem(T oldValue, Action<T> applyValue) : this(oldValue, default, applyValue) { }

        public event Action<T> ApplyValue;
        public T oldValue;
        public T newValue;

        public override void Undo()
        {
            ApplyValue.Invoke(oldValue);
        }

        public override void Redo()
        {
            ApplyValue.Invoke(newValue);
        }
    }
}