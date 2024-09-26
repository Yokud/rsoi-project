using System.Collections.Concurrent;

namespace DS_Project.Auth.Utils
{
    public class UserSessionKeysStorage
    {
        private readonly ConcurrentDictionary<Guid, Guid> _usersSessionsKeys;

        public UserSessionKeysStorage()
        {
            _usersSessionsKeys = new ConcurrentDictionary<Guid, Guid>();
        }

        public Guid GenerateNewSessionKey(Guid userId)
        {
            var newSessionKey = Guid.NewGuid();

            _usersSessionsKeys[userId] = newSessionKey;

            return newSessionKey;
        }

        public Guid? GetUserSessionKey(Guid userId)
        {
            Guid? result = null;

            if (_usersSessionsKeys.ContainsKey(userId))
                result = _usersSessionsKeys[userId];

            return result;
        }

        public void RemoveUserSessionKey(Guid userId)
        {
            _usersSessionsKeys.TryRemove(userId, out _);
        }
    }
}
