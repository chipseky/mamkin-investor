using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database.Migrations
{
    /// I run this migration mannually direct in PostgreSQL server
    /// because I faced with error:
    /// cannot create logical replication slot in transaction that has performed writes
    /// https://stackoverflow.com/questions/78598172/is-it-possible-to-create-publication-and-replication-slot-in-the-same-transactio
    /// I just keep it here not to forget to launch it on brand new database
    public partial class AddTradeEventReplicaSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DO $$
                                 BEGIN

                                 IF NOT EXISTS (SELECT 1 FROM pg_catalog.pg_publication WHERE pubname = 'mamkin_investor_trade_events_pub') 
                                 THEN
                                 
                                     CREATE PUBLICATION mamkin_investor_trade_events_pub FOR TABLE 
                                         public.trade_events;

                                 END IF;

                                 IF NOT EXISTS (SELECT 1 from pg_catalog.pg_replication_slots WHERE slot_name = 'mamkin_investor_trade_events_slot') 
                                 THEN
                                 
                                     -- Replication slot, which will hold the state of the replication stream:
                                     PERFORM pg_create_logical_replication_slot('mamkin_investor_trade_events_slot', 'pgoutput');

                                 END IF;
                                 END;

                                 $$ LANGUAGE plpgsql;
                                 """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
