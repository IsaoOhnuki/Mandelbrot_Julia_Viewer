using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UndoList<T>
    {
        List<T> undoList;
        int undoPos;
        public bool CanUndo { get { return undoPos > 0; } }
        public bool CanRedo { get { return undoPos < undoList.Count - 1; } }
        public T Last { get { return undoList[undoPos]; } }
        public bool HasLast { get { return undoPos > -1; } }
        public UndoList()
        {
            undoList = new List<T>();
            undoPos = -1;
        }
        public UndoList(T[] array)
        {
            undoList = new List<T>(array);
            undoPos = undoList.Count - 1;
        }
        public void Push(T value)
        {
            while (undoList.Count > undoPos + 1)
                undoList.RemoveAt(undoList.Count - 1);
            undoList.Add(value);
            ++undoPos;
        }
        public T Undo()
        {
            return undoList[--undoPos];
        }
        public T Redo()
        {
            return undoList[++undoPos];
        }
    }
}
