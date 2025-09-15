using System;
using System.Collections.Generic;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Models;

public partial class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aianalysis> Aianalyses { get; set; }

    public virtual DbSet<Aidocument> Aidocuments { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassMember> ClassMembers { get; set; }

    public virtual DbSet<DocumentChunk> DocumentChunks { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<HomeWork> HomeWorks { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aianalysis>(entity =>
        {
            entity.HasKey(e => e.AianalysisId).HasName("PK__AIAnalys__D290B97A00E04981");

            entity.ToTable("AIAnalysis");

            entity.Property(e => e.AianalysisId).HasColumnName("AIAnalysisID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Sentiment).HasMaxLength(50);
            entity.Property(e => e.TopicId).HasColumnName("TopicID");

            entity.HasOne(d => d.Topic).WithMany(p => p.Aianalyses)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIAnalysi__Topic__14270015");
        });

        modelBuilder.Entity<Aidocument>(entity =>
        {
            entity.HasKey(e => e.AidocumentId).HasName("PK__AIDocume__86D79F9197EF9923");

            entity.ToTable("AIDocuments");

            entity.Property(e => e.AidocumentId).HasColumnName("AIDocumentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("FileURL");

            entity.HasOne(d => d.Class).WithMany(p => p.Aidocuments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIDocumen__Class__2739D489");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Aidocuments)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIDocumen__Uploa__282DF8C2");
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answers__D482502437B612E7");

            entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Topic).WithMany(p => p.Answers)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__TopicID__09A971A2");

            entity.HasOne(d => d.User).WithMany(p => p.Answers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__UserID__0A9D95DB");
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.ChatSessionId).HasName("PK__ChatSess__9AB8242FF66351DD");

            entity.Property(e => e.ChatSessionId).HasColumnName("ChatSessionID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatSessi__Class__2BFE89A6");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatSessi__Creat__2CF2ADDF");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A0D600F08C");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Semester).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SubjectCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__Created__75A278F5");
        });

        modelBuilder.Entity<ClassMember>(entity =>
        {
            entity.HasKey(e => e.ClassMemberId).HasName("PK__ClassMem__4205F738FE09FBDE");

            entity.Property(e => e.ClassMemberId).HasColumnName("ClassMemberID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleInClass).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassMembers)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassMemb__Class__7F2BE32F");

            entity.HasOne(d => d.Group).WithMany(p => p.ClassMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__ClassMemb__Group__01142BA1");

            entity.HasOne(d => d.User).WithMany(p => p.ClassMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassMemb__UserI__00200768");
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.HasKey(e => e.ChunkId).HasName("PK__Document__FBFF9D205CF73DED");

            entity.Property(e => e.ChunkId).HasColumnName("ChunkID");
            entity.Property(e => e.AidocumentId).HasColumnName("AIDocumentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Aidocument).WithMany(p => p.DocumentChunks)
                .HasForeignKey(d => d.AidocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DocumentC__AIDoc__3587F3E0");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Group__149AF30AFB3909B9");

            entity.ToTable("Group");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Group__ClassID__797309D9");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Group__CreatedBy__7A672E12");
        });

        modelBuilder.Entity<HomeWork>(entity =>
        {
            entity.HasKey(e => e.HomeworkId).HasName("PK__HomeWork__FDE46A924A42DCA7");

            entity.Property(e => e.HomeworkId).HasColumnName("HomeworkID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.HomeWorks)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeWorks__Creat__18EBB532");

            entity.HasOne(d => d.Topic).WithMany(p => p.HomeWorks)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeWorks__Topic__17F790F9");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50613174616D100");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("FileURL");

            entity.HasOne(d => d.Class).WithMany(p => p.Materials)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__Class__22751F6C");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__Uploa__236943A5");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C760401D0");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ChatSessionId).HasColumnName("ChatSessionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.ChatSession).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__ChatSe__30C33EC3");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__31B762FC");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE1052A263760");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(500)
                .HasColumnName("AttachmentURL");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.HomeworkId).HasColumnName("HomeworkID");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Group).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__Submissio__Group__1EA48E88");

            entity.HasOne(d => d.Homework).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.HomeworkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Homew__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__UserI__1DB06A4F");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topics__022E0F7DC5742153");

            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.Topics)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topics__ClassID__04E4BC85");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topics__CreatedB__05D8E0BE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC69C0F95");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053410EF0C9C").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("AvatarURL");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__Votes__52F015E2664F20B6");

            entity.Property(e => e.VoteId).HasColumnName("VoteID");
            entity.Property(e => e.Amount).HasDefaultValue(1);
            entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VoteType).HasMaxLength(20);

            entity.HasOne(d => d.Answer).WithMany(p => p.Votes)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Votes__AnswerID__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.Votes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Votes__UserID__10566F31");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
