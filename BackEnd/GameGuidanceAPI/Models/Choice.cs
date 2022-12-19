﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameGuidanceAPI.Models
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApiChoiceId { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ChoiceName { get; set; }
    }
}