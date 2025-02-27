﻿using MadaniOstad.Entities.Models;
using MadaniOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace MadaniOstad.WebApi.Models.Professors
{
    public class ProfessorInputDto : IMapTo<Professor>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public ProfessorRank Rank { get; set; }

        public string Avatar { get; set; }

        public bool IsFacultyMember { get; set; }

        [Required]
        public int FacultyId { get; set; }
    }
}
