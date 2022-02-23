using Microsoft.EntityFrameworkCore;
using SmoothNotesAPI.Models;
using SmoothNotesAPI.Models.Logging;

namespace SmoothNotesAPI.Data;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    
    //SmoothNotes Sets
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<LogFile> Logs { get; set; }
    public DbSet<DBAction> Actions { get; set; }
    
    //Testing Sets
    public DbSet<WorkingTest> WorkingTests { get; set; }

}
