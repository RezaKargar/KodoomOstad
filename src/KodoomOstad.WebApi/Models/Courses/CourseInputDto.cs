﻿using KodoomOstad.Entities.Models;
using KodoomOstad.IocConfig.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace KodoomOstad.WebApi.Models.Courses
{
    public class CourseInputDto : IMapTo<Course>
    {
        [Required]
        public string Title { get; set; }

        public decimal Grade { get; set; }

        public int Year { get; set; }

        public int Term { get; set; }

        public int UserId { get; set; }

        public int ProfessorId { get; set; }
    }
}
