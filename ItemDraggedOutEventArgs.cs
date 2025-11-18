using System;

namespace MusicChange
{
	public class ItemDraggedOutEventArgs
	{
        public string FilePath
        {
            get;
        }
        public DateTime DragTime
        {
            get;
        }
        public bool Success
        {
            get;
        }

        public ItemDraggedOutEventArgs(string filePath, bool success = true)
        {
            FilePath = filePath;
            DragTime = DateTime.Now;
            Success = success;
        }
    }
}