﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.DAO.Models
{
    [Index("CNPJ", Name = "IX_Empresa_CNPJ", IsUnique = true)]
    public partial class Empresa
    {
        public Empresa()
        {
            Usuario = new HashSet<Usuario>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string RazaoSocial { get; set; }
        [Required]
        [StringLength(150)]
        public string NomeFantasia { get; set; }
        [Required]
        [StringLength(14)]
        public string CNPJ { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativa { get; set; }

        [InverseProperty("IdEmpresaNavigation")]
        public virtual EmpresaLogo EmpresaLogo { get; set; }
        [InverseProperty("IdEmpresaNavigation")]
        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}