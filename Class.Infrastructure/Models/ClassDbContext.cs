using System;
using System.Collections.Generic;
using Class.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Class.Infrastructure.Models;

public partial class ClassDbContext : DbContext
{
    public ClassDbContext()
    {
    }

    public ClassDbContext(DbContextOptions<ClassDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aianalysis> Aianalyses { get; set; }

    public virtual DbSet<Aidocument> Aidocuments { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<Domain.Entities.Class> Classes { get; set; }

    public virtual DbSet<ClassMember> ClassMembers { get; set; }

    public virtual DbSet<DocumentChunk> DocumentChunks { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<HomeWork> HomeWorks { get; set; }

    public virtual DbSet<JoinRequest> JoinRequests { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADMIN-PC;Database=CriticoreDB;User ID=sa;Password=admin@123;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aianalysis>(entity =>
        {
            entity.HasKey(e => e.AianalysisId).HasName("PK__AIAnalys__D290B97AF7773DCB");

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
                .HasConstraintName("FK__AIAnalysi__Topic__5AEE82B9");
        });

        modelBuilder.Entity<Aidocument>(entity =>
        {
            entity.HasKey(e => e.AidocumentId).HasName("PK__AIDocume__86D79F911659245A");

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
                .HasConstraintName("FK__AIDocumen__Class__6E01572D");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Aidocuments)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIDocumen__Uploa__6EF57B66");
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answers__D482502486EEE96D");

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
                .HasConstraintName("FK__Answers__TopicID__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.Answers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__UserID__5165187F");
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.ChatSessionId).HasName("PK__ChatSess__9AB8242F072F1276");

            entity.Property(e => e.ChatSessionId).HasColumnName("ChatSessionID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatSessi__Class__72C60C4A");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatSessi__Creat__73BA3083");
        });

        modelBuilder.Entity<Domain.Entities.Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A084F08036");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.JoinCode).HasMaxLength(100);
            entity.Property(e => e.Semester).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SubjectCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classes__Created__3C69FB99");
        });

        modelBuilder.Entity<ClassMember>(entity =>
        {
            entity.HasKey(e => e.ClassMemberId).HasName("PK__ClassMem__4205F738434BAF67");

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
                .HasConstraintName("FK__ClassMemb__Class__45F365D3");

            entity.HasOne(d => d.Group).WithMany(p => p.ClassMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__ClassMemb__Group__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.ClassMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassMemb__UserI__46E78A0C");
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.HasKey(e => e.ChunkId).HasName("PK__Document__FBFF9D2059B2FA6D");

            entity.Property(e => e.ChunkId).HasColumnName("ChunkID");
            entity.Property(e => e.AidocumentId).HasColumnName("AIDocumentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Aidocument).WithMany(p => p.DocumentChunks)
                .HasForeignKey(d => d.AidocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DocumentC__AIDoc__7C4F7684");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Group__149AF30AB8181354");

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
                .HasConstraintName("FK__Group__ClassID__403A8C7D");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Group__CreatedBy__412EB0B6");
        });

        modelBuilder.Entity<HomeWork>(entity =>
        {
            entity.HasKey(e => e.HomeworkId).HasName("PK__HomeWork__FDE46A92FB6865CE");

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
                .HasConstraintName("FK__HomeWorks__Creat__5FB337D6");

            entity.HasOne(d => d.Topic).WithMany(p => p.HomeWorks)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomeWorks__Topic__5EBF139D");
        });

        modelBuilder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(e => e.JoinRequestId).HasName("PK__JoinRequ__2573934AD3B9ACC4");

            entity.Property(e => e.JoinRequestId).HasColumnName("JoinRequestID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.ReviewedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Class).WithMany(p => p.JoinRequests)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JoinReque__Class__17F790F9");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.JoinRequestReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__JoinReque__Revie__19DFD96B");

            entity.HasOne(d => d.User).WithMany(p => p.JoinRequestUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JoinReque__UserI__18EBB532");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50613170966C517");

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
                .HasConstraintName("FK__Materials__Class__693CA210");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Materials)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__Uploa__6A30C649");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C79F45147");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ChatSessionId).HasColumnName("ChatSessionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.ChatSession).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__ChatSe__778AC167");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__787EE5A0");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE1059025664B");

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
                .HasConstraintName("FK__Submissio__Group__656C112C");

            entity.HasOne(d => d.Homework).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.HomeworkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Homew__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__UserI__6477ECF3");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topics__022E0F7DF57E8613");

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
                .HasConstraintName("FK__Topics__ClassID__4BAC3F29");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topics__CreatedB__4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC27D59CE3");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534BF75E474").IsUnique();

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
            entity.HasKey(e => e.VoteId).HasName("PK__Votes__52F015E2330368AA");

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
                .HasConstraintName("FK__Votes__AnswerID__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.Votes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Votes__UserID__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
