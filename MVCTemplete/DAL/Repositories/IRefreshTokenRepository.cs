using System;
using MVCTemplete.Models;

/// <summary>
/// Single point of access for the RefreshTokens table. Callers always pass the raw
/// (unhashed) token as received from the client cookie — hashing happens inside the
/// repository so nobody outside this class ever needs to think about it.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>Returns the row for this token only if it is not revoked and not expired.</summary>
    RefreshToken GetActive(string rawToken);

    /// <summary>
    /// Returns the row for this token regardless of its revoked/expired state.
    /// Used to detect refresh-token reuse (a revoked token being presented again).
    /// </summary>
    RefreshToken FindAny(string rawToken);

    /// <summary>Creates and persists a new active refresh token row.</summary>
    void Add(int userId, string rawToken, DateTime expiryUtc);

    /// <summary>Marks a single token row as revoked. Does not save.</summary>
    void Revoke(RefreshToken token);

    /// <summary>
    /// Revokes every currently-active refresh token for a user. Used on login (retire
    /// old sessions) and on reuse-detection (kill the whole token family if theft is
    /// suspected).
    /// </summary>
    void RevokeAllForUser(int userId);

    /// <summary>Revokes the row matching this raw token, if any and if still active.</summary>
    bool RevokeByToken(string rawToken);

    void SaveChanges();
}
