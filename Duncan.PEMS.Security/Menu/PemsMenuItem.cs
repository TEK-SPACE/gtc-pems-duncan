using System;
using System.Collections.Generic;

namespace Duncan.PEMS.Security.Menu
{
    /// <summary>
    /// Represents a single menu item.  Inherits from <see cref="System.Collections.Generic.List{T}"/> 
    /// so a menu item may contain a list of child menu items.
    /// </summary>
    public class PemsMenuItem : List<PemsMenuItem>, IComparable<PemsMenuItem>
    {
        /// <summary>
        /// String reprenting the text of the menu item.
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// String reprenting the text of the menu item.
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// String reprenting an icon location for the menu item.
        /// </summary>
        public string Icon
        {
            get;
            set;
        }

        /// <summary>
        /// String representing an optional tool tip for the menu item.
        /// </summary>
        public string ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// String representing the url associated with the menu item.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// String representing the url associated with the menu item.
        /// </summary>
        public string MenuUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Boolean representing whether the menu link should be opened
        /// in a new window or tab
        /// </summary>
        public bool NewWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Integer representing orde of this menu item in the list in
        /// which it has siblings.
        /// </summary>
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Compare method
        /// </summary>
        /// <param name="other"><see cref="PemsMenuItem"/> to compare</param>
        /// <returns>1 if <see cref="other"/> is larger, 0 if equal, -1 if less</returns>
        public int CompareTo(PemsMenuItem other)
        {
            return this.Order.CompareTo( other.Order );
        }


        /// <summary>
        /// Checks <see cref="PemsMenuItem"/> to see if it has a valid URL
        /// associated with it.
        /// </summary>
        /// <returns>True if URL has been populated.</returns>
        public bool HasLink()
        {
            // Note:  This is somewhat arbitrary but serves the purpose.
            return Url != null && Url.Length > 3;
        }


        /// <summary>
        /// Override of ToString().  Primarily used for debugging.
        /// </summary>
        /// <returns>String representation of menu item.</returns>
        public override string ToString()
        {
            return "Label: " + Label + ", Order: " + Order + ", URL: " + (Url ?? "[null]") + ", ToolTip: " + (ToolTip ?? "[null]");
        }

    }
}
