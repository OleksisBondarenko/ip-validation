﻿// <auto-generated />
using System;
using ADValidation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ADValidation.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250417071259_AddedResourceNameToAuditRecords")]
    partial class AddedResourceNameToAuditRecords
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("ADValidation.Models.Audit.AuditData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuditRecordId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Hostname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuditRecordId")
                        .IsUnique();

                    b.ToTable("AuditData");
                });

            modelBuilder.Entity("ADValidation.Models.Audit.AuditRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AuditType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ResourceName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuditRecord");
                });

            modelBuilder.Entity("ADValidation.Models.Audit.AuditData", b =>
                {
                    b.HasOne("ADValidation.Models.Audit.AuditRecord", "AuditRecord")
                        .WithOne("AuditData")
                        .HasForeignKey("ADValidation.Models.Audit.AuditData", "AuditRecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuditRecord");
                });

            modelBuilder.Entity("ADValidation.Models.Audit.AuditRecord", b =>
                {
                    b.Navigation("AuditData");
                });
#pragma warning restore 612, 618
        }
    }
}
