using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Contexts
{
	public class TwoPhaseCommitContext(DbContextOptions<TwoPhaseCommitContext> options) : DbContext(options)
	{
		public DbSet<Node> Nodes { get; set; }
		public DbSet<NodeState> NodeStates { get; set; }
	}
}
