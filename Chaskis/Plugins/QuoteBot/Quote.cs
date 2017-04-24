﻿//
//          Copyright Seth Hendrick 2017.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System.Text;
using SQLite.Net.Attributes;

namespace Chaskis.Plugins.QuoteBot
{
    /// <summary>
    /// Represents a quote.
    /// </summary>
    public class Quote
    {
        // ---------------- Constructor ----------------

        public Quote()
        {
            this.Id = null;
            this.Author = string.Empty;
            this.QuoteText = string.Empty;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The ID of the quote in the database.
        /// Must be null for InsertOrReplace to work,
        /// otherwise we'll just consistently be modifying row 0.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long? Id { get; set; }

        /// <summary>
        /// Who said the quote.
        /// </summary>
        [Indexed]
        public string Author { get; set; }

        /// <summary>
        /// The text of the quote.
        /// </summary>
        public string QuoteText { get; set; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Ensures our config is valid before going into the database.
        /// </summary>
        /// <param name="errorString">Why we failed validation.</param>
        /// <returns>True if we are valid, else false.</returns>
        public bool TryValidate( out string errorString )
        {
            bool success = true;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine( "Could not validate Quote for the following reasons:" );

            if( string.IsNullOrEmpty( this.Author ) || string.IsNullOrWhiteSpace( this.Author ) )
            {
                success = false;
                builder.AppendLine( "\t-Author can not be null, empty, or whitespace." );
            }

            if( string.IsNullOrEmpty( this.QuoteText ) || string.IsNullOrWhiteSpace( this.QuoteText ) )
            {
                success = false;
                builder.AppendLine( "\t-Quote Text can not be null, empty, or whitespace." );
            }

            if( success )
            {
                errorString = string.Empty;
            }
            else
            {
                errorString = builder.ToString();
            }

            return success;
        }
    }
}