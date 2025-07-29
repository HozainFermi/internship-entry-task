using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;
using TicTacToe.Core.Entities;
using TicTacToe.Core.Enums;


namespace TicTacToe.Infrastructure.DbContexts
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Board)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<char[][]>(v, (JsonSerializerOptions)null))//?
                    .IsRequired();

                entity.Property(g => g.CurrentPlayer)
                    .IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => v[0]);

                entity.Property(g => g.State)
                    .IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => (GameState)Enum.Parse(typeof(GameState), v));

                entity.Property(g => g.MoveCount)
                    .IsRequired();

                entity.Property(g => g.Etag)
                    .IsRequired()
                    .IsConcurrencyToken()
                    .HasDefaultValue("no-etag");

                entity.HasMany(g => g.Moves)
                    .WithOne(m => m.Game)
                    .HasForeignKey(m => m.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(g => g.CreatedAt)
                    .HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Move>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Row)
                    .IsRequired();

                entity.Property(m => m.Column)
                    .IsRequired();

                entity.Property(m => m.Player)
                    .IsRequired();

                entity.Property(m => m.CreatedAt)
                    .HasDefaultValueSql("NOW()");

                entity.Property(m => m.Etag)
                    .IsRequired()
                    .IsConcurrencyToken()
                    .HasDefaultValue("no-etag");

                entity.HasOne(m => m.Game)
                    .WithMany(g => g.Moves)
                    .HasForeignKey(m => m.GameId);
            });
        }

        private static char[][] InitializeDefaultBoard(int size)
        {
            var board = new char[size][];
            for (int i = 0; i < size; i++)
            {
                board[i] = new char[size];
                Array.Fill(board[i], ' ');
            }
            return board;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
    }
}