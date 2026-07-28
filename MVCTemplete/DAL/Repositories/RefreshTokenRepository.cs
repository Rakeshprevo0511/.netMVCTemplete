using System;
using System.Linq;
using MVCTemplete.Helpers;
using MVCTemplete.Models;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly JwtAuthAppDbEntities _dbContext;

    // Takes the DbContext in via constructor rather than creating its own, so the
    // same context (and therefore the same unit of work) is shared across every
    // service touching refresh tokens within a single request.
    public RefreshTokenRepository(JwtAuthAppDbEntities dbContext)
    {
        _dbContext = dbContext;
    }

    public RefreshToken GetActive(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return null;

        string hash = JWTHelper.HashRefreshToken(rawToken);

        return _dbContext.RefreshTokens.FirstOrDefault(x =>
            x.Token == hash &&
            !x.IsRevoked &&
            x.ExpiryDate > DateTime.UtcNow);
    }

    public RefreshToken FindAny(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return null;

        string hash = JWTHelper.HashRefreshToken(rawToken);

        return _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == hash);
    }

    public void Add(int userId, string rawToken, DateTime expiryUtc)
    {
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Token = JWTHelper.HashRefreshToken(rawToken),
            ExpiryDate = expiryUtc,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        });
    }

    public void Revoke(RefreshToken token)
    {
        if (token == null)
            return;

        token.IsRevoked = true;
    }

    public void RevokeAllForUser(int userId)
    {
        var activeTokens = _dbContext.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToList();

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
        }
    }

    public bool RevokeByToken(string rawToken)
    {
        var token = GetActive(rawToken);

        if (token == null)
            return false;

        token.IsRevoked = true;
        SaveChanges();
        return true;
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
    public void DeleteExpiredTokens()
    {
        var cutoff = DateTime.UtcNow.AddDays(-7);

        _dbContext.Database.ExecuteSqlCommand(@"
        DELETE FROM RefreshTokens
        WHERE ExpiryDate <= @p0
           OR (IsRevoked = 1 AND CreatedAt <= @p1)",
            DateTime.UtcNow,
            cutoff);
    }
    public void DeleteExpiredTokensForUser(int userId)
    {
        var expiredTokens = _dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.ExpiryDate <= DateTime.UtcNow)
            .ToList();

        if (expiredTokens.Any())
        {
            _dbContext.RefreshTokens.RemoveRange(expiredTokens);
            _dbContext.SaveChanges();
        }
    }
}
