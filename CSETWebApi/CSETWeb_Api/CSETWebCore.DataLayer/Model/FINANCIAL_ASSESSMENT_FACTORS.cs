﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

/// <summary>
/// A collection of FINANCIAL_ASSESSMENT_FACTORS records
/// </summary>
[Index("AssessmentFactor", Name = "IX_FINANCIAL_ASSESSMENT_FACTORS", IsUnique = true)]
public partial class FINANCIAL_ASSESSMENT_FACTORS
{
    [Key]
    public int AssessmentFactorId { get; set; }

    [Required]
    [StringLength(255)]
    public string AssessmentFactor { get; set; }

    public int AssessmentFactorWeight { get; set; }

    [StringLength(50)]
    public string Acronym { get; set; }

    [InverseProperty("AssessmentFactor")]
    public virtual ICollection<FINANCIAL_GROUPS> FINANCIAL_GROUPS { get; set; } = new List<FINANCIAL_GROUPS>();
}