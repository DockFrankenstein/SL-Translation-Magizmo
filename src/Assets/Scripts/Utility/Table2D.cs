using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Project
{
    /// <summary>Collection of cells contained in a 2D table.</summary>
    public class Table2D : IEnumerable<Table2D.Cell>
    {
        /// <summary>Creates a new table.</summary>
        public Table2D() { }

        /// <summary>Creates a new table.</summary>
        /// <param name="cells">List of rows containing cells/</param>
        public Table2D(List<List<string>> cells)
        {
            for (int y = 0; y < cells.Count; y++)
                for (int x = 0; x < cells[y].Count; x++)
                    SetCell((uint)x, (uint)y, cells[y][x]);
        }

        /// <summary>Creates a new table.</summary>
        /// <param name="cells">List of rows containing cells/</param>
        public Table2D(string[][] cells)
        {
            for (int y = 0; y < cells.Length; y++)
                for (int x = 0; x < cells[y].Length; x++)
                    SetCell((uint)x, (uint)y, cells[y][x]);
        }

        Dictionary<uint, Dictionary<uint, string>> cells = new Dictionary<uint, Dictionary<uint, string>>();

        /// <summary>Amount of columns in the table.</summary>
        public uint ColumnsCount { get; private set; }
        /// <summary>Amount of rows in the table</summary>
        public uint RowsCount { get; private set; }

        public IEnumerator<Cell> GetEnumerator() =>
            cells
            .SelectMany(x => x.Value.Select(y => new Cell(x.Key, y.Key, y.Value)))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public override string ToString()
        {
            var cellContent = cells.SelectMany((x, column) => x.Value.Select((y, row) => $"{row}x{column}:{y.Value}"));

            return $"Table {ColumnsCount}x{RowsCount}, content: {string.Join("\n", cellContent)}";
        }

        public override int GetHashCode() =>
            cells.GetHashCode();

        /// <summary>Gets the content of a cell from the table.</summary>
        /// <param name="xPosition">X position of the cell.</param>
        /// <param name="yPosition">Y position of the cell.</param>
        /// <returns>Returns the contents of the cell.</returns>
        public string GetCell(int xPosition, int yPosition)
        {
            if (xPosition < 0 || yPosition < 0)
                throw new ArgumentOutOfRangeException("Grid position cannot be negative!");

            return GetCell((uint)xPosition, (uint)yPosition);
        }

        /// <summary>Gets the content of a cell from the table.</summary>
        /// <param name="xPosition">X position of the cell.</param>
        /// <param name="yPosition">Y position of the cell.</param>
        /// <returns>Returns the contents of the cell.</returns>
        public string GetCell(uint xPosition, uint yPosition) =>
            cells.ContainsKey(yPosition) && cells[yPosition].ContainsKey(xPosition) ?
            cells[yPosition][xPosition] :
            string.Empty;

        /// <summary>Sets the content of a cell in the table.</summary>
        /// <param name="xPosition">X position of the cell.</param>
        /// <param name="yPosition">Y position of the cell.</param>
        /// <param name="cellContent">The contents of the cell.</param>
        public void SetCell(int xPosition, int yPosition, string cellContent)
        {
            if (xPosition < 0 || yPosition < 0)
                throw new ArgumentOutOfRangeException("Grid position cannot be negative!");

            SetCell((uint)xPosition, (uint)yPosition, cellContent);
        }

        /// <summary>Sets the content of a cell in the table.</summary>
        /// <param name="xPosition">X position of the cell.</param>
        /// <param name="yPosition">Y position of the cell.</param>
        /// <param name="cellContent">The contents of the cell.</param>
        public void SetCell(uint xPosition, uint yPosition, string cellContent)
        {
            if (!cells.ContainsKey(yPosition))
                cells.Add(yPosition, new Dictionary<uint, string>());

            if (!cells[yPosition].ContainsKey(xPosition))
                cells[yPosition].Add(xPosition, string.Empty);

            cells[yPosition][xPosition] = cellContent ?? string.Empty;

            if (xPosition >= ColumnsCount)
                ColumnsCount = xPosition + 1;

            if (yPosition >= RowsCount)
                RowsCount = yPosition + 1;
        }

        public struct Cell
        {
            internal Cell(uint xPosition, uint yPosition, string content)
            {
                this.xPosition = xPosition;
                this.yPosition = yPosition;
                this.content = content;
            }

            public uint xPosition;
            public uint yPosition;
            public string content;
        }
    }
}