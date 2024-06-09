﻿// <auto-generated />
using System;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240611051502_AddTradeEventReplicaSlot")]
    partial class AddTradeEventReplicaSlot
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chipseky.MamkinInvestor.Domain.Order", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<decimal>("CoinsAmount")
                        .HasColumnType("numeric")
                        .HasColumnName("coins_amount");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<decimal>("ForecastedPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("forecasted_price");

                    b.Property<int>("OrderType")
                        .HasColumnType("integer")
                        .HasColumnName("order_type");

                    b.Property<string>("TradingPair")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("trading_pair");

                    b.HasKey("OrderId")
                        .HasName("pk_orders");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.DbTradeEvent", b =>
                {
                    b.Property<long>("DbTradeEventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("db_trade_event_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("DbTradeEventId"));

                    b.Property<object>("Data")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("data");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("DbTradeEventId")
                        .HasName("pk_trade_events");

                    b.ToTable("trade_events", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}