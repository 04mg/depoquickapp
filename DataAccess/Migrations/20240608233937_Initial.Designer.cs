﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20240608233937_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DepositPromotion", b =>
                {
                    b.Property<string>("DepositName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PromotionsId")
                        .HasColumnType("int");

                    b.HasKey("DepositName", "PromotionsId");

                    b.HasIndex("PromotionsId");

                    b.ToTable("DepositPromotion", (string)null);
                });

            modelBuilder.Entity("Domain.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClientEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DepositName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int");

                    b.Property<int>("Stage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientEmail");

                    b.HasIndex("DepositName");

                    b.HasIndex("PaymentId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("Domain.Deposit", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Area")
                        .HasColumnType("int");

                    b.Property<bool>("ClimateControl")
                        .HasColumnType("bit");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("Deposits");
                });

            modelBuilder.Entity("Domain.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Domain.Promotion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("Domain.User", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NameSurname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.HasKey("Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DepositPromotion", b =>
                {
                    b.HasOne("Domain.Deposit", null)
                        .WithMany()
                        .HasForeignKey("DepositName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Promotion", null)
                        .WithMany()
                        .HasForeignKey("PromotionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Booking", b =>
                {
                    b.HasOne("Domain.User", "Client")
                        .WithMany()
                        .HasForeignKey("ClientEmail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Deposit", "Deposit")
                        .WithMany()
                        .HasForeignKey("DepositName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Payment", "Payment")
                        .WithMany()
                        .HasForeignKey("PaymentId");

                    b.OwnsOne("DateRange.DateRange", "Duration", b1 =>
                        {
                            b1.Property<int>("BookingId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("EndDate")
                                .HasColumnType("date");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<DateTime>("StartDate")
                                .HasColumnType("date");

                            b1.HasKey("BookingId");

                            b1.ToTable("Bookings");

                            b1.WithOwner()
                                .HasForeignKey("BookingId");
                        });

                    b.Navigation("Client");

                    b.Navigation("Deposit");

                    b.Navigation("Duration")
                        .IsRequired();

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("Domain.Deposit", b =>
                {
                    b.OwnsOne("Domain.AvailabilityPeriods", "AvailabilityPeriods", b1 =>
                        {
                            b1.Property<string>("DepositName")
                                .HasColumnType("nvarchar(450)");

                            b1.HasKey("DepositName");

                            b1.ToTable("Deposits");

                            b1.WithOwner()
                                .HasForeignKey("DepositName");

                            b1.OwnsMany("DateRange.DateRange", "AvailablePeriods", b2 =>
                                {
                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("AvailabilityPeriodsDepositName")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(450)");

                                    b2.Property<DateTime>("EndDate")
                                        .HasColumnType("date");

                                    b2.Property<DateTime>("StartDate")
                                        .HasColumnType("date");

                                    b2.HasKey("Id");

                                    b2.HasIndex("AvailabilityPeriodsDepositName");

                                    b2.ToTable("Deposits_AvailablePeriods");

                                    b2.WithOwner()
                                        .HasForeignKey("AvailabilityPeriodsDepositName");
                                });

                            b1.OwnsMany("DateRange.DateRange", "UnavailablePeriods", b2 =>
                                {
                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("AvailabilityPeriodsDepositName")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(450)");

                                    b2.Property<DateTime>("EndDate")
                                        .HasColumnType("date");

                                    b2.Property<DateTime>("StartDate")
                                        .HasColumnType("date");

                                    b2.HasKey("Id");

                                    b2.HasIndex("AvailabilityPeriodsDepositName");

                                    b2.ToTable("Deposits_UnavailablePeriods");

                                    b2.WithOwner()
                                        .HasForeignKey("AvailabilityPeriodsDepositName");
                                });

                            b1.Navigation("AvailablePeriods");

                            b1.Navigation("UnavailablePeriods");
                        });

                    b.Navigation("AvailabilityPeriods")
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Promotion", b =>
                {
                    b.OwnsOne("DateRange.DateRange", "Validity", b1 =>
                        {
                            b1.Property<int>("PromotionId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("EndDate")
                                .HasColumnType("date");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<DateTime>("StartDate")
                                .HasColumnType("date");

                            b1.HasKey("PromotionId");

                            b1.ToTable("Promotions");

                            b1.WithOwner()
                                .HasForeignKey("PromotionId");
                        });

                    b.Navigation("Validity")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
