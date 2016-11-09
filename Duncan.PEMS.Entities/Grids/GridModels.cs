using System;
using System.Collections.Generic;
using System.Text;

namespace Duncan.PEMS.Entities.Grids
{
    public class GridTemplateSet : List<GridTemplate>
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Version { get; set; }

    }

    public class GridController
    {
        public int CustomerGridsId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public string OriginalTitle { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public int OriginalPosition { get; set; }
        public bool IsHidden { get; set; }
    }


    public class GridTemplate
    {
        public GridTemplate()
        {
            Grid = new GridColumns();
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public int GridId { get; set; }
        public GridColumns Grid { get; set; }
        public bool Selected { get; set; }
        public int Version { get; set; }

    }

    public class GridColumns : List<GridData>
    {
        public override bool Equals(System.Object obj)
        {
            // If parameter is null, return false;
            if ( obj == null )
            {
                return false;
            }

            // If parameter cannot be cast to GridColumns return false.
            GridColumns that = obj as GridColumns;
            if ( that == null )
            {
                return false;
            }

            // Compare column sets.
            if ( this.Count != that.Count ) return false;

            for (int index = 0; index < this.Count; index++)
            {
                if ( this[index].OriginalPosition != that[index].OriginalPosition )
                    return false;
                //also check for hidden
                if (this[index].IsHidden != that[index].IsHidden)
                    return false;
            }

            // Return true if the fields match:
            return true;
        }
    }

    public class GridData : IComparable<GridData>
    {
        public string OriginalTitle { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public int OriginalPosition { get; set; }
        public bool IsHidden { get; set; }

        /// <summary>
        ///     Compare method
        /// </summary>
        /// <param name="other">
        ///     <see cref="GridData" /> to compare
        /// </param>
        /// <returns>
        ///     1 if <see cref="other" /> is larger, 0 if equal, -1 if less
        /// </returns>
        public int CompareTo(GridData other)
        {
            return this.Position.CompareTo( other.Position );
        }
    }
}