using Gemini.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gemini.Data.DbContexts
{
    public class GeminiContext : DbContext
    {
        public DbSet<GeminiIssueEntity>? Issues { get; set; }

        public DbSet<GeminiIssueHistoryEntity>? IssueHistory { get; set; }

        public DbSet<GeminiCustomFieldEntity>? CustomFields { get; set; }

        public DbSet<GeminiProjectEnitity>? Projects { get; set; }

        public static readonly ILoggerFactory ConsoleLoggerFactory
          = LoggerFactory.Create(builder =>
          {
              builder
           .AddFilter((category, level) =>
               category == DbLoggerCategory.Database.Command.Name
               && level == LogLevel.Information)
           .AddConsole();
          });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
                .UseLoggerFactory(ConsoleLoggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer("Data Source=gemini.company.int;Initial Catalog=Gemini;Integrated Security=True");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            LoggerFactory.Create(builder => builder.AddConsole());

            modelBuilder.Entity<GeminiIssueEntity>().HasNoKey().ToView("gemini_issuesview");

            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.IssueId).HasColumnName("issueid");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.CreatedDate).HasColumnName("created");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ResolvedDate).HasColumnName("resolveddate");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.IssueTypeId).HasColumnName("issuetypeid");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ReportedByName).HasColumnName("reporteddesc");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ReportedById).HasColumnName("reportedby");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ProjectCode).HasColumnName("projectcode");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.Summary).HasColumnName("summary");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.Description).HasColumnName("longdesc");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ClosedDate).HasColumnName("closeddate");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.IssueStatusId).HasColumnName("issuestatusid");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.IssueResolutionId).HasColumnName("issueresolutionid");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.IssueKey).HasColumnName("issuekey");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ProjectId).HasColumnName("projectid");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.ProjectName).HasColumnName("projectname");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.Version).HasColumnName("versiondesc");
            modelBuilder.Entity<GeminiIssueEntity>().Property(x => x.Status).HasColumnName("statusdesc");

            modelBuilder.Entity<GeminiCustomFieldEntity>().ToTable("gemini_customfielddata").HasKey(c => c.CustomFieldDataId);

            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.CustomFieldDataId).HasColumnName("customfielddataid");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.CustomFieldId).HasColumnName("customfieldid");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.UserId).HasColumnName("userid");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.ProjectId).HasColumnName("projectid");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.IssueId).HasColumnName("issueid");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.FieldData).HasColumnName("fielddata");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.NumericData).HasColumnName("numericdata");
            modelBuilder.Entity<GeminiCustomFieldEntity>().Property(x => x.Created).HasColumnName("created");

            modelBuilder.Entity<GeminiIssueHistoryEntity>().ToTable("gemini_issuehistory").HasKey(c => c.HistoryId);

            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.HistoryId).HasColumnName("historyid");
            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.IssueId).HasColumnName("issueid");
            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.ProjectId).HasColumnName("projectid");
            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.History).HasColumnName("history");
            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.UserName).HasColumnName("username");
            modelBuilder.Entity<GeminiIssueHistoryEntity>().Property(x => x.Created).HasColumnName("created");

            modelBuilder.Entity<GeminiProjectEnitity>().ToTable("gemini_projects").HasKey(c => c.ProjectId);

            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectId).HasColumnName("projectid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectCode).HasColumnName("projectcode");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectName).HasColumnName("projectname");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectLeader).HasColumnName("projectleader");            
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectDescription).HasColumnName("projectdesc");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectReadonly).HasColumnName("projectreadonly");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ProjectArchived).HasColumnName("projectarchived");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ResourceMode).HasColumnName("resourcemode");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.ComponentMode).HasColumnName("componentmode");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.GlobalSchemeId).HasColumnName("globalschemeid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.IssueTypeSchemeId).HasColumnName("issuetypeschemeID");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.IssuePrioritySchemeId).HasColumnName("issuepriorityschemeID");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.IssueSeveritySchemeId).HasColumnName("issueseverityschemeid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.IssueWorkflowId).HasColumnName("issueworkflowID");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.FieldVisibilitySchemeId).HasColumnName("fieldvisibilityschemeid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.UserId).HasColumnName("userid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.Created).HasColumnName("created");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.TimeStamp).HasColumnName("tstamp");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.Projectlabelid).HasColumnName("projectlabelid");
            modelBuilder.Entity<GeminiProjectEnitity>().Property(x => x.Settingdescription).HasColumnName("settingdescription");
        }
    }
}
