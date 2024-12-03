﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebProje.Models;

#nullable disable

namespace WebProje.Migrations
{
    [DbContext(typeof(KuaforContext))]
    [Migration("20241203013817_AddRegisterValidation")]
    partial class AddRegisterValidation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebProje.Models.Calisan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MagazaId")
                        .HasColumnType("int");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UygunlukSaatleri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UzmanlikAlanlari")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MagazaId");

                    b.ToTable("Calisanlar");
                });

            modelBuilder.Entity("WebProje.Models.Islem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Sure")
                        .HasColumnType("int");

                    b.Property<decimal>("Ucret")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Islemler");
                });

            modelBuilder.Entity("WebProje.Models.Kullanici", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sifre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Kullanicilar");
                });

            modelBuilder.Entity("WebProje.Models.Magaza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Adres")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CalismaSaatleri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Magazalar");
                });

            modelBuilder.Entity("WebProje.Models.Randevu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CalisanId")
                        .HasColumnType("int");

                    b.Property<int>("IslemId")
                        .HasColumnType("int");

                    b.Property<int>("KullaniciId")
                        .HasColumnType("int");

                    b.Property<bool>("OnaylandiMi")
                        .HasColumnType("bit");

                    b.Property<string>("Saat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Tarih")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CalisanId");

                    b.HasIndex("IslemId");

                    b.HasIndex("KullaniciId");

                    b.ToTable("Randevular");
                });

            modelBuilder.Entity("WebProje.Models.Calisan", b =>
                {
                    b.HasOne("WebProje.Models.Magaza", "Magaza")
                        .WithMany("Calisanlar")
                        .HasForeignKey("MagazaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Magaza");
                });

            modelBuilder.Entity("WebProje.Models.Randevu", b =>
                {
                    b.HasOne("WebProje.Models.Calisan", "Calisan")
                        .WithMany("Randevular")
                        .HasForeignKey("CalisanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebProje.Models.Islem", "Islem")
                        .WithMany()
                        .HasForeignKey("IslemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebProje.Models.Kullanici", "Kullanici")
                        .WithMany()
                        .HasForeignKey("KullaniciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Calisan");

                    b.Navigation("Islem");

                    b.Navigation("Kullanici");
                });

            modelBuilder.Entity("WebProje.Models.Calisan", b =>
                {
                    b.Navigation("Randevular");
                });

            modelBuilder.Entity("WebProje.Models.Magaza", b =>
                {
                    b.Navigation("Calisanlar");
                });
#pragma warning restore 612, 618
        }
    }
}
