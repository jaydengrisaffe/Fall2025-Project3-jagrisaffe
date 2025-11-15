using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2025_project3_jagrisaffe.Models
{
    public class ActorMovie
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        public Actor? Actor { get; set; }
        public Movie? Movie { get; set; }
    }
}