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
    
    public partial class Song
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Song()
        {
            this.Playlists = new HashSet<Playlist>();
        }
    
        public int idSong { get; set; }
        public string nameSong { get; set; }
        public int idSinger { get; set; }
        public int idKind { get; set; }
        public string urlSong { get; set; }
        public string imageSong { get; set; }
        public string created_date { get; set; }
    
        public virtual Author Author { get; set; }
        public virtual Kind Kind { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}