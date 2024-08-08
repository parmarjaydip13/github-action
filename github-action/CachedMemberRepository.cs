using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace github_action;

public class CachedMemberRepository : IMemberRepository
{
    private readonly IMemberRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly ApplicationDbContext _dbContext;

    public CachedMemberRepository(IMemberRepository decorated, IDistributedCache distributedCache, ApplicationDbContext dbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dbContext;
    }

    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string key = $"member-{id}";

        string? cachedMember = await _distributedCache.GetStringAsync(
            key,
            cancellationToken);

        Member? member;
        if (string.IsNullOrEmpty(cachedMember))
        {
            member = await _decorated.GetByIdAsync(id, cancellationToken);

            if (member is null)
            {
                return member;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(member),
                cancellationToken);

            return member;
        }

        member = JsonConvert.DeserializeObject<Member>(
            cachedMember,
            new JsonSerializerSettings
            {
                ConstructorHandling =
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (member is not null)
        {
            _dbContext.Set<Member>().Attach(member);
        }

        return member;
    }

    public void Add(Member member) => _decorated.Add(member);

}
