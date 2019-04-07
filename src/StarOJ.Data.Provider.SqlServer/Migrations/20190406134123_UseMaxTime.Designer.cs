﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StarOJ.Data.Provider.SqlServer.Models;

namespace StarOJ.Data.Provider.SqlServer.Migrations
{
    [DbContext(typeof(OJContext))]
    [Migration("20190406134123_UseMaxTime")]
    partial class UseMaxTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.Problem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Hint");

                    b.Property<string>("Input");

                    b.Property<string>("Name");

                    b.Property<string>("Output");

                    b.Property<string>("Source");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedName");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.Submission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AcceptedCase");

                    b.Property<long>("CodeLength");

                    b.Property<bool>("HasIssue");

                    b.Property<string>("Issues");

                    b.Property<int>("Language");

                    b.Property<long?>("MaximumMemory");

                    b.Property<long?>("MaximumTime");

                    b.Property<int>("ProblemId");

                    b.Property<string>("SampleResults");

                    b.Property<int>("State");

                    b.Property<string>("TestResults");

                    b.Property<DateTimeOffset>("Time");

                    b.Property<int?>("TotalCase");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.TestCase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsSample");

                    b.Property<long>("MemoryLimit");

                    b.Property<int>("ProblemId");

                    b.Property<TimeSpan>("TimeLimit");

                    b.HasKey("Id");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedName");

                    b.Property<string>("PasswordHash");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StarOJ.Data.Provider.SqlServer.Models.WorkspaceInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Config");

                    b.HasKey("Id");

                    b.ToTable("WorkspaceInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
