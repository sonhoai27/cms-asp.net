//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Imusik
{
    using System;
    using System.Collections.Generic;
    
    public partial class Playlist
    {
        public int idPlaylist { get; set; }
        public string imagePlaylist { get; set; }
        public string namePlaylist { get; set; }
        public int idSong { get; set; }
        public string created_date { get; set; }
        public int idUser { get; set; }
    
        public virtual Song Song { get; set; }
        public virtual User User { get; set; }
    }
}