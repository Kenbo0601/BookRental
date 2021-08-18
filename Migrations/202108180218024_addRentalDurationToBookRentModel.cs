namespace BookRental.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRentalDurationToBookRentModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookRents", "RentalDuration", c => c.String(nullable: false));
            AlterColumn("dbo.BookRents", "UserId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BookRents", "UserId", c => c.String());
            DropColumn("dbo.BookRents", "RentalDuration");
        }
    }
}
