using Microsoft.EntityFrameworkCore;

namespace github_action;

public sealed class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _dbContext;


    public MemberRepository(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext
            .Set<Member>()
            .FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

    public void Add(Member member)
    {
        _dbContext.Set<Member>().Add(member);
        _dbContext.SaveChanges();
    }

}

