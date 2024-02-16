using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Project.Serialization
{
    [Serializable]
    public class CsvParser
    {
        public CsvParser()
        {

        }

        [SerializeField] string wrappingCharacter = "\"";
        [SerializeField] string cellSeparator = ",";

        /// <summary>String used for wrapping cells whenever they contain any special character (default: ").</summary>
        public string CellWrapping
        {
            get => wrappingCharacter;
            set => wrappingCharacter = value;
        }

        /// <summary>String used for splitting cells (default: ,).</summary>
        public string CellSeparator
        {
            get => cellSeparator;
            set => cellSeparator = value;
        }

        public Table2D Deserialize(string txt)
        {
            txt = txt
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            var sections = new string[] { txt }
                .SplitWithSplits(wrappingCharacter)
                .SplitWithSplits(CellSeparator)
                .SplitWithSplits("\n")
                .ToArray();

            var table = new Table2D();

            string currentCell = string.Empty;

            //Determines if current cell is surrounded by quotation marks
            bool isWrapped = false;

            uint column = 0;
            uint row = 0;

            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i] == wrappingCharacter)
                {
                    //If it's the first item of the cell, mark it as wrapped
                    if (string.IsNullOrEmpty(currentCell) && !isWrapped)
                    {
                        isWrapped = true;
                        continue;
                    }

                    //If cell is not wrapped, then symbol is part of the cell
                    if (!isWrapped)
                    {
                        currentCell += sections[i];
                        continue;
                    }

                    //If last cell, finish
                    if (i == sections.Length - 1)
                    {
                        continue;
                    }

                    //If combined with the next one section is: ""
                    //then add it to cell content as a single quotation "
                    if (sections[i + 1] == wrappingCharacter)
                    {
                        i++;
                        currentCell += wrappingCharacter;
                        continue;
                    }

                    //If it's a single quotation mark, escape wrapping
                    isWrapped = false;
                    continue;
                }

                //If cell is wrapped, treat every character as part of cell, until
                //cell escapes wrapping
                if (isWrapped)
                {
                    currentCell += sections[i];
                    continue;
                }

                //If it's a new line, push current and move to the next row
                if (sections[i] == "\n")
                {
                    PushCell();
                    row++;
                    column = 0;
                    continue;
                }

                if (sections[i] == CellSeparator)
                {
                    //If it's not wrapped, move to the next cell
                    PushCell();
                    continue;
                }

                currentCell += sections[i];
            }

            if (!string.IsNullOrEmpty(currentCell))
                PushCell();

            return table;

            void PushCell()
            {
                table.SetCell(column, row, currentCell);
                currentCell = string.Empty;
                column++;
            }
        }

        public string Serialize(Table2D table)
        {
            var txt = new StringBuilder();

            for (uint row = 0; row < table.RowsCount; row++)
            {
                List<string> cells = new List<string>();

                for (uint column = 0; column < table.ColumnsCount; column++)
                {
                    var cell = table.GetCell(column, row);

                    if (cell.Contains(CellSeparator) || cell.Contains("\"") || cell.Contains("\n"))
                        cell = $"\"{cell.Replace("\"", "\"\"")}\"";

                    cells.Add(cell);
                }

                txt.Append($"{string.Join(CellSeparator, cells)}\n");
            }

            return txt.ToString();
        }
    }
}