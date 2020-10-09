using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDiary.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Display(Name = "Testo nel diario")]
        [StringLength(8000, MinimumLength = 3, ErrorMessage = "Il testo dell'evento deve avere almeno 3 lettere")]
        public string event_text { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data")]
        public DateTime event_date { get; set; }

        // user ID from AspNetUser table.

        [StringLength(450)]
        [Display(Name = "Creato da")]
        public string created_user_id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Creato il")]
        public DateTime created_on { get; set; }
        // user ID from AspNetUser table.
        [StringLength(450)]
        [Display(Name = "Modificato da")]
        public string? modified_user_id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Modificato il")]
        public DateTime? modified_on { get; set; }
    }
}
