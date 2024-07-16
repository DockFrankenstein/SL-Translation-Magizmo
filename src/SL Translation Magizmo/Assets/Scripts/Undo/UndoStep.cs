using System;

namespace Project.Undo
{
    [Serializable]
    public abstract class UndoStep
    {
        public abstract void Undo();

        public abstract void Redo();

        public object Context { get; set; }

        public virtual bool Skip => false;
    }

    public class UndoStep<T> : UndoStep
    {
        public UndoStep() { }
        public UndoStep(T oldValue, T newValue, Action<T> applyValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            ApplyValue = applyValue;
        }

        public UndoStep(Action<T> applyValue) : this(default, default, applyValue) { }
        public UndoStep(T oldValue, Action<T> applyValue) : this(oldValue, default, applyValue) { }

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

    public class UndoSaveStep : UndoStep
    {
        public override void Undo() { }
        public override void Redo() { }

        public override bool Skip => true;
    }
}