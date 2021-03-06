// <auto-generated />
using System;
using ClinicaMedica.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ClinicaMedica.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20211206134828_migracao")]
    partial class migracao
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ClinicaMedica.Models.Consulta", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Anotacoes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Chave")
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<string>("Comprovante")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ConfirmacaoSeVaiAconsulta")
                        .HasColumnType("bit");

                    b.Property<long?>("Cpf")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<long?>("Crm")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("FimConsulta")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("InicioConsulta")
                        .HasColumnType("datetime2");

                    b.Property<string>("NomePaciente")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Observacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pagamento")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Secretario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Situacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tipo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("dataCadastro")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("dataConfirmação")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("dataConsulta")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("quemCancelou")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("secretariaCancelamento")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Consulta");
                });

            modelBuilder.Entity("ClinicaMedica.Models.Paciente", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Ativo")
                        .HasColumnType("bit");

                    b.Property<long?>("Cpf")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DataNasc")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Observacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Profissao")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Sexo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("Cpf")
                        .IsUnique();

                    b.ToTable("Paciente");
                });

            modelBuilder.Entity("ClinicaMedica.Models.Usuario", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Ativo")
                        .HasColumnType("bit");

                    b.Property<long?>("Cpf")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<long?>("Crm")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DataNasc")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool?>("PrimeiroAcesso")
                        .HasColumnType("bit");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SenhaRecuperacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("TempoMaxRecupSenha")
                        .HasColumnType("datetime2");

                    b.Property<string>("TipoUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Logins");
                });
#pragma warning restore 612, 618
        }
    }
}
