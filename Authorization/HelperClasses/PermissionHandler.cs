﻿using Authorization.Entities;
using Authorization.Contracts;

namespace Authorization.HelperClasses
{
    public class PermissionHandler : IPermissionHandler
    {
        private readonly IUsersContainer _usersContainer;

        public PermissionHandler(IUsersContainer usersContainer)
        {
            _usersContainer = usersContainer;
        }

        public bool IsAdministrator(ulong id)
        {
            User user = _usersContainer.ById(id);
            if(user == null)
            {
                return false;
            }
            return user.Administrator;
        }
    }
}