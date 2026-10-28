using CloudCompare.Models;

namespace CloudCompare.Data
{
    // NOTE: prices / SLA / regions are illustrative sample data for the demo.
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Providers.Any()) return;

            var aws = new Provider { Name = "Amazon Web Services", Headquarters = "Seattle, Washington, USA", Website = "https://aws.amazon.com", FoundedYear = 2006, Description = "The oldest and largest cloud platform with the widest range of services." };
            var azure = new Provider { Name = "Microsoft Azure", Headquarters = "Redmond, Washington, USA", Website = "https://azure.microsoft.com", FoundedYear = 2010, Description = "Strong enterprise and hybrid-cloud platform, tightly integrated with Microsoft tools." };
            var gcp = new Provider { Name = "Google Cloud Platform", Headquarters = "Mountain View, California, USA", Website = "https://cloud.google.com", FoundedYear = 2008, Description = "Known for data analytics, machine learning and Kubernetes." };
            db.Providers.AddRange(aws, azure, gcp);

            db.CloudServices.AddRange(
                new CloudService { Name = "Amazon EC2", Category = "Compute", Provider = aws, Description = "Resizable virtual servers in the cloud.", StartingPrice = 8.50m, FreeTier = true, UptimeSla = 99.99m, RegionCount = 33 },
                new CloudService { Name = "Amazon S3", Category = "Storage", Provider = aws, Description = "Object storage with industry-leading scalability.", StartingPrice = 2.30m, FreeTier = true, UptimeSla = 99.9m, RegionCount = 33 },
                new CloudService { Name = "Amazon RDS", Category = "Database", Provider = aws, Description = "Managed relational databases (MySQL, PostgreSQL, SQL Server).", StartingPrice = 15.00m, FreeTier = true, UptimeSla = 99.95m, RegionCount = 33 },
                new CloudService { Name = "AWS Lambda", Category = "Serverless", Provider = aws, Description = "Run code without managing servers.", StartingPrice = 0m, FreeTier = true, UptimeSla = 99.95m, RegionCount = 33 },

                new CloudService { Name = "Azure Virtual Machines", Category = "Compute", Provider = azure, Description = "On-demand, scalable computing resources.", StartingPrice = 9.00m, FreeTier = true, UptimeSla = 99.99m, RegionCount = 60 },
                new CloudService { Name = "Azure Blob Storage", Category = "Storage", Provider = azure, Description = "Massively scalable object storage for unstructured data.", StartingPrice = 2.00m, FreeTier = true, UptimeSla = 99.9m, RegionCount = 60 },
                new CloudService { Name = "Azure SQL Database", Category = "Database", Provider = azure, Description = "Fully managed SQL Server database service.", StartingPrice = 15.00m, FreeTier = true, UptimeSla = 99.99m, RegionCount = 60 },
                new CloudService { Name = "Azure Functions", Category = "Serverless", Provider = azure, Description = "Event-driven serverless compute.", StartingPrice = 0m, FreeTier = true, UptimeSla = 99.95m, RegionCount = 60 },

                new CloudService { Name = "Compute Engine", Category = "Compute", Provider = gcp, Description = "Customizable virtual machines running in Google's data centers.", StartingPrice = 7.50m, FreeTier = true, UptimeSla = 99.99m, RegionCount = 42 },
                new CloudService { Name = "Cloud Storage", Category = "Storage", Provider = gcp, Description = "Unified object storage for any amount of data.", StartingPrice = 2.00m, FreeTier = true, UptimeSla = 99.95m, RegionCount = 42 },
                new CloudService { Name = "Cloud SQL", Category = "Database", Provider = gcp, Description = "Fully managed MySQL, PostgreSQL and SQL Server.", StartingPrice = 10.00m, FreeTier = false, UptimeSla = 99.95m, RegionCount = 42 },
                new CloudService { Name = "Cloud Functions", Category = "Serverless", Provider = gcp, Description = "Scalable pay-as-you-go functions as a service.", StartingPrice = 0m, FreeTier = true, UptimeSla = 99.95m, RegionCount = 42 }
            );
            db.SaveChanges();

            int Id(string name) => db.CloudServices.First(s => s.Name == name).Id;
            db.Reviews.AddRange(
                new Review { CloudServiceId = Id("Amazon EC2"), ReviewerName = "Rahul Patel", Rating = 5, Comment = "Very flexible, huge choice of instance types.", CreatedOn = DateTime.Now.AddDays(-9) },
                new Review { CloudServiceId = Id("Amazon EC2"), ReviewerName = "Neha Shah", Rating = 4, Comment = "Great performance but pricing can be confusing.", CreatedOn = DateTime.Now.AddDays(-6) },
                new Review { CloudServiceId = Id("Azure Virtual Machines"), ReviewerName = "Amit Joshi", Rating = 4, Comment = "Works smoothly with Windows Server workloads.", CreatedOn = DateTime.Now.AddDays(-5) },
                new Review { CloudServiceId = Id("Compute Engine"), ReviewerName = "Priya Mehta", Rating = 5, Comment = "Per-second billing and live migration are excellent.", CreatedOn = DateTime.Now.AddDays(-4) },
                new Review { CloudServiceId = Id("Amazon S3"), ReviewerName = "Karan Desai", Rating = 5, Comment = "Rock solid storage, never lost a single file.", CreatedOn = DateTime.Now.AddDays(-3) },
                new Review { CloudServiceId = Id("Azure Blob Storage"), ReviewerName = "Sneha Rao", Rating = 4, Comment = "Easy to integrate with .NET applications.", CreatedOn = DateTime.Now.AddDays(-2) },
                new Review { CloudServiceId = Id("Cloud Storage"), ReviewerName = "Vivek Nair", Rating = 4, Comment = "Simple pricing and fast transfers.", CreatedOn = DateTime.Now.AddDays(-2) },
                new Review { CloudServiceId = Id("Azure SQL Database"), ReviewerName = "Anita Verma", Rating = 5, Comment = "Automatic backups and tuning saved us a lot of time.", CreatedOn = DateTime.Now.AddDays(-1) },
                new Review { CloudServiceId = Id("AWS Lambda"), ReviewerName = "Rohan Gupta", Rating = 4, Comment = "Perfect for small event-driven tasks.", CreatedOn = DateTime.Now.AddDays(-1) }
            );
            db.SaveChanges();
        }
    }
}
