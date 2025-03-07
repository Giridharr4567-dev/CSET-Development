﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

[PrimaryKey("Assessment_Id", "Id")]
public partial class NETWORK_WARNINGS
{
    [Key]
    public int Assessment_Id { get; set; }

    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string WarningText { get; set; }

    [ForeignKey("Assessment_Id")]
    [InverseProperty("NETWORK_WARNINGS")]
    public virtual ASSESSMENTS Assessment { get; set; }
}