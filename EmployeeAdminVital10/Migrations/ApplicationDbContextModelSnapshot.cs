﻿// <auto-generated />
using System;
using EmployeeAdminVital10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeAdminVital10.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("EmployeeAdminVital10.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PartnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PartnerId");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("EmployeeAdminVital10.Models.Employee", b =>
                {
                    b.HasOne("EmployeeAdminVital10.Models.Employee", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerId");
                });
#pragma warning restore 612, 618
        }
    }
}
