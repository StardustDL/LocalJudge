﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Core.Identity;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Linq;

namespace StarOJ.Data.Provider.SqlServer
{
    public class RoleListProvider : IRoleListProvider
    {
        private readonly OJContext _context;

        public RoleListProvider(OJContext context)
        {
            _context = context;
        }

        public async Task<IRoleProvider> Create(RoleMetadata metadata)
        {
            Role empty = new Role();
            _context.Roles.Add(empty);
            await _context.SaveChangesAsync();
            var res = new RoleProvider(_context, empty);
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<IRoleProvider> Create()
        {
            string name = Guid.NewGuid().ToString();
            return Create(new RoleMetadata { Name = name, NormalizedName = name });
        }

        public async Task Delete(string id)
        {
            var item = await _context.Roles.FindAsync(id);
            if (item != null)
            {
                _context.Roles.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IRoleProvider> Get(string id)
        {
            var item = await _context.Roles.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new RoleProvider(_context, item);
            }
        }

        public Task<IEnumerable<IRoleProvider>> GetAll()
        {
            List<IRoleProvider> res = new List<IRoleProvider>();
            foreach (var v in _context.Roles)
            {
                res.Add(new RoleProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<IRoleProvider>)res);
        }

        public Task<IRoleProvider> GetByName(string name)
        {
            var item = (from x in _context.Roles where x.NormalizedName == name select x).FirstOrDefault();
            if (item == null)
                return Task.FromResult<IRoleProvider>(null);
            else
                return Task.FromResult((IRoleProvider)new RoleProvider(_context, item));
        }

        public async Task<bool> Has(string id)
        {
            return await _context.Roles.FindAsync(id) != null;
        }
    }
}