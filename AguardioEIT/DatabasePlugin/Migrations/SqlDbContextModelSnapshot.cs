﻿// <auto-generated />
using System;
using DatabasePlugin.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabasePlugin.Migrations
{
    [DbContext(typeof(SqlDbContext))]
    partial class SqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Common.Models.LeakSensorData", b =>
                {
                    b.Property<int>("DataRawId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DataRawId"));

                    b.Property<DateTime>("DCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DLifeTimeUseCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DReported")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("DTemperatureIn")
                        .HasColumnType("double precision");

                    b.Property<double>("DTemperatureOut")
                        .HasColumnType("double precision");

                    b.Property<int>("LeakLevelId")
                        .HasColumnType("integer");

                    b.Property<int>("SensorId")
                        .HasColumnType("integer");

                    b.HasKey("DataRawId");

                    b.ToTable("LeakSensorData");
                });
#pragma warning restore 612, 618
        }
    }
}