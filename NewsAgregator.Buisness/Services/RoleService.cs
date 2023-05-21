﻿using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Data.Migrations;

namespace NewsAgregator.Buisness.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsyc(RoleDto role)
        {
            if (!IsNameAvailable(role.Name))
            {
                throw new Exception($"Role with name : {role.Name} already exist");
            }
            await _unitOfWork.Roles.AddAsync(_mapper.Map<Role>(role));
            await _unitOfWork.CommitAsync();
            return role.Id;
        }
        private bool IsNameAvailable(string name)
        {
            return !_unitOfWork.Roles.FindBy(role => role.Name == name).Any();
        }
        public List<RoleDto> GetRoles()
        {
            return _unitOfWork.Roles.GetAsQueryable().Select(role => _mapper.Map<RoleDto>(role)).ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(Guid id)
        {
            return _mapper.Map<RoleDto>(await _unitOfWork.Roles.GetByIdAsync(id));
        }
        public async Task Update(RoleDto role)
        {
            var roles = _unitOfWork.Roles.FindBy(roleDb => roleDb.Name.Equals(role.Name));
            if (roles.Count() > 1 | (roles.Any() && roles.First().Id != role.Id))
            {
                throw new InvalidDataException();
            }
            _unitOfWork.Roles
                .Update(_mapper.Map<Role>(role));
            await _unitOfWork.CommitAsync();
        }
    }
}